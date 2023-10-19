using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using MCT.Functions.Models;
using Azure.Identity;

namespace MCT.Functions;
public class AddRegistration
{
    [FunctionName("PostRegistration")]
    public async Task<IActionResult> PostRegistration([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/registrations")] HttpRequest req, ILogger log)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try
            {
                var registrationData = JsonConvert.DeserializeObject<RegistrationData>(requestBody);
                if (registrationData.RegistrationId == Guid.Empty)
                    registrationData.RegistrationId = Guid.NewGuid();

                var credential = new DefaultAzureCredential();
                var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://database.windows.net/.default" }));

                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
                // Add the token to the SQL connection
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString);
                connection.AccessToken = token.Token;
                await connection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(
                    "INSERT INTO registrations (RegistrationId, LastName, FirstName, Email, Zipcode, Age, IsFirstTimer) " +
                    "VALUES (@RegistrationId, @LastName, @FirstName, @Email, @Zipcode, @Age, @IsFirstTimer)", connection);

                sqlCommand.Parameters.AddWithValue("@RegistrationId", registrationData.RegistrationId);
                sqlCommand.Parameters.AddWithValue("@LastName", registrationData.LastName);
                sqlCommand.Parameters.AddWithValue("@FirstName", registrationData.FirstName);
                sqlCommand.Parameters.AddWithValue("@Email", registrationData.Email);
                sqlCommand.Parameters.AddWithValue("@Zipcode", registrationData.Zipcode);
                sqlCommand.Parameters.AddWithValue("@Age", registrationData.Age);
                sqlCommand.Parameters.AddWithValue("@IsFirstTimer", registrationData.IsFirstTimer);

                await sqlCommand.ExecuteNonQueryAsync();

                return new OkObjectResult($"Registration data received and processed successfully.\nFollowing data inserted in database:\n - ID: {registrationData.RegistrationId}\n - Last name: {registrationData.LastName}\n - First name: {registrationData.FirstName}\n - Email: {registrationData.Email}\n - Zipcode: {registrationData.Zipcode}\n - Age: {registrationData.Age}\n - First time: {registrationData.IsFirstTimer}");
            }
            catch (JsonSerializationException)
            {
                return new BadRequestObjectResult("Invalid JSON format for registration data.");
            }
        }
        catch (Exception ex)
        {
            log.LogError($"An error occurred: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }
}