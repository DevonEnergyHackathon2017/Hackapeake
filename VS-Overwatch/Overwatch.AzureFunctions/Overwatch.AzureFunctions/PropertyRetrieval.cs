using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Overwatch.Shared;
using System.Collections.Generic;
using System.Configuration;
using System;

namespace Overwatch.AzureFunctions
{
    public static class PropertyRetrieval
    {
        [FunctionName("PropertyRetrieval")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var connString = ConfigurationManager.AppSettings["OverwatchConnectionString"];
            KeyValuePair<string, string> propertyId = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "propertyId", true) == 0);
            
            using (var context = new OverwatchEntities(connString))
            {
                var query = context.Properties.Select(n => new
                {
                    id = n.id,
                    propertyNumber = n.propertyNumber,
                    propertyName = n.propertyName,
                    sourceName = n.sourceName,
                    lattitude = n.latitude,
                    longitude = n.longitude,
                    approximateNumOfPeople = n.approximateNumOfPeople,
                    propertyStatus = n.PropertyStatus.propertyStatus,
                    latestPropertyAlert = (n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).FirstOrDefault() == null) ? null : new
                    {
                        propertyAlert = n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).FirstOrDefault().PropertyAlertStatus.propertyAlertStatus,
                        timestamp = n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).FirstOrDefault().createTimestamp,
                        imageUrl = n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).FirstOrDefault().imageUrl,
                        severity = n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).FirstOrDefault().PropertyAlertStatus.Severity.severity1
                    },
                    latestFivePropertyAlerts = (n.PropertyAlerts.OrderByDescending(x => x.createTimestamp).Skip(1).Take(5).Select(x => new
                    {
                        propertyAlert = x.PropertyAlertStatus.propertyAlertStatus,
                        timestamp = x.createTimestamp,
                        imageUrl = x.imageUrl,
                        severity = x.PropertyAlertStatus.Severity.severity1
                    }))
                });

                if (propertyId.Value == null)
                {       
                    return req.CreateResponse(query.ToList());
                }
                else
                {
                    int castedId = Convert.ToInt32(propertyId.Value);
                    return req.CreateResponse(query.Where(x => castedId == x.id).FirstOrDefault());
                }
            }
                
        }
    }
}
