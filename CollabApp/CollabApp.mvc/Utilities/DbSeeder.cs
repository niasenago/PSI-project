using CollabApp.mvc.Context;
using CollabApp.mvc.Models;

namespace CollabApp.mvc.Utilities
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }
        public void SeedSampleData()
        {
            // Sample posts
            var post1 = new Post
            {
                Title = "Becoming a C# Maestro",
                Author = "VisiemsGeraiZinomaFormule",
                Description = "Embark on a journey from C# novice to ninja as you master the art of C# development",
                DatePosted = DateTime.UtcNow
            };

            var post2 = new Post
            {
                Title = "What the duck is a kilometer",
                Author = "Alma",
                Description = "On the following day, May 30th, 2021, the meme was reposted on Reddit across multiple subreddits, going viral. For instance, on May 30th, Redditor smallppowner posted the meme to /r/shitposting,[2] gaining roughly 13,600 upvotes in two years. A similar repost was uploaded to /r/okbuddyretard [3] on May 31st, gaining roughly 8,800 upvotes in two years. The image was also reposted to Instagram[4] leading up to July 4th, 2021.",
                DatePosted = DateTime.UtcNow
            };

            var comment1 = new Comment("Alice", "Nice post", post1.Id);

            var comment2 = new Comment("Bob", "Great content!", post2.Id);
            var comment3 = new Comment("Ev", "Genius!", post2.Id);


            _context.Posts.Add(post1);
            _context.Posts.Add(post2);
            _context.Comments.Add(comment1);
            _context.Comments.Add(comment2);
            _context.Comments.Add(comment3);


            _context.SaveChanges();
        }
    } 
}