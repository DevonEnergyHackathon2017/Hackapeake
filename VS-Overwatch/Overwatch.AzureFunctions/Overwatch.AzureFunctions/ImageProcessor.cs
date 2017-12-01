using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Cognitive.CustomVision.Models;
using Microsoft.Cognitive.CustomVision;
using System.Threading;
using System;
using Newtonsoft.Json;
using Overwatch.Shared;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace Overwatch.AzureFunctions
{
    public static class ImageProcessor
    {
        [FunctionName("ImageProcessor")]
        public static void Run([BlobTrigger("trucks/{name}", Connection = "blobConnection")]Stream myBlob, string name, TraceWriter log)
        {
            Guid projectId = Guid.Parse("61f0cc4c-fb4f-4d62-95e1-2b2c631ae9b8");
            var imageUrl = $"https://hpkimagestorage.blob.core.windows.net/trucks/{name}";
            var propertyNumber = name.Substring(0, name.IndexOf("/"));
            var imagePredictions = ProcessImagePredictions(imageUrl,projectId);
            if (imagePredictions != null)
            {
                List<PropertyAlertStatu> alertStatuses = new List<PropertyAlertStatu>();
                using (var context = new OverwatchEntities())
                {
                    alertStatuses = context.PropertyAlertStatus.ToList();
                }
                string predictionAsJson = JsonConvert.SerializeObject(imagePredictions.Predictions);
                //check for type of alert
                //this is just getting the first tag that is over 90% but you would want to do more here.
                var tagInAlert = imagePredictions.Predictions.Where(x => x.Probability > .9).FirstOrDefault();
                var alertStatusId = 1;
                switch(tagInAlert.Tag)
                {
                    case "pickups" :
                        alertStatusId = alertStatuses.First(x => x.propertyAlertStatus == "Unrecognized pickup").id;
                        break;
                    case "trucks":
                        alertStatusId = alertStatuses.First(x => x.propertyAlertStatus == "Unrecognized semi").id;
                        break;
                    case "logos":
                        Guid chkProjectId = Guid.Parse("72550364-3d7a-46b7-a191-8369ae2749ba");
                        var logoResult = ProcessImagePredictions(imageUrl, chkProjectId);
                        if (logoResult != null)
                        {
                            alertStatusId = alertStatuses.First(x => x.propertyAlertStatus == "Recognized vehicle (Chesapeake)").id;
                        }
                        else
                        {
                            alertStatusId = alertStatuses.First(x => x.propertyAlertStatus == "Unmarked vehicle").id;
                        }
                        break;
                    default:
                        alertStatusId = alertStatuses.First(x => x.propertyAlertStatus == "Unmarked vehicle").id;
                        break;
                }

                var result = ProcessAlert(propertyNumber, imageUrl, predictionAsJson,alertStatusId);
            }
        }

        private static bool ProcessAlert(string propertyNumber, string imageUrl, string predictionAsJson, int alertStatusId)
        {
            var result = false;
            var connString = ConfigurationManager.AppSettings["OverwatchConnectionString"];
            using (var context = new OverwatchEntities(connString))
            {
                var property = context.Properties.Where(x => x.propertyNumber == propertyNumber).FirstOrDefault();
                if (property != null)
                {
                    PropertyAlert propertyAlert = new PropertyAlert()
                    {
                        imageUrl = imageUrl,
                        createTimestamp = DateTime.Now,
                        propertyAlertStatusId = alertStatusId,
                        propertyId = property.id,
                        predictionJson = predictionAsJson
                    };

                    context.PropertyAlerts.Add(propertyAlert);
                    context.SaveChanges();
                    result = true;
                }

            }
            return result;
        }

        private static ImagePredictionResultModel ProcessImagePredictions(string imageUrl, Guid projectId)
        {
            string predictionKey = "6c9f96aa9a574ab8b3aa18f5e0ec4c1c";
            var shouldAlert = false;

            // Create a prediction endpoint, passing in a prediction credentials object that contains the obtained prediction key
            PredictionEndpointCredentials predictionEndpointCredentials = new PredictionEndpointCredentials(predictionKey);
            PredictionEndpoint endpoint = new PredictionEndpoint(predictionEndpointCredentials);

            // Make a prediction against the new project
            var result = endpoint.PredictImageUrl(projectId, new Microsoft.Cognitive.CustomVision.Models.ImageUrl(imageUrl));
            // Need to throttle the requests as to not upset it into throwing the dreaded 429 too many requests in a given period of time
            Thread.Sleep(200);

            //check for any probabilities greater than 90%
            shouldAlert = result.Predictions.Any(x => x.Probability > .9);

            if (shouldAlert)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
