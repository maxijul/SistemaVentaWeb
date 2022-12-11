using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity.Models;


namespace SistemaVenta.BLL.Implementacion
{
  public class VentaService : IVentaService
  {
    public readonly IGenericRepository<Producto> _repositorioProducto;
    public readonly IVentaRepository _repositorioVenta;

    public VentaService(IGenericRepository<Producto> repositorioProducto, IVentaRepository repistorioVenta)
    {
      _repositorioProducto = repositorioProducto;
      _repositorioVenta = repistorioVenta;
    }

    public async Task<List<Producto>> ObtenerProductos(string busqueda)
    {
      IQueryable<Producto> query = await _repositorioProducto.Consultar(p =>
          p.EsActivo == true &&
          p.Stock > 0 &&
          string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
        );

      return query.Include(c => c.IdCategoriaNavigation).ToList();
    }

    public async Task<Venta> Registrar(Venta entidad)
    {
      try
      {
        return await _repositorioVenta.Registrar(entidad);
      }
      catch
      {
        throw;
      }
    }

    public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
    {
      IQueryable<Venta> query = await _repositorioVenta.Consultar();

      fechaInicio = fechaInicio is null ? "" : fechaInicio;
      fechaFin = fechaFin is null ? "" : fechaFin;

      if (fechaInicio != "" && fechaFin != "")
      {
        DateTime fechaInicioParseada = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
        DateTime fechaFinParseada = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

        return query.Where(v =>
          v.FechaRegistro.Value.Date >= fechaInicioParseada.Date &&
          v.FechaRegistro.Value.Date <= fechaFinParseada.Date
        )
          .Include(tipoDocumentoVenta => tipoDocumentoVenta.IdTipoDocumentoVentaNavigation)
          .Include(usuario => usuario.IdUsuarioNavigation)
          .Include(detalleVenta => detalleVenta.DetalleVenta)
          .ToList();
      }
      else
      {
        return query.Where(v =>
          v.NumeroVenta == numeroVenta
        )
          .Include(tipoDocumento => tipoDocumento.IdTipoDocumentoVentaNavigation)
          .Include(usuario => usuario.IdUsuarioNavigation)
          .Include(detalleVenta => detalleVenta.DetalleVenta)
          .ToList();
      }

    }

    public async Task<Venta> Detalle(string numeroVenta)
    {
      IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == numeroVenta);

      return query
       .Include(tipoDocumento => tipoDocumento.IdTipoDocumentoVentaNavigation)
       .Include(usuario => usuario.IdUsuarioNavigation)
       .Include(detalleVenta => detalleVenta.DetalleVenta)
       .First();

    }

    public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
    {
      DateTime fechaInicioParseada = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
      DateTime fechaFinParseada = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

      List<DetalleVenta> lista = await _repositorioVenta.Reporte(fechaInicioParseada, fechaFinParseada);

      return lista;

    }

  }
}
