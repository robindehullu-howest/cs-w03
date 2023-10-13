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
using System.Security.Cryptography;

namespace MCT.Functions;

public class GetRegistration
{
    [FunctionName("GetRegistrations")]
    public async Task<IActionResult> GetRegistrations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/registrations")] HttpRequest req, ILogger log)
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
                    RegistrationData registration = new RegistrationData() {
                        LastName = sqlDataReader["LastName"].ToString(),
                        FirstName = sqlDataReader["FirstName"].ToString(),
                        Email = sqlDataReader["Email"].ToString(),
                        Age = Convert.ToInt32(sqlDataReader["Age"]),
                        IsFirstTimer = Convert.ToBoolean(sqlDataReader["IsFirstTimer"]),
                        Zipcode = sqlDataReader["Zipcode"].ToString()
                    };

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