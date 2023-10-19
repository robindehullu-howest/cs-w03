using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using MCT.Functions.Models;
using System.Collections.Generic;
using System.IO.Enumeration;
using Azure.Storage.Blobs;

namespace MCT.Functions
{
    public class Export
    {
        [FunctionName("Export")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/registrations/export")] HttpRequest req, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("TableStorage");
            string partitionKey = "registrations";

            var tableClient = new TableClient(connectionString, partitionKey);
            Pageable<TableEntity> queryRegistrations = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");

            var returnValue = new List<RegistrationData>();

            foreach (var registration in queryRegistrations)
            {
                returnValue.Add(new RegistrationData()
                {
                    Age = int.Parse(registration["age"].ToString()),
                    Email = registration["email"].ToString(),
                    LastName = registration["lastname"].ToString(),
                    FirstName = registration["firstname"].ToString(),
                    IsFirstTimer = bool.Parse(registration["isfirsttimer"].ToString()),
                    Zipcode = registration["zipcode"].ToString()
                });
            }
            var fileName = WriteCSV(returnValue);
            Upload(fileName);


            return new OkObjectResult("");
        }

        private void Upload(string fileName)
        {
            string connectionString = Environment.GetEnvironmentVariable("TableStorage");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            string containerName = "csv";
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobContainerClient.CreateIfNotExists();

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            blobClient.Upload(fileName);
        }

        private string WriteCSV(List<RegistrationData> registrations)
        {
            string csv = "RegistrationId,FirstName,LastName,Age,Email,IsFirstTimer,Zipcode\n";

            foreach (var registration in registrations)
            {
                csv += $"{registration.RegistrationId},{registration.FirstName},{registration.LastName},{registration.Age},{registration.Email},{registration.IsFirstTimer},{registration.Zipcode}\n";
            }

            string fileName = $"registrations-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            File.WriteAllText(fileName, csv);

            return fileName;
        }
    }
}
