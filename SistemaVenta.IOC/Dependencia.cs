using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
//using SistemaVenta.DAL.Interfaces;
//using SistemaVenta.DAL.Implementacion;
//using SistemaVenta.BLL.Interfaces;
//using SistemaVenta.BLL.Implementacion;


namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        // Inyectamos la dependencia del DBContext
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBVENTAContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });
        }
    }
}
