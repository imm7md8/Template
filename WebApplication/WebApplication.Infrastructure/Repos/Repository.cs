﻿using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Data;
using WebApplication.Infrastructure.Repos.Interfaces;

namespace WebApplication.Infrastructure.Repos
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;
		private readonly DbSet<T> _dbSet;

		public Repository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _dbSet.FindAsync(id);
			if (entity != null)
			{
				_dbSet.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
