using BugTracker.Data;
using System.Linq.Expressions;

namespace BugTracker.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _db.Set<T>().AddRange(entities);
        }
        public IEnumerable<T> Find(Expression<Func<T,bool>> expression)
        {
            return _db.Set<T>().Where(expression);
        }
        public IEnumerable<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }
        public T? GetById(int id)
        {
            return _db.Set<T>().Find(id);
        }
        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
        }
    }
}
