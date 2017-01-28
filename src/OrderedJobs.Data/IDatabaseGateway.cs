using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderedJobs.Data
{
  public interface IDatabaseGateway<T>
  {
    void Add(T item);
    void DeleteAll();
    void Delete(T item);
    Task<IEnumerable<T>> GetAll();
  }
}