using System;
using Lab3.Models;

namespace Lab3.ViewModels;

public class TaskEditViewModel : BaseViewModel
{
    private string _title = string.Empty;
    private string _description = string.Empty;
    private bool _isCompleted;
    private string? _titleError;

    public TaskEditViewModel() { }

    public TaskEditViewModel(TodoTask task)
    {
        _title = task.Title;
        _description = task.Description;
        _isCompleted = task.IsCompleted;
        OriginalId = task.Id;
        CreatedAt = task.CreatedAt;
    }

    public Guid? OriginalId { get; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public bool IsNew => OriginalId is null;

    public string Title
    {
        get => _title;
        set
        {
            SetProperty(ref _title, value);
            TitleError = TodoTask.ValidateTitle(value);
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public string? TitleError
    {
        get => _titleError;
        private set => SetProperty(ref _titleError, value);
    }

    public bool IsValid => TitleError is null && !string.IsNullOrWhiteSpace(Title);

    public TodoTask ToModel() => new()
    {
        Id = OriginalId ?? Guid.NewGuid(),
        Title = Title.Trim(),
        Description = Description.Trim(),
        IsCompleted = IsCompleted,
        CreatedAt = CreatedAt
    };
}
