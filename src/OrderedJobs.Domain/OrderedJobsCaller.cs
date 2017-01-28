using System.Net.Http;
using System.Threading.Tasks;

namespace OrderedJobs.Domain
{
  public class OrderedJobsCaller : IOrderedJobsCaller
  {
    private readonly HttpClient _httpClient;

    public OrderedJobsCaller()
    {
      _httpClient = new HttpClient();
    }

    public async Task<string> GetOrderedJobs(string url, string testCase)
    {
      var response = await _httpClient.GetAsync(url + "/" + testCase);
      return await response.Content.ReadAsStringAsync();
    }
  }
}