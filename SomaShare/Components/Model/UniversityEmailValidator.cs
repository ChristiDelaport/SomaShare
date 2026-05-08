using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    // custom validation attribute to check if an email belongs to an allowed uni domain
    public class UniversityEmailValidator : ValidationAttribute
    {
        // list of domains that are allowed by the system
        private static readonly string[] AllowedDomains = new[] //The domains allowed by the system
        {
            "stadio.ac.za",    
            "UTC.ac.za",
            "UWC.ac.za",
            "Highschool.com",
            "SomaSchool.com",
            "SomaAfrika.ac.za",
            "Ubiquity.ac.za",
            "Fairmont.ac.za",
            "otherunis.com",       
        };

        // method that checks if the provided value is valid
        public override bool IsValid(object? value) //checking if Valid
        {
            if (value is not string email) //if value isnt a string
                return false;

            if (string.IsNullOrWhiteSpace(email)) //If empty
                return false;

            // extract the domain part of the email (after '@') and convert to lowercase
            var domain = email.Split('@').LastOrDefault()?.ToLower(); 
            //check is extracted domain matches allowed domain
            return AllowedDomains.Any(d => d.Equals(domain, StringComparison.OrdinalIgnoreCase));
        }

        //custom error msg if validation fails
        public override string FormatErrorMessage(string name) 
        {
            var domains = string.Join(", ", AllowedDomains);
            return $"Email must be from an approved university domain: {domains}";
        }
    }
}
