using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ToDoDB _database;

        public List<string> Categories { get; } = new()
        {
            "Дом",
            "Учеба",
            "Работа",
            "Личное",
            "Другое"
        };

        public List<string> FilterCategories { get; } = new()
        {
            "Все",
            "Дом",
            "Учеба",
            "Работа",
            "Личное",
            "Другое"
        };


        [ObservableProperty]
        private string selectedCategory = "Все";

        [ObservableProperty]
        private ObservableCollection<ToDoItem> todoItems;

        [ObservableProperty]
        private ObservableCollection<ToDoItem> doneItems;

        [ObservableProperty]
        private string newTaskName;

        [ObservableProperty]
        private string newCategory = "Личное";

        [ObservableProperty]
        private TimeSpan newEndTime;

        [ObservableProperty]
        private DateTime? newEndDate;

        public MainViewModel()
        {
            _database = new ToDoDB();
            TodoItems = new ObservableCollection<ToDoItem>();
            DoneItems = new ObservableCollection<ToDoItem>();
            LoadItemsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadItems()
        {
            var items = await _database.GetItems();
            if (SelectedCategory == "Все"){
                TodoItems = new ObservableCollection<ToDoItem>(items.Where(item => item.IsCompleted == false));
                DoneItems = new ObservableCollection<ToDoItem>(items.Where(item => item.IsCompleted == true));
            }
            else
            {
                var filteredItems = items.Where(item => item.Category == SelectedCategory);
                TodoItems = new ObservableCollection<ToDoItem>(filteredItems.Where(item => item.IsCompleted == false));
                DoneItems = new ObservableCollection<ToDoItem>(filteredItems.Where(item => item.IsCompleted == true));
            }
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            LoadItemsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task ToggleComplete(ToDoItem item)
        {
            if (item != null)
            {
                item.IsCompleted = !item.IsCompleted;

                await _database.SaveItem(item);

                LoadItemsCommand.Execute(null);
            }
        }


        [RelayCommand]
        private async Task AddItem()
        {
            if ((!string.IsNullOrWhiteSpace(NewTaskName)) && (!string.IsNullOrWhiteSpace(NewCategory)))
            {
                var newItem = new ToDoItem { ItemName = NewTaskName, Category = NewCategory, EndTime = NewEndTime, EndDate = NewEndDate, IsCompleted = false };
                await _database.SaveItem(newItem);

                NewTaskName = string.Empty;
                NewCategory = string.Empty;
                await LoadItemsCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private async Task DeleteItem(ToDoItem item)
        {
            if (item != null)
            {
                await _database.DeleteItem(item);
                await LoadItemsCommand.ExecuteAsync(null);
            }
        }
    }
}