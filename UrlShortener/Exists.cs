using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace UrlShortener
{
    public static class Exists
    {
        [FunctionName("Existing")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "existing/{source}")]HttpRequestMessage req,
            string source,
            TraceWriter log)
        {
            if (await Store.Exists(source))
            {
                var response = req.CreateResponse(HttpStatusCode.Found);
                response.Headers.Location = await Store.GetDestination(source);
                return response;
            }

            return req.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
