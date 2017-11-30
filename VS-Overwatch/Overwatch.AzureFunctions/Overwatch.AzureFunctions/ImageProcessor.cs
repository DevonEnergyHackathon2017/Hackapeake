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

namespace Overwatch.AzureFunctions
{
    public static class ImageProcessor
    {
        [FunctionName("ImageProcessor")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hpkimagestorage;AccountKey=7eG/6kr0HV1hYk9Inl6EzGvtMxo4M++zVNeoJmPOFdf7wch7UEZHO7dvP3g0LI4QlLUzWmK7fIgFvxPFYKlrTQ==;EndpointSuffix=core.windows.net";
            var containerName = "trucks";

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container, or it will create the container if it wasn't previously created.
            var container = blobClient.GetContainerReference(containerName);

            foreach (IListBlobItem blob in container.ListBlobs())
            {
                if (blob.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)blob;
                    foreach (IListBlobItem innerBlob in directory.ListBlobs())
                    {
                        if (innerBlob.GetType() != typeof(CloudBlobDirectory))
                        {
                            var propertyNumber = innerBlob.Uri.Segments[2].Replace("/", "");
                            var imageUrl = innerBlob.Uri;
                            var imagePredictions = ProcessImagePredictions(imageUrl.ToString());
                            if (imagePredictions != null)
                            {
                                string predictionAsJson = JsonConvert.SerializeObject(imagePredictions.Predictions);
                                var result = ProcessAlert(propertyNumber, imageUrl, predictionAsJson);
                            }
                        }
                    }
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, "Successfully processed images");
        }

        private static bool ProcessAlert(string propertyNumber, Uri imageUrl, string predictionAsJson)
        {
            var result = false;
            using (var context = new OverwatchEntities())
            {
                var property = context.Properties.Where(x => x.propertyNumber == propertyNumber).FirstOrDefault();
                if (property != null)
                {
                    PropertyAlert propertyAlert = new PropertyAlert()
                    {
                        imageUrl = imageUrl.ToString(),
                        createTimestamp = DateTime.Now,
                        propertyAlertStatusId = 1,
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

        private static ImagePredictionResultModel ProcessImagePredictions(string imageUrl)
        {
            string predictionKey = "6c9f96aa9a574ab8b3aa18f5e0ec4c1c";
            Guid projectId = Guid.Parse("2f3ab445-617c-42c6-a5b6-d73009d8db56");
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
