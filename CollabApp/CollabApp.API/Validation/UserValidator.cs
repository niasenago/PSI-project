
using CollabApp.API.Exceptions;
using CollabApp.API.Context;
using CollabApp.API.Repo;

namespace CollabApp.API.Validation
{
    public class UserValidator
    {
        public static async Task UserExists(IUnitOfWork _unitOfWork, int userId) // Change later
        {

            var user = await _unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
            {
                throw new InvalidUserException();
            }


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