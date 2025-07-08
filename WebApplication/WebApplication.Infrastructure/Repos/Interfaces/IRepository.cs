using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.Infrastructure.Repos.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(T entity);
		Task DeleteAsync(int id);
	}
}
