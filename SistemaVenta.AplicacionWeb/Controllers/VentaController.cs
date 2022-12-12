using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace SistemaVenta.AplicacionWeb.Controllers
{
  public class VentaController : Controller
  {
    private readonly ITipoDocumentoVentaService _tipoDocumentoVentaServicio;
    private readonly IVentaService _ventaServicio;
    private readonly IMapper _mapper;
    private readonly IConverter _converter;

    public VentaController(ITipoDocumentoVentaService tipoDocumentoVentaServicio, IVentaService ventaServicio, IMapper mapper, IConverter converter)
    {
      _tipoDocumentoVentaServicio = tipoDocumentoVentaServicio;
      _ventaServicio = ventaServicio;
      _mapper = mapper;
      _converter = converter;
    }


    public IActionResult NuevaVenta()
    {
      return View();
    }

    public IActionResult HistorialVenta()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListaTipoDocumentoVenta()
    {
      List<VMTipoDocumentoVenta> vmListaTipoDocumentos = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaServicio.Lista());
      return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentos);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos(string busqueda)
    {
      List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaServicio.ObtenerProductos(busqueda));
      return StatusCode(StatusCodes.Status200OK, vmListaProductos);
    }

    [HttpGet]
    public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
    {
      List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaServicio.Historial(numeroVenta, fechaInicio, fechaFin));
      return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
    {
      GenericResponse<VMVenta> genericResponse = new GenericResponse<VMVenta>();
      try
      {
        //TODO realizar logica de logeo
        modelo.IdUsuario = 15;

        var ventaMapeada = _mapper.Map<Venta>(modelo);
        Venta ventaCreada = await _ventaServicio.Registrar(ventaMapeada);
        modelo = _mapper.Map<VMVenta>(ventaCreada);

        genericResponse.Estado = true;
        genericResponse.Objeto = modelo;

      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status200OK, genericResponse);

    }

    public IActionResult MostrarPDFVenta(string numeroVenta)
    {
      string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";

      var pdf = new HtmlToPdfDocument()
      {
        GlobalSettings = new GlobalSettings()
        {
          PaperSize = PaperKind.A4,
          Orientation= Orientation.Portrait
        },
        Objects =
        {
          new ObjectSettings()
          {
            Page = urlPlantillaVista
          }
        }
      };

      var archivoPDF = _converter.Convert(pdf);

      return File(archivoPDF, "application/pdf");

    }


  }
}
