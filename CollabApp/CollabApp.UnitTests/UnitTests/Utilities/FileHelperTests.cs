using System;
using CollabApp.mvc.Utilities;

namespace CollabApp.Tests.UnitTests.Utilities
{
    public class FileHelperTests
    {
        [Fact]
        public void IsPdfFile_WhenFileTypeIsPdf_ReturnsTrue()
        {
            // Arrange
            var fileType = "PDF Document";

            // Act
            var result = fileType.IsPdfFile();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPdfFile_WhenFileTypeIsNotPdf_ReturnsFalse()
        {
            // Arrange
            var fileType = "Not PDF";

            // Act
            var result = fileType.IsPdfFile();

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("JPEG Image")]
        [InlineData("PNG Image")]
        public void IsImageFile_WhenFileTypeIsImage_ReturnsTrue(string fileType)
        {
            // Act
            var result = fileType.IsImageFile();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsImageFile_WhenFileTypeIsNotImage_ReturnsFalse()
        {
            // Arrange
            var fileType = "PDF Document";

            // Act
            var result = fileType.IsImageFile();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAudioFile_WhenFileTypeIsAudio_ReturnsTrue()
        {
            // Arrange
            var fileType = "MP3 Audio";

            // Act
            var result = fileType.IsAudioFile();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAudioFile_WhenFileTypeIsNotAudio_ReturnsFalse()
        {
            // Arrange
            var fileType = "PDF Document";

            // Act
            var result = fileType.IsAudioFile();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsVideoFile_WhenFileTypeIsVideo_ReturnsTrue()
        {
            // Arrange
            var fileType = "MP4 Video";

            // Act
            var result = fileType.IsVideoFile();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsVideoFile_WhenFileTypeIsNotVideo_ReturnsFalse()
        {
            // Arrange
            var fileType = "PDF Document";

            // Act
            var result = fileType.IsVideoFile();

            // Assert
            Assert.False(result);
        }
    }
}
