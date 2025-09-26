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
        private string newTaskName;

        [ObservableProperty]
        private string newCategory;

        public MainViewModel()
        {
            _database = new ToDoDB();
            TodoItems = new ObservableCollection<ToDoItem>();
            LoadItemsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadItems()
        {
            var items = await _database.GetItems();
            if (SelectedCategory == "Все"){
                TodoItems = new ObservableCollection<ToDoItem>(items);
            }
            else
            {
                var filteredItems = items.Where(item => item.Category == SelectedCategory);
                TodoItems = new ObservableCollection<ToDoItem>(filteredItems);
            }
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            LoadItemsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task AddItem()
        {
            if ((!string.IsNullOrWhiteSpace(NewTaskName)) && (!string.IsNullOrWhiteSpace(NewCategory)))
            {
                var newItem = new ToDoItem { ItemName = NewTaskName, Category = NewCategory, IsCompleted = false };
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