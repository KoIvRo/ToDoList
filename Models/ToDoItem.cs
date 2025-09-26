using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ToDoList.Models
{
    public partial class ToDoItem : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ObservableProperty]
        private string _itemName;


        [ObservableProperty]
        private string _category;


        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}