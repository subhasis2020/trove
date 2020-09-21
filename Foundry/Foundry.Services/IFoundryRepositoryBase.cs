using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IFoundryRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> AllAsync();
        Task<IEnumerable<T>> GetDataAsync(string qry, object filterParameters);
        Task<T> FindAsync(object filterParameters);
        Task<int> AddAsync(T entity);
        Task<int> AddAsync(IEnumerable<T> entities);
        Task<int> RemoveAsync(object filterParameters);
        Task<int> UpdateAsync(T entity, object filterParameters);
        Task<int> InsertOrUpdateAsync(T entity, object filterParameters);
        Task<IEnumerable<T>> GetDataAsync(object filterParameters);
        Task<T> GetDataByIdAsync(object filterParameters);
        Task<T> GetSingleDataByConditionAsync(object filterParameters);
        Task<T> GetSingleDataAsync(string query, object filterParameters);
        Task<int> DeleteEntityAsync(object filterParameters);
        Task<int> GetPrimaryMaxAsync();
        Task<IEnumerable<T>> GetMultipleDataByConditionAsync(object filterParameters);
    }
}
