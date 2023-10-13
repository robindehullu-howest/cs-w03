using System;
using Newtonsoft.Json;

namespace MCT.Functions.Models;

public class RegistrationData
{
    [JsonProperty("registrationId")]
    public Guid RegistrationId { get; set; }
    [JsonProperty("lastName")]
    public string LastName { get; set; }
    [JsonProperty("firstName")]
    public string FirstName { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("zipcode")]
    public string Zipcode { get; set; }
    [JsonProperty("age")]
    public int Age { get; set; }
    [JsonProperty("isFirstTimer")]
    public bool IsFirstTimer { get; set; }

    //public RegistrationData(Guid registrationId, string lastName, string firstName, string email, string zipcode, int age, bool isFirstTimer)
    //{
    //    RegistrationId = registrationId;
    //    LastName = lastName;
    //    FirstName = firstName;
    //    Email = email;
    //    Zipcode = zipcode;
    //    Age = age;
    //    IsFirstTimer = isFirstTimer;
    //}

    //public RegistrationData(string lastName, string firstName, string email, string zipcode, int age, bool isFirstTimer)
    //{
    //    RegistrationId = Guid.NewGuid();
    //    LastName = lastName;
    //    FirstName = firstName;
    //    Email = email;
    //    Zipcode = zipcode;
    //    Age = age;
    //    IsFirstTimer = isFirstTimer;
    //}
}
