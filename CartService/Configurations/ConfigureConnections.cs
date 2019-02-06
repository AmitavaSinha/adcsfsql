using Cart.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Configurations
{
    public static class ConfigureConnections
    {
        public static IServiceCollection AddConnectionProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("ConnectionString");
            services.AddDbContext<ModernAppDBContext>(options => options.UseSqlServer(connection));

            return services;
        }
    }
}
