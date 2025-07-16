using Microsoft.EntityFrameworkCore;
using WebApplication.Domain.Models;

namespace WebApplication.Infrastructure.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		public DbSet<Note> Notes => Set<Note>();
		public DbSet<User> Users => Set<User>();


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

		}
	}
}
