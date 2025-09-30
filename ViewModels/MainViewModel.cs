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
        private ObservableCollection<ToDoItem> overDueItems;

        [ObservableProperty]
        private string newTaskName;

        [ObservableProperty]
        private string newCategory = "Личное";

        [ObservableProperty]
        private TimeSpan newEndTime;

        [ObservableProperty]
        private DateTime? newEndDate = DateTime.Today;

        public MainViewModel()
        {
            _database = new ToDoDB();
            TodoItems = new ObservableCollection<ToDoItem>();
            DoneItems = new ObservableCollection<ToDoItem>();
            OverDueItems = new ObservableCollection<ToDoItem>();
            LoadItemsCommand.Execute(null);

            _ = OverDueChecker();
        }

        [RelayCommand]
        private async Task LoadItems()
        {
            var items = await _database.GetItems();
            if (SelectedCategory == "Все"){
                TodoItems = new ObservableCollection<ToDoItem>(items.Where(item => !item.IsCompleted && !item.IsOverDue));
                DoneItems = new ObservableCollection<ToDoItem>(items.Where(item => item.IsCompleted == true));
                OverDueItems = new ObservableCollection<ToDoItem>(items.Where(item => item.IsOverDue == true));
            }
            else
            {
                var filteredItems = items.Where(item => item.Category == SelectedCategory);
                TodoItems = new ObservableCollection<ToDoItem>(filteredItems.Where(item => !item.IsCompleted && !item.IsOverDue));
                DoneItems = new ObservableCollection<ToDoItem>(filteredItems.Where(item => item.IsCompleted == true));
                OverDueItems = new ObservableCollection<ToDoItem>(filteredItems.Where(item => item.IsOverDue == true));
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
            if (!string.IsNullOrWhiteSpace(NewTaskName) && 
                !string.IsNullOrWhiteSpace(NewCategory))
            {
                var newItem = new ToDoItem { ItemName = NewTaskName, Category = NewCategory, EndTime = NewEndTime, EndDate = NewEndDate, IsCompleted = false };
                await _database.SaveItem(newItem);

                NewTaskName = string.Empty;
                NewCategory = "Личное";
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

        private async Task OverDueComplete(List<ToDoItem> items)
        {
            foreach (ToDoItem item in items)
            {
                item.IsOverDue = true;

                await _database.SaveItem(item);
            }
            LoadItemsCommand.Execute(null);
        }

        private async Task CheckOverDue()
        {
            var CheckItems = new List<ToDoItem>();
            foreach (ToDoItem item in TodoItems)
            {
                if (item.EndDate < DateTime.Today ||
                   (item.EndDate == DateTime.Today &&
                   (item.EndTime < DateTime.Now.TimeOfDay)))
                {
                    CheckItems.Add(item);
                }
            }
            if (CheckItems.Count > 0)
            {
                ShowOverduePushNotification(CheckItems);
                await OverDueComplete(CheckItems);
            }
        }

        private async Task OverDueChecker()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                await CheckOverDue();
            }
        }

        private async Task ShowOverduePushNotification(List<ToDoItem> items)
        {
            if (items.Count == 1)
            {
                await Application.Current.MainPage.DisplayAlert(
                    $"ЗАДАЧА {items[0].ItemName} ПРОСРОЧЕНА!",
                    "У вас есть просроченные задачи",
                    "ОК");
            }
            else {
                await Application.Current.MainPage.DisplayAlert(
                    "ЗАДАЧИ ПРОСРОЧЕНЫ!",
                    $"У вас есть {items.Count} просроченных задач",
                    "ОК");
            }
        }
    }
}