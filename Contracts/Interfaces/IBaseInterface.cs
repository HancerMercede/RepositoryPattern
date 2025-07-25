using Shared.RequestFeatures;

namespace Contracts.Interfaces;

public interface IBaseInterface<T, U> where T : class where U : class
{ 
    Task<PageList<T>> GetAll(U pagination, bool trackChanges);
    Task<T> GetByCondition(string id, bool trackChanges);
    Task<T> CreateRecord(T model);
    Task<IEnumerable<T>> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    Task DeleteRecord(string id, bool trackChanges);
}
