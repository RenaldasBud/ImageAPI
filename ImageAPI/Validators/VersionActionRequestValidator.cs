using ImageAPI.DTOs.Requests;

using System.ComponentModel.DataAnnotations;

namespace ImageAPI.Validators
{
    public class VersionActionRequestValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is VersionActionRequestDTO request)
            {
                if (request.ListId <= 0)
                {
                    return new ValidationResult("ListId must be greater than 0.");
                }

                if (string.IsNullOrWhiteSpace(request.Action))
                {
                    return new ValidationResult("Action is required.");
                }

                // Only allowing "newest" as a valid action for this example
                if (request.Action != "newest")
                {
                    return new ValidationResult("Invalid action. Supported actions: 'newest'.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
