using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Infrastructure.Data;
using WebApplication.Infrastructure.Repos.Interfaces;
using WebApplication.Infrastructure.Repos;
namespace WebApplication.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

			return services;
		}
	}
}
