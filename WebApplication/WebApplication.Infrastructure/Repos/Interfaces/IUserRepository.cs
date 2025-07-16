using WebApplication.Domain.Models;

namespace WebApplication.Infrastructure.Repos.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetByEmailAsync(string email);
		Task<User?> GetByUsernameAsync(string username);
		Task<User?> GetByIdAsync(Guid id);
		Task AddAsync(User user);
	}
}
