using System;

namespace CollabApp.mvc.Utilities
{
    public static class PostIdGenerator
    {
        private static readonly Random random = new Random();

        public static int GenerateRandomId()
        {
            // Generate a random integer as the ID.
            return random.Next(1, int.MaxValue);
        }
        /**TODO: check if generated id already exists */
    }
}