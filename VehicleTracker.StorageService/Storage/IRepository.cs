using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleTracker.StorageService.Storage
{
    public interface IRepository<TKey, T, TFilter>
    {
        Task<IEnumerable<T>> List(TFilter filter);
        Task<T> Get(TKey id);
        Task<T> Create(T vehicle);
        Task<T> Update(TKey id, T vehicle);
        Task Delete(TKey id);
    }
}
