
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Context;

namespace CollabApp.mvc.Validation
{
    public class UserValidator
    {
        public static void UserExists(ApplicationDbContext _context, int userId) // Change later
        {
            if (!_context.Users.Any(u => u.Id == userId))
                throw new InvalidUserException();


            // if (null == username)
            //     throw new InvalidUserException();
        }

        public static void UserExists(string username)
        {
            if (null == username)
                throw new InvalidUserException();
        }
    }
}