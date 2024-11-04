namespace Repository;
public abstract class RepositoryBase<T>(RepositoryContext repositoryContext) : IRepositoryBase<T>
    where T : class
{
    public IQueryable<T> FindAll(bool trackingChanges) =>
       !trackingChanges ? 
            repositoryContext.Set<T>()
                .AsNoTracking() 
             : repositoryContext.Set<T>();
        


    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackingChanges) => 
        !trackingChanges ?
              repositoryContext.Set<T>()
                  .Where(expression)
                  .AsNoTracking() 
              : repositoryContext.Set<T>()
                  .Where(expression);



    public async Task Create(T entity) => await repositoryContext.Set<T>().AddAsync(entity);
    public async Task Update(T entity) => await Task.FromResult(repositoryContext.Set<T>().Update(entity));
    public async Task Delete(T entity) => await Task.FromResult(repositoryContext.Set<T>().Remove(entity));

}
