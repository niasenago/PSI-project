using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<Post>Posts {get;set;}
        public virtual DbSet<Comment>Comments{get;set;}
        public virtual DbSet<User>Users{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // connect post with comments  1 - Many relationship
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Comment>(entity => {    //for a Comment entity
                entity.HasOne(p => p.Post)              //for every comment it has one post
                    .WithMany(c => c.Comments)          //post has many comments
                    .HasForeignKey(x => x.PostId)       //specify under which id it needs to connect post and comments
                    .OnDelete(DeleteBehavior.Restrict)  //not allowed to delete if there is a connection
                    .HasConstraintName("FK_Comment_Post");
            });
        }
    }
}