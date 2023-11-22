
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.Tests.Validation
{
    public class StringValidatorTests
    {
        [Theory]
        [InlineData(null, typeof(EmptyFieldException))]
        [InlineData("  ", typeof(EmptyFieldException))]
        [InlineData(
            "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 
            typeof(MaxLengthExceededException)
        )]
        public void IsValidMessage_ThrowsCorrectException(string line, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => line.IsValidMessage());
        }

        [Fact]
        public void IsValidMessage_ThrowsNoException()
        {
            string line = "Hello";
            var exception = Record.Exception(() => line.IsValidMessage());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, typeof(EmptyFieldException))]
        [InlineData("   ", typeof(EmptyFieldException))]
        [InlineData("1234567890123456789012345678901234567890", typeof(MaxLengthExceededException))]
        [InlineData("Fuck", typeof(ProfanityException))]
        public void IsValidGroupName_ThrowsCorrectException(string line, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => line.IsValidGroupName());
        }

        [Fact]
        public void IsValidGroupName_ThrowsNoException()
        {
            string line = "SuperGroup9000";
            var exception = Record.Exception(() => line.IsValidGroupName());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, typeof(EmptyFieldException))]
        [InlineData("   ", typeof(EmptyFieldException))]
        [InlineData("1234567890123456789012345678901234567890", typeof(MaxLengthExceededException))]
        [InlineData("Bitch", typeof(ProfanityException))]
        public void IsValidTitle_ThrowsCorrectException(string line, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => line.IsValidTitle());
        }

        [Fact]
        public void IsValidTitle_ThrowsNoException()
        {
            string line = "This is a message!";
            var exception = Record.Exception(() => line.IsValidTitle());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, typeof(EmptyFieldException))]
        [InlineData("  ", typeof(EmptyFieldException))]
        [InlineData("Bitch", typeof(ProfanityException))]
        [InlineData(
            "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 
            typeof(MaxLengthExceededException)
        )]
        public void IsValidDescription_ThrowsCorrectException(string line, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => line.IsValidDescription());
        }

        [Fact]
        public void IsValidDescription_ThrowsNoException()
        {
            string line = "Once upon a time, in a quaint little village nestled between rolling hills and lush forests, there lived a young inventor named Oliver.";
            var exception = Record.Exception(() => line.IsValidDescription());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, typeof(EmptyFieldException))]
        [InlineData("  ", typeof(EmptyFieldException))]
        [InlineData("J Fuck", typeof(ProfanityException))]
        [InlineData(
            "123456789012345678901234567890123456789012345678901234567890", 
            typeof(MaxLengthExceededException)
        )]
        public void IsValidUsername_ThrowsCorrectException(string line, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => line.IsValidUsername());
        }

        [Fact]
        public void IsValidUsername_ThrowsNoException()
        {
            string line = "RobloxDestroyer";
            var exception = Record.Exception(() => line.IsValidUsername());
            Assert.Null(exception);
        }
    }
}