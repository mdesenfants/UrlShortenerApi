using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace UrlShortener
{
    public static class Save
    {
        [FunctionName("Save")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "{source}")]HttpRequestMessage req,
            string source,
            TraceWriter log)
        {
            var body = await req.Content.ReadAsStringAsync();

            Uri destination;
            try
            {
                destination = JsonConvert.DeserializeObject<Uri>(body);
            }
            catch
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }


            if (await Store.Exists(source))
            {
                return req.CreateResponse(HttpStatusCode.Conflict);
            }

            if (Store.IsValidSource(source))
            {
                await Store.SaveRoute(source, destination);
                var response = req.CreateResponse(HttpStatusCode.Created);
                response.Headers.Location = new Uri(source, UriKind.Relative);
                return response;
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
