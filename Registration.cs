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

namespace MCT.Functions;
public  class Registration
{
[FunctionName("PostRegistration")]
public async Task<IActionResult> PostRegistration([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/registrations")] HttpRequest req, ILogger log)
{
    try
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var registrationData = JsonConvert.DeserializeObject<RegistrationData>(requestBody);

        string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            await sqlConnection.OpenAsync();

            SqlCommand sqlCommand = new SqlCommand(
                "INSERT INTO registrations (RegistrationId, LastName, FirstName, Email, Zipcode, Age, IsFirstTimer) " +
                "VALUES (@RegistrationId, @LastName, @FirstName, @Email, @Zipcode, @Age, @IsFirstTimer)", sqlConnection);

            sqlCommand.Parameters.AddWithValue("@RegistrationId", registrationData.RegistrationId);
            sqlCommand.Parameters.AddWithValue("@LastName", registrationData.LastName);
            sqlCommand.Parameters.AddWithValue("@FirstName", registrationData.FirstName);
            sqlCommand.Parameters.AddWithValue("@Email", registrationData.Email);
            sqlCommand.Parameters.AddWithValue("@Zipcode", registrationData.Zipcode);
            sqlCommand.Parameters.AddWithValue("@Age", registrationData.Age);
            sqlCommand.Parameters.AddWithValue("@IsFirstTimer", registrationData.IsFirstTimer);

            await sqlCommand.ExecuteNonQueryAsync();
        }

        return new OkObjectResult("Registration data received and saved successfully.");
    }
    catch (Exception ex)
    {
        log.LogError($"An error occurred: {ex.Message}");
        return new StatusCodeResult(500);
    }
}


    [FunctionName("GetRegistrations")]
    public async Task<IActionResult> GetRegistrations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/registrations")] HttpRequest req,ILogger log)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM registrations", sqlConnection);
                SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

                List<RegistrationData> registrations = new List<RegistrationData>();

                while (sqlDataReader.Read())
                {
                    RegistrationData registration = new RegistrationData(
                        Guid.Parse(sqlDataReader["RegistrationId"].ToString()),
                        sqlDataReader["LastName"].ToString(),
                        sqlDataReader["FirstName"].ToString(),
                        sqlDataReader["Email"].ToString(),
                        sqlDataReader["Zipcode"].ToString(),
                        Convert.ToInt32(sqlDataReader["Age"]),
                        Convert.ToBoolean(sqlDataReader["IsFirstTimer"])
                    );

                    registrations.Add(registration);
                }

                return new OkObjectResult(registrations);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"An error occurred: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }
}
