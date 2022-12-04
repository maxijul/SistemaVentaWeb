using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.Entity.Models;

namespace SistemaVenta.BLL.Implementacion
{
  public class CorreoService : ICorreoService
  {
    private readonly IGenericRepository<Configuracion> _repositorio;

    public CorreoService(IGenericRepository<Configuracion> repositorio)
    {
      _repositorio = repositorio;
    }

    public async Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje)
    {
      try
      {
        IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
        Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
        var credenciales = new NetworkCredential(config["correo"], config["clave"]);

        var correo = new MailMessage()
        {
          From = new MailAddress(config["correo"], config["alias"]),
          Subject = asunto,
          Body = mensaje,
          IsBodyHtml= true
        };

        correo.To.Add(new MailAddress(correoDestino));

        var clienteServidor = new SmtpClient()
        {
          Host = config["host"],
          Port = int.Parse(config["puerto"]),
          DeliveryMethod = SmtpDeliveryMethod.Network,
          Credentials = credenciales,
          UseDefaultCredentials = false,
          EnableSsl = true
        };

        clienteServidor.Send(correo);
        return true;

      }
      catch
      {
        return false;
      }
    }
  }
}
