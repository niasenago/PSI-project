using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Validation;
using Xunit;

namespace CollabApp.Tests.IntegrationTests.Validation
{
    public class StringValidationTests
    {
        [Fact]
        public void IsValidMessage_EmptyString_ThrowsEmptyFieldException()
        {
            Assert.Throws<EmptyFieldException>(() => StringValidator.IsValidMessage(""));
        }
        [Fact]
        public void IsValidUsername_ExceedMax_ThrowsMaxLengthExceededException()
        {
            var username = "123456789123456789";

            Assert.Throws<MaxLengthExceededException>(() => StringValidator.IsValidUsername(username));
        }
        [Fact]
        public void IsValidGroupName_Profanity_ThrowsProfanityException()
        {
            var groupName = "fuck";

            Assert.Throws<ProfanityException>(() => StringValidator.IsValidGroupName(groupName));
        }
    }
}
