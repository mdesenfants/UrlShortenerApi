using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace UrlShortener
{
    public static class Redirect
    {
        [FunctionName("Redirect")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{source}")]HttpRequestMessage req,
            string source,
            TraceWriter log)
        {
            if (!Store.IsValidSource(source))
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var location = await Store.GetDestination(source);

            if (location == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = req.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Location = location;

            return response;
        }
    }
}
