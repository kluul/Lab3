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
    private readonly ITaskRepository _repository;
    private ObservableCollection<TaskItemViewModel> _tasks = [];
    private FilterStatus _currentFilter = FilterStatus.All;
    private bool _isLoading;
    private string _statusText = string.Empty;

    public MainViewModel(ITaskRepository repository)
    {
        _repository = repository;

        LoadCommand = new AsyncRelayCommand(LoadTasksAsync);
        AddCommand = new AsyncRelayCommand(AddTaskAsync, () => !IsLoading);
        EditCommand = new AsyncRelayCommand(EditTaskAsync, p => p is TaskItemViewModel && !IsLoading);
        DeleteCommand = new AsyncRelayCommand(DeleteTaskAsync, p => p is TaskItemViewModel && !IsLoading);
        DeleteSelectedCommand = new AsyncRelayCommand(DeleteSelectedAsync, () => Tasks.Any(t => t.IsSelected) && !IsLoading);
        SetFilterCommand = new RelayCommand(p => ApplyFilter((FilterStatus)p!));
    }

    public ObservableCollection<TaskItemViewModel> Tasks
    {
        get => _tasks;
        private set => SetProperty(ref _tasks, value);
    }

    public FilterStatus CurrentFilter
    {
        get => _currentFilter;
        private set
        {
            SetProperty(ref _currentFilter, value);
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
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
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
            var all = await _repository.GetAllAsync();
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
            await _repository.AddAsync(task);
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
            await _repository.UpdateAsync(task);
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
            await _repository.DeleteAsync(item.Id);
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
            await _repository.DeleteManyAsync(selected.Select(t => t.Id));
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
        var all = await _repository.GetAllAsync();
        var filtered = Filter(all);
        Tasks = new ObservableCollection<TaskItemViewModel>(filtered.Select(t => new TaskItemViewModel(t)));
        UpdateStatus();
    }

    private IEnumerable<TodoTask> Filter(IEnumerable<TodoTask> tasks) => CurrentFilter switch
    {
        FilterStatus.Active => tasks.Where(t => !t.IsCompleted),
        FilterStatus.Completed => tasks.Where(t => t.IsCompleted),
        _ => tasks
    };

    private void UpdateStatus()
    {
        var total = Tasks.Count;
        var completed = Tasks.Count(t => t.IsCompleted);
        StatusText = $"Задач: {total} | Выполнено: {completed}";
    }
}
