using Shared.RequestFeatures;

namespace Contracts.Interfaces;

public interface IBaseInterface<T, U> where T : class where U : class
{
    Task<PageList<T>> GetAll(U pagination, bool trackChanges);
    Task<T> GetByCondiction(string Id, bool trackChanges);
    Task<T> CreateCompany(T model);
    Task<IEnumerable<T>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges);
    Task DeleteCompany(string Id, bool trackChanges);
}
