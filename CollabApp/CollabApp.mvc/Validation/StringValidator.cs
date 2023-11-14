
namespace CollabApp.mvc.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class EmptyFieldException : ValidationException
    {
        public EmptyFieldException() : base("Field cannot be empty.") { }
    }

    public class MaxLengthExceededException : ValidationException
    {
        public MaxLengthExceededException(int maxLength)
            : base($"String exceeds maximum length of {maxLength} characters.") { }
    }

    public class InvalidCharException : ValidationException
    {
        public InvalidCharException() : base("Found invalid character.") { }
    }

    public class ProfanityException : ValidationException
    {
        public ProfanityException() : base("Profanities are not allowed.") { }
    }

    public enum MaxLengths 
    {
        Description = 256,
        Message = 500,
        Title = 30,
        Username = 15,
        GroupName = 30,
    }

    public static class StringValidator {

        private static void ValidateLength(string input, MaxLengths maxLength)
        {
            if(string.IsNullOrEmpty(input))
                throw new EmptyFieldException();

            string trimmedInput = input.Trim();
            if(trimmedInput.Length == 0)
                throw new EmptyFieldException();

            if(input.Length > (int)maxLength)
                throw new MaxLengthExceededException((int)maxLength);
        }

        public static void IsValidMessage(this string message)
        {
            ValidateLength(message, MaxLengths.Message);
        }

        public static void IsValidGroupName(this string groupName)
        {
            ValidateLength(groupName, MaxLengths.GroupName);
            if(ProfanityHandler.HasProfanity(groupName))
                throw new ProfanityException();
        }

        public static void IsValidTitle(this string title)
        {
            ValidateLength(title, MaxLengths.Title);
            if(ProfanityHandler.HasProfanity(title))
                throw new ProfanityException();
        }

        public static void IsValidDescription(this string description)
        {
            ValidateLength(description, MaxLengths.GroupName);
            if(ProfanityHandler.HasProfanity(description))
                throw new ProfanityException();
        }

        public static void IsValidUsername(this string username)
        {
            ValidateLength(username, MaxLengths.Username);
            if(ProfanityHandler.HasProfanity(username))
                throw new ProfanityException();
        }
    }
}