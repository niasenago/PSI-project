using System;
using System.Collections.Generic;

namespace CollabApp.mvc.Utilities
{
    public static class IdGenerator
    {
        private static readonly Random random = new Random();
        private static readonly HashSet<int> usedPostIds = new HashSet<int>();
        private static readonly HashSet<int> usedMessageIds = new HashSet<int>();
        private static readonly HashSet<int> usedUserIds = new HashSet<int>();

        public static int GenerateUniqueId(HashSet<int> usedIds)
        {
            int generatedId;
            do
            {
                generatedId = random.Next(1, int.MaxValue);
            } while (usedIds.Contains(generatedId));

            usedIds.Add(generatedId);
            return generatedId;
        }

        public static int GeneratePostId()
        {
            return GenerateUniqueId(usedPostIds);
        }

        public static int GenerateMessageId()
        {
            return GenerateUniqueId(usedMessageIds);
        }

        public static int GenerateUserId()
        {
            return GenerateUniqueId(usedUserIds);
        }
    }
}
