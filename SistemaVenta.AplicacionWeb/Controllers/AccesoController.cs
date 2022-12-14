using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SistemaVenta.AplicacionWeb.Controllers
{

  public class AccesoController : Controller
  {
    private readonly IUsuarioService _usuarioServicio;


    public AccesoController(IUsuarioService usuarioServicio)
    {
      _usuarioServicio = usuarioServicio;
    }

    public IActionResult Login()
    {
      ClaimsPrincipal claimUser = HttpContext.User; // Aqui guardamos la sesion del usuario

      if (claimUser.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Home");
      }

      return View();
    }

    public IActionResult RestablecerClave()
    {
      return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(VMUsuarioLogin modelo)
    {
      Usuario usuarioEncontrado = await _usuarioServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

      if(usuarioEncontrado == null)
      {
        ViewData["Mensaje"] = "No se encontraron coincidencias";
        return View();
      }

      ViewData["Mensaje"] = null;
      // Los claims son el cuerpo de la cookie
      List<Claim> claims = new List<Claim>()
      {
        new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
        new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
        new Claim(ClaimTypes.Role, usuarioEncontrado.IdRol.ToString()),
        new Claim("UrlFoto", usuarioEncontrado.UrlFoto),
      };

      ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

      AuthenticationProperties properties = new AuthenticationProperties()
      {
        AllowRefresh = true,
        IsPersistent = modelo.MantenerSesion
      };

      await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        properties
      );

      return RedirectToAction("Index", "Home");

    }

    [HttpPost]
    public async Task<IActionResult> RestablecerClave(VMUsuario modelo)
    {
      try
      {
        string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
        bool resultado = await _usuarioServicio.RestablecerClave(modelo.Correo, urlPlantillaCorreo);

        if (resultado)
        {
          ViewData["Mensaje"] = "Listo, su contraseña fue restablecida. Revise su correo";
          ViewData["MensajeError"] = null;
        }
        else
        {
          ViewData["Mensaje"] = null;
          ViewData["MensajeError"] = "Tenemos problemas. Por favor inténtelo de nuevo más tarde";
        }
      }
      catch (Exception ex)
      {
        ViewData["Mensaje"] = null;
        ViewData["MensajeError"] = ex.Message;
      }

      return View();
    }

  }
}
