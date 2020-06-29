using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {

      Task DeleteAsync(T entity);

      Task<T> GetAsync(long id);

      Task<List<T>> GetAsync();

      Task<T> InsertAsync(T entity);

      Task UpdateAsync(T entity);
        
    }
}
