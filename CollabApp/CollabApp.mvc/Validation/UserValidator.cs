
using CollabApp.mvc.Exceptions;

namespace CollabApp.mvc.Validation
{
    public class UserValidator
    {
        public static void UserExists(string username) // Change later
        {
            if (null == username)
                throw new InvalidUserException();
        }
    }
}