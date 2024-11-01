using System.Linq.Expressions;

namespace Contracts.Interfaces;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackingChanges);
    IQueryable<T> FindByCondiction(Expression<Func<T, bool>> expression, bool trackingChanges);
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(T entity);

}

