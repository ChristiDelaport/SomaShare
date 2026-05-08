using System.ComponentModel.DataAnnotations;

namespace SomaShare.Services
{

    public class FormValidator
    {
        private readonly List<string> errors = new();

        public List<string> Errors => errors;

        public bool HasErrors => errors.Count > 0;

        public void Clear()
        {
            errors.Clear();
        }

        public void AddError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && !errors.Contains(message))
            {
                errors.Add(message);
            }
        }
        public bool ValidateRequired(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError($"{fieldName} is required");
                return false;
            }
            return true;
        }
        public bool ValidateEmail(string? email, string fieldName = "Email")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                AddError($"{fieldName} is required");
                return false;
            }

            if (!IsValidEmail(email))
            {
                AddError($"{fieldName} format is invalid");
                return false;
            }

            return true;
        }

        public bool ValidateMinLength(string? value, int minLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError($"{fieldName} is required");
                return false;
            }

            if (value.Length < minLength)
            {
                AddError($"{fieldName} must be at least {minLength} characters");
                return false;
            }

            return true;
        }

        public bool ValidateMaxLength(string? value, int maxLength, string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
            {
                AddError($"{fieldName} cannot exceed {maxLength} characters");
                return false;
            }

            return true;
        }

        public bool ValidateRange(decimal value, decimal min, decimal max, string fieldName)
        {
            if (value < min || value > max)
            {
                AddError($"{fieldName} must be between {min} and {max}");
                return false;
            }

            return true;
        }

        public bool ValidateNumeric(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError($"{fieldName} is required");
                return false;
            }

            if (!decimal.TryParse(value, out _))
            {
                AddError($"{fieldName} must be a valid number");
                return false;
            }

            return true;
        }
        public bool ValidateInteger(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError($"{fieldName} is required");
                return false;
            }

            if (!int.TryParse(value, out _))
            {
                AddError($"{fieldName} must be a valid whole number");
                return false;
            }

            return true;
        }

        public bool ValidateUrl(string? url, string fieldName = "URL")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return true; 
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                AddError($"{fieldName} format is invalid");
                return false;
            }

            return true;
        }

        public bool ValidateMatch(string? value1, string? value2, string fieldName)
        {
            if (value1 != value2)
            {
                AddError($"{fieldName} values do not match");
                return false;
            }

            return true;
        }

        public bool ValidateObject(object obj)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, results, validateAllProperties: true))
            {
                foreach (var error in results)
                {
                    AddError(error.ErrorMessage ?? "Validation failed");
                }
                return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string? GetErrorForField(string fieldName)
        {
            return errors.FirstOrDefault(e => e.Contains(fieldName));
        }

        public void RemoveFieldError(string fieldName)
        {
            var errorToRemove = errors.FirstOrDefault(e => e.Contains(fieldName));
            if (errorToRemove != null)
            {
                errors.Remove(errorToRemove);
            }
        }
    }
    public static class FormValidatorExtensions
    {
        public static string GetErrorClass(this FormValidator validator, string fieldName)
        {
            return validator.GetErrorForField(fieldName) != null ? "is-invalid" : "";
        }

        public static string GetErrorMessage(this FormValidator validator, string fieldName)
        {
            return validator.GetErrorForField(fieldName) ?? "";
        }
    }
}
