using System;
using Lab3.Models;

namespace Lab3.ViewModels;

public class TaskEditViewModel : BaseViewModel
{
    private string title = string.Empty;
    private string description = string.Empty;
    private bool isCompleted;
    private string? titleError;

    public TaskEditViewModel() { }

    public TaskEditViewModel(TodoTask task)
    {
        title = task.Title;
        description = task.Description;
        isCompleted = task.IsCompleted;
        OriginalId = task.Id;
        CreatedAt = task.CreatedAt;
    }

    public Guid? OriginalId { get; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public bool IsNew => OriginalId is null;

    public string Title
    {
        get => title;
        set
        {
            SetProperty(ref title, value);
            TitleError = TodoTask.ValidateTitle(value);
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public string Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set => SetProperty(ref isCompleted, value);
    }

    public string? TitleError
    {
        get => titleError;
        private set => SetProperty(ref titleError, value);
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
