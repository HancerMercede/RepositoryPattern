

namespace Service.Contracts.Interfaces;

internal interface IRead<T>
{
    Task<(T, T)> GetAll2(T Pagination, bool tracking);
}
