using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace CollabApp.Tests.UnitTests.Services
{
    public class PostComparerTests
    {
        [Fact]
        public void CompareOnlyDates_ReturnsCorrectComparisonResult()
        {
            // Arrange
            var comparer = new CompareOnlyDates();
            var earlierDate = new DateTime(2023, 1, 1);
            var laterDate = new DateTime(2023, 1, 2);

            var post1 = new Post { DatePosted = earlierDate };
            var post2 = new Post { DatePosted = laterDate };

            // Act
            var result = comparer.Compare(post1, post2);

            // Assert
            Assert.True(result < 0); // post1 should be considered "smaller" than post2
        }

        [Fact]
        public void CompareOnlyCommentAmount_ReturnsCorrectComparisonResult()
        {
            // Arrange
            var comparer = new CompareOnlyCommentAmount();

            var postWithFewerComments = new Post
            {
                Comments = new List<Comment>
                {
                    new Comment(1, "Comment 1", 1),
                    new Comment(2, "Comment 2", 1)
                }
            };

            var postWithMoreComments = new Post
            {
                Comments = new List<Comment>
                {
                    new Comment(1, "Comment 1", 1),
                    new Comment(2, "Comment 2", 1),
                    new Comment(3, "Comment 3", 1)
                }
            };

            // Act
            var result = comparer.Compare(postWithFewerComments, postWithMoreComments);

            // Assert
            Assert.True(result < 0); // postWithFewerComments should be considered "smaller" than postWithMoreComments
        }
    }
}
