using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Entity.Models;

namespace SistemaVenta.BLL.Implementacion
{
  public class ProductoService : IProductoService
  {
    private readonly IGenericRepository<Producto> _repositorio;
    private readonly IFireBaseService _fireBaseServicio;

    public ProductoService(IGenericRepository<Producto> repositorio, IFireBaseService fireBaseServicio)
    {
      _repositorio = repositorio;
      _fireBaseServicio = fireBaseServicio;
    }

    public async Task<List<Producto>> Lista()
    {
      IQueryable<Producto> query = await _repositorio.Consultar();
      return query.Include(c => c.IdCategoriaNavigation).ToList();
    }
    public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
    {
      Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

      if(productoExiste != null)
        throw new TaskCanceledException("El código de barras ya existe");

      try
      {
        entidad.NombreImagen = nombreImagen;
        if(imagen != null)
        {
          string urlImagen = await _fireBaseServicio.SubirStorage(imagen, "carpeta_producto", nombreImagen);
          entidad.UrlImagen = urlImagen;
        }

        Producto productoCreado = await _repositorio.Crear(entidad);

        if(productoCreado.IdProducto == 0)
          throw new TaskCanceledException("No se pudo crear el producto");

        IQueryable<Producto> query = await _repositorio.Consultar(p => p.IdProducto == productoCreado.IdProducto);

        productoCreado = query.Include(c => c.IdCategoriaNavigation).First();

        return productoCreado;
      }
      catch
      {
        throw;
      }

    }

    public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
    {
      Producto productoExiste = await _repositorio.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

      if(productoExiste != null)
        throw new TaskCanceledException("El código de barras ya esta asignado a otro producto");

      try
      {
        IQueryable<Producto> queryProducto = await _repositorio.Consultar(p => p.IdProducto == entidad.IdProducto);
        Producto productoParaEditar = queryProducto.First();

        productoParaEditar.CodigoBarra = entidad.CodigoBarra;
        productoParaEditar.Marca = entidad.Marca;
        productoParaEditar.Descripcion = entidad.Descripcion;
        productoParaEditar.IdCategoria = entidad.IdCategoria;
        productoParaEditar.Stock = entidad.Stock;
        productoParaEditar.Precio = entidad.Precio;
        productoParaEditar.EsActivo = entidad.EsActivo;

        if(productoParaEditar.NombreImagen == "")
          productoParaEditar.NombreImagen = nombreImagen;
        


        if(imagen != null)
        {
          string urlImagen = await _fireBaseServicio.SubirStorage(imagen, "carpeta_producto", productoParaEditar.NombreImagen);
          productoParaEditar.UrlImagen = urlImagen;
        }

        bool respuesta = await _repositorio.Editar(productoParaEditar);

        if(!respuesta)
          throw new TaskCanceledException("No se pudo modificar el producto");

        Producto productoEditado = queryProducto.Include(c => c.IdCategoriaNavigation).First();
      
        return productoEditado;
      }
      catch 
      {
        throw;
      }
      
    }

    public async Task<bool> Eliminar(int idProducto)
    {
      try
      {
        Producto productoEncontrado = await _repositorio.Obtener(p => p.IdProducto == idProducto);

        if(productoEncontrado == null)
          throw new TaskCanceledException("El producto no existe");

        string nombreImagen = productoEncontrado.NombreImagen;

        bool respuesta = await _repositorio.Eliminar(productoEncontrado);

        if (respuesta)
          await _fireBaseServicio.EliminarStorage("carpeta_producto", nombreImagen);

        return true;

      }
      catch 
      {
        throw;
      }
    }

  }
}
