using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IReposotoryBase<T>
    {
        IQueryable<T> FindAll(bool trackingChanges);
        IQueryable<T> FindByCondiction(Expression<Func<T, bool>> expression, bool trackingChanges);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);

    }
}
