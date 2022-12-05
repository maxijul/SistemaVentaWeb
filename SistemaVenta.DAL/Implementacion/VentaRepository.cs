using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity.Models;


namespace SistemaVenta.DAL.Implementacion
{
  public class VentaRepository : GenericRepository<Venta>, IVentaRepository
  {
    private readonly DBVENTAContext _dbContext;

    public VentaRepository(DBVENTAContext dbContext) : base(dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<Venta> Registrar(Venta entidad)
    {
      Venta ventaGenerada = new Venta();

      // Realizo una transaccion
      using (var transaction = _dbContext.Database.BeginTransaction())
      {
        try
        {
          foreach (DetalleVenta detalleVenta in entidad.DetalleVenta)
          {
            Producto productoEncontrado = _dbContext.Productos
                .Where(p => p.IdProducto == detalleVenta.IdProducto).First();

            productoEncontrado.Stock = productoEncontrado.Stock - detalleVenta.Cantidad;
            _dbContext.Productos.Update(productoEncontrado);
          }
          await _dbContext.SaveChangesAsync();

          NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos
              .Where(n => n.Gestion == "venta").First();

          correlativo.UltimoNumero += 1;
          correlativo.FechaActualizacion = DateTime.Now;

          _dbContext.NumeroCorrelativos.Update(correlativo);
          await _dbContext.SaveChangesAsync();

          string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
          string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
          numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

          entidad.NumeroVenta = numeroVenta;

          await _dbContext.Venta.AddAsync(entidad);
          await _dbContext.SaveChangesAsync();

          ventaGenerada = entidad;

          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }

      return ventaGenerada;
    }

    public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
    {
      // El Include es hacer un join en EF
      //  El ThenInclude es cuando hacemos un join y ya tenemos esa entidad dentro de donde llamamos el metodo
      // tdv = tipo detalle venta
      // dv = detalle venta
      List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
          .Include(v => v.IdVentaNavigation)
          .ThenInclude(u => u.IdUsuarioNavigation)
          .Include(v => v.IdVentaNavigation)
          .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
          .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date &&
              dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date).ToListAsync();

      return listaResumen;

    }
  }
}
