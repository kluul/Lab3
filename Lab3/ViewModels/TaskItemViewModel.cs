using System;
using Lab3.Models;

namespace Lab3.ViewModels;

public class TaskItemViewModel : BaseViewModel
{
    private bool _isSelected;

    public TodoTask Task { get; }

    public TaskItemViewModel(TodoTask task) => Task = task;

    public Guid Id => Task.Id;
    public string Title => Task.Title;
    public string Description => Task.Description;
    public bool IsCompleted => Task.IsCompleted;
    public DateTime CreatedAt => Task.CreatedAt;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
