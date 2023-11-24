
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.Tests.Validation
{
    public class ValidationHandlerTests
    {
        [Theory]
        [InlineData("My name is John. Fuck you.", true)]
        [InlineData("I am a very good boi.", false)]
        public void IsProfanityDetected_DetectsProfanity(string line, bool expectedResult)
        {
            bool result = ProfanityHandler.HasProfanity(line);

            Assert.Equal(result, expectedResult);
        }

        [Theory]
        [InlineData("My name is John. Fuck you.", "My name is John. **** you.")]
        [InlineData("I am a very good boi.", "I am a very good boi.")]
        public void CensorProfanities_CensorsProfanities(string line, string expectedLine)
        {
            string result = ProfanityHandler.CensorProfanities(line);

            Assert.Equal(result, expectedLine);
        }
    }
}