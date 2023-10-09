
namespace CollabApp.mvc.Validation
{
    public enum ValidationResult
    {
        Valid,
        EmptyField,
        Profanity
    }

    public class ValidatorError
    {
        public static string GetErrorMessage(ValidationResult result)
        {
            return result switch
            {
                ValidationResult.EmptyField => "Field cannot be empty.",
                ValidationResult.Profanity => "Profanity detected.",
                _ => "Valid",
            };
        }
    }

    public static class StringValidator {

        private static ValidationResult ValidateString(string input)
        {
            string trimmedInput = input.Trim();
            if(trimmedInput.Length == 0)
                return ValidationResult.EmptyField;

            if(ProfanityChecker.HasProfanity(trimmedInput))
                return ValidationResult.Profanity;

            return ValidationResult.Valid;
        }

        public static ValidationResult IsValidMessage(this string message)
        {
            return ValidateString(message);
        }

        public static ValidationResult IsValidGroupName(this string groupName)
        {
            return ValidateString(groupName);
        }

        public static ValidationResult IsValidTitle(this string title)
        {
            return ValidateString(title);
        }

        public static ValidationResult IsValidDescription(this string description)
        {
            return ValidateString(description);
        }
    }
}