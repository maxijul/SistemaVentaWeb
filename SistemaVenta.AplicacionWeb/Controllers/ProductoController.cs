using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;

namespace SistemaVenta.AplicacionWeb.Controllers
{
  public class ProductoController : Controller
  {
    private readonly IMapper _mapper;
    private readonly IProductoService _productoServicio;

    public ProductoController(IMapper mapper, IProductoService productoServicio)
    {
      _mapper = mapper;
      _productoServicio = productoServicio;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
      List<VMProducto> vmProductoLista = _mapper.Map<List<VMProducto>>(await _productoServicio.Lista());

      return StatusCode(StatusCodes.Status200OK, new { data = vmProductoLista });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
    {
      GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();

      try
      {
        VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
        string nombreImagen = "";
        Stream imagenStream = null;

        if(imagen != null)
        {
          string nombre_en_codigo = Guid.NewGuid().ToString("N");
          string extension = Path.GetExtension(imagen.FileName);
          nombreImagen = string.Concat(nombre_en_codigo, extension);
          imagenStream = imagen.OpenReadStream();
        }

        var productoMapeado = _mapper.Map<Producto>(vmProducto);
        Producto productoCreado = await _productoServicio.Crear(productoMapeado, imagenStream, nombreImagen);

        vmProducto = _mapper.Map<VMProducto>(productoCreado);
        
        genericResponse.Estado = true;
        genericResponse.Objeto = vmProducto;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status201Created, genericResponse);

    }

    [HttpPut]
    public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
    {
      GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();

      try
      {
        VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

        string nombreImagen = "";
        Stream imagenStream = null;

        if (imagen != null)
        {
          string nombre_en_codigo = Guid.NewGuid().ToString("N");
          string extension = Path.GetExtension(imagen.FileName);
          nombreImagen = string.Concat(nombre_en_codigo, extension);
          imagenStream = imagen.OpenReadStream();
        }

        var productoMapeado = _mapper.Map<Producto>(vmProducto);
        Producto productoEditado = await _productoServicio.Editar(productoMapeado, imagenStream, nombreImagen);

        vmProducto = _mapper.Map<VMProducto>(productoEditado);

        genericResponse.Estado = true;
        genericResponse.Objeto = vmProducto;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);

    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int idProducto)
    {
      GenericResponse<string> genericResponse = new GenericResponse<string>();

      try
      {
        genericResponse.Estado = await _productoServicio.Eliminar(idProducto);
      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);

    }

  }
}
