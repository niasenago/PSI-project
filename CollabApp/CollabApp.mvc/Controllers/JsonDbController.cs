using CollabApp.mvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CollabApp.mvc.Controllers
{
    public class JsonDbController : IDBAccess
    {
        public int itemId { get; set; }
        private string dbFilename { get; set; }
        private string dbPath { get; set; }
        private string fullDbPath { get; set; }

        public JsonDbController(string dbFilename, string dbPath)
        {
            this.dbFilename = dbFilename;
            this.dbPath = dbPath;
            this.fullDbPath = Path.Combine(dbPath, dbFilename);
            InitializeItemId();
        }
        public JsonDbController(string fullDbPath)
        {
            this.fullDbPath = fullDbPath;
            InitializeItemId();
        }
        //every time then this method is called it overwrite the entire JSON file with the new data 
        public void AddPost(Post post) {
            post.Id = itemId;   //every time then post is created post id is set
            List<Post> posts = GetAllPosts();
            posts.Add(post);

            this.itemId++;      //then post id is incremented

            string jsonString = JsonSerializer.Serialize(posts);
            File.WriteAllText(fullDbPath, jsonString);
            
        }

        public Post GetPostById(int id) {
            List<Post> posts = GetAllPosts();
            foreach (var post in posts){
                if (post.Id == id){
                    return post;
                }
            }
            /*TO-DO If no post with the given ID is found*/  
            return null;
        }

        public List<Post> GetAllPosts() {
            if (File.Exists(fullDbPath)){
                string jsonString = File.ReadAllText(fullDbPath);
                List<Post> posts = JsonSerializer.Deserialize<List<Post>>(jsonString);
                return posts ?? new List<Post>(); //If posts variable is not null, it will return the posts.
            } else {
                /* TO-DO If the file does not exist */
                return null;
            }
        }

        /**TEMPORARILY*/
        private void InitializeItemId()
        {
            List<Post> posts = GetAllPosts();
            /*If there are posts, this part calculates the maximum Id value from the posts list and adds 1 to it.*/
            this.itemId = posts.Count > 0 ? posts.Max(p => p.Id) + 1 : 1;
            
        }

    }
}
