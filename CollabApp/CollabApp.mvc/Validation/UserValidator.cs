
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Validation
{
    public class UserValidator
    {
        public static async void UserExists(IUnitOfWork _unitOfWork, int userId) // Change later
        {

            var user = await _unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
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