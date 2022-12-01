﻿namespace SistemaVenta.AplicacionWeb.Utilidades.Response
{
  public class GenericResponse<TObject>
  {
    public bool Estado { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public TObject? Objeto { get; set; }
    public List<TObject>? ListaObjeto { get; set; }
  }
}
