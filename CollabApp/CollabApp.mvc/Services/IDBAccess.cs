namespace CollabApp.mvc.Services {

    public interface IDBAccess<T>
    {
        /*interface with CRUD (Create, Read, Update, Delete) operations*/
        void AddItem(T item);
        T GetItemById(int id);
        List<T> GetAllItems();
        public void UpdateItemById(int id, T newItem);
        /*TODO delete operation*/
    }    
}    