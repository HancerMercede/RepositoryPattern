using Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Linq.Expressions;

namespace Repository;
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected RepositoryContext _repositoryContext;
    
    public RepositoryBase(RepositoryContext repositoryContext) =>
        _repositoryContext = repositoryContext;
    

    public IQueryable<T> FindAll(bool trackingChanges) =>
       !trackingChanges ? 
            _repositoryContext.Set<T>()
                .AsNoTracking() 
             : _repositoryContext.Set<T>();
        


    public IQueryable<T> FindByCondiction(Expression<Func<T, bool>> expression, bool trackingChanges) => 
        !trackingChanges ?
              _repositoryContext.Set<T>()
                  .Where(expression)
                  .AsNoTracking() 
              : _repositoryContext.Set<T>()
                  .Where(expression);



    public async Task Create(T entity) => await _repositoryContext.Set<T>().AddAsync(entity);

    public async Task Update(T entity) => await Task.FromResult(_repositoryContext.Set<T>().Update(entity));

    public async Task Delete(T entity) => await Task.FromResult(_repositoryContext.Set<T>().Remove(entity));

}
