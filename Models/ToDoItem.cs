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

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private bool _isOverDue;

        [ObservableProperty]
        private TimeSpan _endTime;

        [ObservableProperty]
        private DateTime? _endDate;
    }
}