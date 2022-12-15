using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;


namespace SistemaVenta.AplicacionWeb.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {

    private readonly IUsuarioService _usuarioService;
    private readonly IMapper _mapper;

    public HomeController(IUsuarioService usuarioService, IMapper mapper)
    {
      _usuarioService = usuarioService;
      _mapper = mapper;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    public IActionResult Perfil()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerUsuario()
    {
      GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();


      try
      {
        ClaimsPrincipal claimUser = HttpContext.User;

        string idUsuario = claimUser.Claims
          .Where(c => c.Type == ClaimTypes.NameIdentifier)
          .Select(c => c.Value).SingleOrDefault();

        VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerPorId(int.Parse(idUsuario)));

        genericResponse.Estado = true;
        genericResponse.Objeto = usuario;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
    {
      GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();


      try
      {
        ClaimsPrincipal claimUser = HttpContext.User;

        string idUsuario = claimUser.Claims
          .Where(c => c.Type == ClaimTypes.NameIdentifier)
          .Select(c => c.Value).SingleOrDefault();

        Usuario entidad = _mapper.Map<Usuario>(modelo);

        entidad.IdUsuario = int.Parse(idUsuario);

        bool resultado = await _usuarioService.GuardarPerfil(entidad);

        genericResponse.Estado = resultado;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
    {
      GenericResponse<bool> genericResponse = new GenericResponse<bool>();


      try
      {
        ClaimsPrincipal claimUser = HttpContext.User;

        string idUsuario = claimUser.Claims
          .Where(c => c.Type == ClaimTypes.NameIdentifier)
          .Select(c => c.Value).SingleOrDefault();


        bool resultado = await _usuarioService.CambiarClave(
          int.Parse(idUsuario),
          modelo.ClaveActual,
          modelo.ClaveNueva
          );

        genericResponse.Estado = resultado;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Salir()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      return RedirectToAction("Login", "Acceso");
    }

  }
}