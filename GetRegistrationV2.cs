using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MCT.Functions.Models;
using Azure.Data.Tables;
using Azure;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MCT.Functions
{
    public class GetRegistrationV2
    {
        [FunctionName("GetRegistrationV2")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/registrations")] HttpRequest req, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("TableStorage");
            string partitionKey = "registrations";

            var tableClient = new TableClient(connectionString, partitionKey);
            Pageable<TableEntity> queryRegistrations = tableClient.Query<TableEntity>(filter: $"PartitionKey eq {partitionKey}");

            var returnValue = new List<RegistrationData>();

            foreach(var registration in queryRegistrations) {
                returnValue.Add(new RegistrationData(){
                    Age = int.Parse(registration["age"].ToString()),
                    Email = registration["email"].ToString(),
                    LastName = registration["lastname"].ToString(),
                    FirstName = registration["firstname"].ToString(),
                    IsFirstTimer = bool.Parse(registration["isfirsttimer"].ToString()),
                    Zipcode = registration["zipcode"].ToString()
                });
            }

            return new OkObjectResult(returnValue);
        }
    }
}
