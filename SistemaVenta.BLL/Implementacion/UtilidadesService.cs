using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BLL.Implementacion
{
  public class UtilidadesService : IUtilidadesService
  {
    public string GenerarClave()
    {
      string clave = Guid.NewGuid().ToString("N").Substring(0, 6); // N Significa que utiliza letras y numeros
      return clave;
    }
    public string ConvertirSha256(string texto)
    {
      StringBuilder stringBuilder = new StringBuilder();

      using (SHA256 hash = SHA256.Create())
      {
        Encoding encoding = Encoding.UTF8;

        byte[] result = hash.ComputeHash(encoding.GetBytes(texto));

        foreach (byte b in result)
        {
          stringBuilder.Append(b.ToString("x2"));
        }
      }

      return stringBuilder.ToString();
    }

  }
}
