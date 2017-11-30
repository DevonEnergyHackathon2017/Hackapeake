using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;

namespace Overwatch.AzureFunctions
{
    public static class ImagePersister
    {
        [FunctionName("ImagePersister")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                // parse query parameter
                string imageUrl = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "imageUrl", true) == 0)
                    .Value;

                string propertyNumber = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "propertyNumber", true) == 0)
                    .Value;
                //  Initialize stuff.

                //  Validate authenticated user & privileges.  

                //  Get the content stream of the request and 
                //  save it in the BLOB storage.


                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
