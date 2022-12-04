using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;

namespace SistemaVenta.AplicacionWeb.Controllers
{
  public class UsuarioController : Controller
  {
    private readonly IUsuarioService _usuarioServicio;
    private readonly IRolService _rolServicio;
    private readonly IMapper _mapper;

    public UsuarioController(IUsuarioService usuarioServicio, IRolService rolServicio, IMapper mapper)
    {
      _usuarioServicio = usuarioServicio;
      _rolServicio = rolServicio;
      _mapper = mapper;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListaRoles()
    {
      var lista = await _rolServicio.Lista();
      List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(lista);

      return StatusCode(StatusCodes.Status200OK, vmListaRoles);
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
      var lista = await _usuarioServicio.Lista();
      List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(lista);

      return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
    {
      GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

      try
      {
        VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
        string nombreFoto = "";
        Stream fotoStream = null;

        if(foto != null)
        {
          string nombreEnCodigo = Guid.NewGuid().ToString("N");
          string extension = Path.GetExtension(foto.FileName);
          nombreFoto = string.Concat(nombreEnCodigo, extension);
          fotoStream = foto.OpenReadStream();
        }

        string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
        var usuarioMapeado = _mapper.Map<Usuario>(vmUsuario);

        Usuario usuarioCreado = await _usuarioServicio.Crear(usuarioMapeado, fotoStream, nombreFoto, urlPlantillaCorreo);

        vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

        genericResponse.Estado = true;
        genericResponse.Objeto = vmUsuario;
      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status201Created, genericResponse);

    }

    [HttpPut]
    public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
    {
      GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

      try
      {
        VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
        string nombreFoto = "";
        Stream fotoStream = null;

        if (foto != null)
        {
          string nombreEnCodigo = Guid.NewGuid().ToString("N");
          string extension = Path.GetExtension(foto.FileName);
          nombreFoto = string.Concat(nombreEnCodigo, extension);
          fotoStream = foto.OpenReadStream();
        }

        var usuarioMapeado = _mapper.Map<Usuario>(vmUsuario);

        Usuario usuarioEditado = await _usuarioServicio.Editar(usuarioMapeado, fotoStream, nombreFoto);

        vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

        genericResponse.Estado = true;
        genericResponse.Objeto = vmUsuario;
      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);

    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int idUsuario)
    {
      GenericResponse<string> genericResponse = new GenericResponse<string>();

      try
      {
        genericResponse.Estado = await _usuarioServicio.Eliminar(idUsuario);
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
