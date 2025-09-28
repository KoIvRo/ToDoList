using SQLite;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ToDoDB
    {
        private readonly SQLiteAsyncConnection _connection;

        public ToDoDB()
        {
            string db_path = Path.Combine(FileSystem.AppDataDirectory, "ToDo.db");
            _connection = new SQLiteAsyncConnection(db_path);
            _connection.CreateTableAsync<ToDoItem>().Wait();
        }

        public Task<List<ToDoItem>> GetItems()
        {
            return _connection.Table<ToDoItem>().ToListAsync();
        }

        public async Task SaveItem(ToDoItem item)
        {
            if (item.Id == 0)
            {
                await _connection.InsertAsync(item);
            }
            else
            {
                await _connection.UpdateAsync(item);
            }
        }

        public Task DeleteItem(ToDoItem item)
        {
            return _connection.DeleteAsync(item);
        }
    }
}