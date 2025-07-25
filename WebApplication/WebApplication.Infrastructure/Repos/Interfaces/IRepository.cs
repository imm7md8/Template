﻿namespace WebApplication.Infrastructure.Repos.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(T entity);
		Task DeleteAsync(int id);
	}
}
