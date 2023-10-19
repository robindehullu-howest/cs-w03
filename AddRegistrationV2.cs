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
using Azure.Identity;

namespace MCT.Functions
{
    public class AddRegistrationV2
    {
        [FunctionName("AddRegistrationV2")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v2/registrations")] HttpRequest req, ILogger log)
        {
            // string connectionString = Environment.GetEnvironmentVariable("TableStorage");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newRegistration = JsonConvert.DeserializeObject<RegistrationData>(requestBody);

            //var tableClient = new TableClient(connectionString, "registrations");
            string partitionKey = "registrations";

            var tableClient = new TableClient(new Uri("https://stw04.table.core.windows.net/registrations"),partitionKey , new DefaultAzureCredential());

            Guid rowKeyGuid = Guid.NewGuid();
            string rowKey = rowKeyGuid.ToString();

            var newEntity = new TableEntity(partitionKey, rowKey)
            {
                {"age", newRegistration.Age},
                {"email", newRegistration.Email},
                {"firstname", newRegistration.FirstName},
                {"isfirsttimer", newRegistration.IsFirstTimer},
                {"lastname", newRegistration.LastName},
                {"zipcode", newRegistration.Zipcode},
                {"registrationid", rowKey}
            };

            await tableClient.AddEntityAsync(newEntity);

            newRegistration.RegistrationId = rowKeyGuid;
            return new OkObjectResult(newRegistration);
        }
    }
}
