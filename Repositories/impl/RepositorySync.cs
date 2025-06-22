using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sep490_Eduseen_BE.Models;

namespace Sep490_Eduseen_BE.Repositories.impl
{
    public class RepositorySync<T> : IRepositorySync<T> where T : class
    {
        private readonly Sep490EduseenContext _context;
        private readonly DbSet<T> _dbSet;

        public RepositorySync(Sep490EduseenContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public IEnumerable<T> Get()
        {
            IQueryable<T> query = _dbSet;
            var data = query.ToList();
            return data;
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id) ?? throw new InvalidOperationException($"Entity with ID {id} not found.");
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}