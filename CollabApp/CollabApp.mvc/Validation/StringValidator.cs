
namespace CollabApp.mvc.Validation
{
    public enum ValidationResult
    {
        Valid,
        EmptyField,
        MaxLength,
        InvalidChar,
        Profanity
    }

    public enum MaxLengths 
    {
        Description = 256,
        Message = 500,
        Title = 30,
        Username = 15,
        GroupName = 30,
    }

    public class ValidationError {
        public ValidationResult ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationError(ValidationResult errorCode, string? errorMessage = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = string.IsNullOrEmpty(errorMessage) ? GenericErrorMessage(errorCode) : errorMessage;
        }
        
        private static string GenericErrorMessage(ValidationResult result)
        {
            return result switch
            {
                ValidationResult.EmptyField => "Field cannot be empty.",
                ValidationResult.InvalidChar => "Found invalid character.",
                ValidationResult.MaxLength => "String exceeds maximum length.",
                ValidationResult.Profanity => "Profanities are not allowed.",
                _ => "Valid",
            };
        }

        public bool HasError()
        {
            return ErrorCode != ValidationResult.Valid;
        }
    }

    public static class StringValidator {

        private static ValidationError ValidateLength(string input, MaxLengths maxLength)
        {
            if(string.IsNullOrEmpty(input))
                return new ValidationError(ValidationResult.EmptyField);
            // For Javascript
            string trimmedInput = input.Trim();
            if(trimmedInput.Length == 0)
                return new ValidationError(ValidationResult.EmptyField);

            if(input.Length > (int)maxLength)
                return new ValidationError(ValidationResult.MaxLength, "String exceeds " + (int)maxLength + " characters.");

            return new ValidationError(ValidationResult.Valid);
        }

        public static ValidationError IsValidMessage(this string message)
        {
            ValidationError result = ValidateLength(message, MaxLengths.Message);

            return result;
        }

        public static ValidationError IsValidGroupName(this string groupName)
        {
            ValidationError result = ValidateLength(groupName, MaxLengths.GroupName);
            if(result.HasError())
                return result;

            if(ProfanityHandler.HasProfanity(groupName))
                result = new ValidationError(ValidationResult.Profanity);

            return result;
        }

        public static ValidationError IsValidTitle(this string title)
        {
            ValidationError result = ValidateLength(title, MaxLengths.Title);
            if(result.HasError())
                return result;

            if(ProfanityHandler.HasProfanity(title))
                result = new ValidationError(ValidationResult.Profanity);

            return result;
        }

        public static ValidationError IsValidDescription(this string description)
        {
            ValidationError result = ValidateLength(description, MaxLengths.Description);

            return result;
        }

        public static ValidationError IsValidUsername(this string username)
        {
            ValidationError result = ValidateLength(username, MaxLengths.Username);
            if(result.HasError())
                return result;

            if(ProfanityHandler.HasProfanity(username))
                result = new ValidationError(ValidationResult.Profanity);

            return result;
        }
    }
}