using Inventory.Dal.Contracts;
using Inventory.Dal.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Dal
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccessServices(
     this IServiceCollection services,
     IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("InventoryDb")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

    }
}
