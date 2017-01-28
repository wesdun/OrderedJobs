using System.Threading.Tasks;

namespace OrderedJobs.Domain
{
  public interface IOrderedJobsCaller
  {
    Task<string> GetOrderedJobs(string url, string testCase);
  }
}