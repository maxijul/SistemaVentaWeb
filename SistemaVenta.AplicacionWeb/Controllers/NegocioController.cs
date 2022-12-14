using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
  [Authorize]
  public class NegocioController : Controller
  {
    private readonly IMapper _mapper;
    private readonly INegocioService _negocioService;

    public NegocioController(IMapper mapper, INegocioService negocioService)
    {
      _mapper = mapper;
      _negocioService = negocioService;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Obtener()
    {
      GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();

      try
      {
        VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
        genericResponse.Estado = true;
        genericResponse.Objeto = vmNegocio;
      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);

    }

    [HttpPut]
    public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
    {
      GenericResponse<VMNegocio> genericResponse = new GenericResponse<VMNegocio>();

      try
      {
        VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
        string nombreLogo = "";
        Stream logoStream = null;

        if(logo != null)
        {
          string nombre_en_codigo = Guid.NewGuid().ToString("N");
          string extension = Path.GetExtension(logo.FileName);
          nombreLogo = string.Concat(nombre_en_codigo, extension);
          logoStream = logo.OpenReadStream();
        }

        var negocioMapeado = _mapper.Map<Negocio>(vmNegocio);

        Negocio negocioEditado = await _negocioService.GuardarCambios(negocioMapeado, logoStream, nombreLogo);

        vmNegocio = _mapper.Map<VMNegocio>(negocioEditado);

        genericResponse.Estado = true;
        genericResponse.Objeto = vmNegocio;
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
