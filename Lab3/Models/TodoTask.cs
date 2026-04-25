using System;

namespace Lab3.Models;

public class TodoTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public static string? ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "Название обязательно";
        if (title.Length > 100)
            return "Название не должно превышать 100 символов";
        return null;
    }

    public static string? ValidateDescription(string? description)
    {
        if (description?.Length > 500)
            return "Описание не должно превышать 500 символов";
        return null;
    }
}
