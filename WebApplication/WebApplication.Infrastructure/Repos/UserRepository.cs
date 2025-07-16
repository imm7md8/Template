using Microsoft.EntityFrameworkCore;
using WebApplication.Domain.Models;
using WebApplication.Infrastructure.Data;
using WebApplication.Infrastructure.Repos.Interfaces;

namespace WebApplication.Infrastructure.Repos
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<User?> GetByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<User?> GetByUsernameAsync(string username)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
		}

		public async Task<User?> GetByIdAsync(Guid id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task AddAsync(User user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
		}
	}
}
