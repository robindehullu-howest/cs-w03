using System;
namespace MCT.Functions.Models;

public record RegistrationData(Guid RegistrationId, string LastName, string FirstName, string Email, string Zipcode, int Age, bool IsFirstTimer);
