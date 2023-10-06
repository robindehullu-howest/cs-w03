using System;
namespace MCT.Functions.Models;

public class RegistrationData
{
    public Guid RegistrationId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string Zipcode { get; set; }
    public int Age { get; set; }
    public bool IsFirstTimer { get; set; }

    public RegistrationData(Guid registrationId, string lastName, string firstName, string email, string zipcode, int age, bool isFirstTimer)
    {
        RegistrationId = registrationId;
        LastName = lastName;
        FirstName = firstName;
        Email = email;
        Zipcode = zipcode;
        Age = age;
        IsFirstTimer = isFirstTimer;
    }

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
