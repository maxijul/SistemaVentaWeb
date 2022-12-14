using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVenta.AplicacionWeb.Controllers
{
  [Authorize]
  public class CategoriaController : Controller
  {

    private readonly IMapper _mapper;
    private readonly ICategoriaService _categoriaService;

    public CategoriaController(IMapper mapper, ICategoriaService categoriaService)
    {
      _mapper = mapper;
      _categoriaService = categoriaService;
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
      List<VMCategoria> vmCategoriaLista = _mapper.Map<List<VMCategoria>>(await _categoriaService.Lista());

      return StatusCode(StatusCodes.Status200OK, new { data = vmCategoriaLista });
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] VMCategoria modelo)
    {
      GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>();

      try
      {
        var categoriaMapeada = _mapper.Map<Categoria>(modelo);
        Categoria categoriaCreada = await _categoriaService.Crear(categoriaMapeada);

        modelo = _mapper.Map<VMCategoria>(categoriaCreada);

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


    [HttpPut]
    public async Task<IActionResult> Editar([FromBody] VMCategoria modelo)
    {
      GenericResponse<VMCategoria> genericResponse = new GenericResponse<VMCategoria>();

      try
      {
        var categoriaMapeada = _mapper.Map<Categoria>(modelo);
        Categoria categoriaEditada = await _categoriaService.Editar(categoriaMapeada);

        modelo = _mapper.Map<VMCategoria>(categoriaEditada);

        genericResponse.Estado = true;
        genericResponse.Objeto = modelo;
      }
      catch (Exception ex)
      {
        genericResponse.Estado = false;
        genericResponse.Mensaje = ex.Message;
      }

      return StatusCode(StatusCodes.Status201Created, genericResponse);

    }


    [HttpDelete]
    public async Task<IActionResult> Eliminar(int idCategoria)
    {
      GenericResponse<string> genericResponse = new GenericResponse<string>();

      try
      {
        genericResponse.Estado = await _categoriaService.Eliminar(idCategoria);
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
