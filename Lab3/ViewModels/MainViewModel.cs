using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Lab3.Models;
using Lab3.Services;

namespace Lab3.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly ITaskRepository repository;
    private ObservableCollection<TaskItemViewModel> tasks = [];
    private FilterStatus currentFilter = FilterStatus.All;
    private bool isLoadingField;
    private string statusText = string.Empty;

    public MainViewModel(ITaskRepository repository)
    {
        this.repository = repository;

        LoadCommand = new AsyncRelayCommand(LoadTasksAsync);
        AddCommand = new AsyncRelayCommand(AddTaskAsync, () => !IsLoading);
        EditCommand = new AsyncRelayCommand(EditTaskAsync, p => p is TaskItemViewModel && !IsLoading);
        DeleteCommand = new AsyncRelayCommand(DeleteTaskAsync, p => p is TaskItemViewModel && !IsLoading);
        DeleteSelectedCommand = new AsyncRelayCommand(DeleteSelectedAsync, () => Tasks.Any(t => t.IsSelected) && !IsLoading);
        SetFilterCommand = new RelayCommand(p => ApplyFilter((FilterStatus)p!));
    }

    public ObservableCollection<TaskItemViewModel> Tasks
    {
        get => tasks;
        private set => SetProperty(ref tasks, value);
    }

    public FilterStatus CurrentFilter
    {
        get => currentFilter;
        private set
        {
            SetProperty(ref currentFilter, value);
            OnPropertyChanged(nameof(IsFilterAll));
            OnPropertyChanged(nameof(IsFilterActive));
            OnPropertyChanged(nameof(IsFilterCompleted));
        }
    }

    public bool IsFilterAll => CurrentFilter == FilterStatus.All;
    public bool IsFilterActive => CurrentFilter == FilterStatus.Active;
    public bool IsFilterCompleted => CurrentFilter == FilterStatus.Completed;

    public bool IsLoading
    {
        get => isLoadingField;
        private set => SetProperty(ref isLoadingField, value);
    }

    public string StatusText
    {
        get => statusText;
        private set => SetProperty(ref statusText, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand DeleteSelectedCommand { get; }
    public ICommand SetFilterCommand { get; }

    public Func<TaskEditViewModel, bool?>? ShowEditDialog { get; set; }
    public Func<string, bool>? ShowConfirmDialog { get; set; }

    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        StatusText = "Загрузка...";
        try
        {
            var all = await repository.GetAllAsync();
            var filtered = Filter(all);
            Tasks = new ObservableCollection<TaskItemViewModel>(filtered.Select(t => new TaskItemViewModel(t)));
            UpdateStatus();
        }
        finally { IsLoading = false; }
    }

    private async Task AddTaskAsync()
    {
        var editVm = new TaskEditViewModel();
        if (ShowEditDialog?.Invoke(editVm) != true) return;

        IsLoading = true;
        StatusText = "Сохранение...";
        try
        {
            var task = editVm.ToModel();
            await repository.AddAsync(task);
            await RefreshListAsync();
        }
        finally { IsLoading = false; }
    }

    private async Task EditTaskAsync(object? parameter)
    {
        if (parameter is not TaskItemViewModel item) return;

        var editVm = new TaskEditViewModel(item.Task);
        if (ShowEditDialog?.Invoke(editVm) != true) return;

        IsLoading = true;
        StatusText = "Сохранение...";
        try
        {
            var task = editVm.ToModel();
            await repository.UpdateAsync(task);
            await RefreshListAsync();
        }
        finally { IsLoading = false; }
    }

    private async Task DeleteTaskAsync(object? parameter)
    {
        if (parameter is not TaskItemViewModel item) return;

        if (ShowConfirmDialog?.Invoke($"Удалить задачу «{item.Title}»?") != true) return;

        IsLoading = true;
        StatusText = "Удаление...";
        try
        {
            await repository.DeleteAsync(item.Id);
            await RefreshListAsync();
        }
        finally { IsLoading = false; }
    }

    private async Task DeleteSelectedAsync()
    {
        var selected = Tasks.Where(t => t.IsSelected).ToList();
        if (selected.Count == 0) return;

        if (ShowConfirmDialog?.Invoke($"Удалить {selected.Count} задач(и)?") != true) return;

        IsLoading = true;
        StatusText = "Удаление...";
        try
        {
            await repository.DeleteManyAsync(selected.Select(t => t.Id));
            await RefreshListAsync();
        }
        finally { IsLoading = false; }
    }

    private void ApplyFilter(FilterStatus filter)
    {
        CurrentFilter = filter;
        _ = RefreshListAsync();
    }

    private async Task RefreshListAsync()
    {
        var all = await repository.GetAllAsync();
        var filtered = Filter(all);
        Tasks = new ObservableCollection<TaskItemViewModel>(filtered.Select(t => new TaskItemViewModel(t)));
        UpdateStatus();
    }

    private IEnumerable<TodoTask> Filter(IEnumerable<TodoTask> taskList) => CurrentFilter switch
    {
        FilterStatus.Active => taskList.Where(t => !t.IsCompleted),
        FilterStatus.Completed => taskList.Where(t => t.IsCompleted),
        _ => taskList
    };

    private void UpdateStatus()
    {
        var total = Tasks.Count;
        var completed = Tasks.Count(t => t.IsCompleted);
        StatusText = $"Задач: {total} | Выполнено: {completed}";
    }
}
