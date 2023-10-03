namespace CollabApp.mvc.Controllers {

    public interface IDBAccess<T>
    {
        void AddItem(T item);
        T GetItemById(int id);
        List<T> GetAllItems();
        public void UpdateItemById(int id, T newItem);
    }    
}    