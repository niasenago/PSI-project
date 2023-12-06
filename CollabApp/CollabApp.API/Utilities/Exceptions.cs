
namespace CollabApp.API.Exceptions
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
    

    public class InvalidUserException : Exception
    {
        public InvalidUserException() : base("You are not logged in.") { }
    }
}