using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderedJobs.Domain
{
  public class OrderedJobsCaller
  {
    private readonly HttpClient _httpClient;

    public OrderedJobsCaller()
    {
      _httpClient = new HttpClient();
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public virtual async Task<string> GetOrderedJobs(string url, string testCase)
    {
      var response = await _httpClient.GetAsync(url + "/" + testCase);
      return await response.Content.ReadAsStringAsync();
    }
  }
}