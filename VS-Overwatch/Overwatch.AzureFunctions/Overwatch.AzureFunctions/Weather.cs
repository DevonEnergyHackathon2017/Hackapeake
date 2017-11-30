using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Text;

namespace Overwatch.AzureFunctions
{
    public static class Weather
    {
        [FunctionName("Weather")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string latVal = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "lat", true) == 0)
                .Value;
            string longVal = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "long", true) == 0)
                .Value;

            double dblLat = Convert.ToDouble(latVal);
            double dblLong = Convert.ToDouble(longVal);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Overwatch");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpResponseMessage response = httpClient.GetAsync($"https://api.weather.gov/points/{dblLat},{dblLong}/forecast").Result;


            if (response.IsSuccessStatusCode)
            {
                dynamic weatherData = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);


            }

            
            return null;
        }
    }
}
