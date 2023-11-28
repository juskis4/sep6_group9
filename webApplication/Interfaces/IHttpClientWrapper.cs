using System.Net.Http;
using System.Threading.Tasks;

namespace webApplication.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}