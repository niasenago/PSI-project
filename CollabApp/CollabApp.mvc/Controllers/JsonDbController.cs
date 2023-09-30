using CollabApp.mvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CollabApp.mvc.Controllers
{
    public class JsonDbController<T> : IDBAccess<T>
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
        public void AddItem(T item)
        {
            List<T> items = GetAllItems();
            items.Add(item);

            this.itemId++;

            string jsonString = JsonSerializer.Serialize(items);
            File.WriteAllText(fullDbPath, jsonString);
        }

        public T GetItemById(int id)
        {
            List<T> items = GetAllItems();
            foreach (var item in items)
            {
                // You'll need a way to identify items by ID; you might want to use interfaces or base classes.
                // For the example, I'm assuming a property called 'Id'.
                var itemIdProperty = item.GetType().GetProperty("Id");
                if (itemIdProperty != null)
                {
                    var itemIdValue = (int)itemIdProperty.GetValue(item);
                    if (itemIdValue == id)
                    {
                        return item;
                    }
                }
            }
            return default(T); // Return default value for the type if item not found.
        }

        public List<T> GetAllItems()
        {
            if (File.Exists(fullDbPath))
            {
                string jsonString = File.ReadAllText(fullDbPath);
                List<T> items = JsonSerializer.Deserialize<List<T>>(jsonString);
                return items ?? new List<T>();
            }
            else
            {
                return new List<T>();
            }
        }

        /**TEMPORARILY*/
        private void InitializeItemId()
        {
            List<T> items = GetAllItems();
            // You'll need a way to identify items by ID; you might want to use interfaces or base classes.
            // For the example, I'm assuming a property called 'Id'.
            var itemIdProperty = typeof(T).GetProperty("Id");
            this.itemId = items.Count > 0 && itemIdProperty != null
                ? items.Max(item => (int)itemIdProperty.GetValue(item)) + 1
                : 1;
        }

    }
}
