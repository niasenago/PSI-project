using CollabApp.mvc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CollabApp.mvc.Services
{
    public class JsonRepository<T> : IDBAccess<T>
    {
        private string dbFilename { get; set; }
        private string dbPath { get; set; }
        private string fullDbPath { get; set; }

        public JsonRepository(string dbFilename, string dbPath)
        {
            this.dbFilename = dbFilename;
            this.dbPath = dbPath;
            this.fullDbPath = Path.Combine(dbPath, dbFilename);
        }
        public JsonRepository(string fullDbPath)
        {
            this.fullDbPath = fullDbPath;
        }
        //every time then this method is called it overwrite the entire JSON file with the new data 
        public void AddItem(T item)
        {
            List<T> items = GetAllItems();
            items.Add(item);

            string jsonString = JsonSerializer.Serialize(items);
            if (File.Exists(fullDbPath))
            {
                using(var streamWriter = new StreamWriter(fullDbPath))
                {
                    streamWriter.WriteLine(jsonString);
                } 
            }
            else
            {
                /* TODO: If the file does not exist */
            }
        }

        public T GetItemById(int id)
        {
            List<T> items = GetAllItems();
            foreach (var item in items)
            {
                 /*TODO: more ellegant way to identify items*/
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

            /*TODO: If no post with the given ID is found*/  
            return default(T); // Return default value for the type if item not found.
        }
        public void UpdateItemById(int id, T newItem)
        {
            List<T> items = GetAllItems();
            int indexToModify = -1;

            foreach (var item in items)
            {
                var itemIdProperty = item.GetType().GetProperty("Id");

                if (itemIdProperty != null)
                {
                    var itemIdValue = (int)itemIdProperty.GetValue(item);

                    if (itemIdValue == id)
                    {
                        indexToModify = items.IndexOf(item);
                        break; // Exit the loop once the item with the specified ID is found.
                    }
                }
            }

            if (indexToModify >= 0)
            {
                // Save the ID of the old item in the new item.
                var oldItem = items[indexToModify];
                var newItemIdProperty = newItem.GetType().GetProperty("Id");
                if (newItemIdProperty != null)
                {
                    newItemIdProperty.SetValue(newItem, (int)newItemIdProperty.GetValue(oldItem));
                }

                items[indexToModify] = newItem; // Replace the old item with the new item.
                string jsonString = JsonSerializer.Serialize(items);

                // Rewrite the entire JSON file with the updated data.
                if (File.Exists(fullDbPath))
                {
                    using(var streamWriter = new StreamWriter(fullDbPath))
                    {
                        streamWriter.WriteLine(jsonString);
                    } 
                }
            else
            {
                /* TODO: If the file does not exist */
            }
            }
            else
            {
                /* TODO: Handle the case where no item with the given ID is found */
                throw new InvalidOperationException("Item not found");
            }
        }




        public List<T> GetAllItems()
        {
            if (File.Exists(fullDbPath))
            {   

                List<T> items;
                using(var streamReader = new StreamReader(fullDbPath))
                {
                    string jsonString = streamReader.ReadToEnd();
                    items = JsonSerializer.Deserialize<List<T>>(jsonString);
                }
                
                return items ?? new List<T>();
            }
            else
            {
                 /* TODO: If the file does not exist */
                return new List<T>();
            }
        }

        
    }
}
