using CollabApp.mvc.Models;

namespace CollabApp.mvc.Services
{
    public class CompareOnlyDates : IComparer<Post>
    {
        public int Compare(Post x, Post y)
        { 
            return x.DatePosted.CompareTo(y.DatePosted);
        }
    }

    public class CompareOnlyCommentAmount : IComparer<Post>
    {
        public int Compare(Post x, Post y)
        {
            return x.Comments.Count.CompareTo(y.Comments.Count);
        }
    }
}