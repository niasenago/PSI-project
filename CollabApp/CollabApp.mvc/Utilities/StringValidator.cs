
namespace CollabApp.mvc.Utilities
{
    // TODO: Add maximum length constraints
    public static class StringValidator {
        public static bool IsValidMessage(this string message)
        {
            return message.Trim().Length != 0;
        }

        public static bool IsValidGroupName(this string groupName)
        {
            return groupName.Trim().Length != 0;
        }

        public static bool IsValidTitle(this string title)
        {
            return title.Trim().Length != 0;
        }

        public static bool IsValidDescription(this string description)
        {
            return true;
        }
    }
}