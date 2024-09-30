using ImageAPI.DTOs.Requests;

using System.ComponentModel.DataAnnotations;

namespace ImageAPI.Validators
{
    public class StoreDimensionsRequestValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is StoreDimensionsRequestDTO request)
            {
                if (request.ListId <= 0)
                {
                    return new ValidationResult("ListId must be greater than 0.");
                }

                if (request.VersionId <= 0)
                {
                    return new ValidationResult("VersionId must be greater than 0.");
                }

                if (request.Width <= 0)
                {
                    return new ValidationResult("Width must be greater than 0.");
                }

                if (request.Height <= 0)
                {
                    return new ValidationResult("Height must be greater than 0.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
