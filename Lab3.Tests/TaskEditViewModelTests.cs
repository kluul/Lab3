using System;
using Lab3.Models;
using Lab3.ViewModels;
using NUnit.Framework;

namespace Lab3.Tests;

[TestFixture]
public class TaskEditViewModelTests
{
    [Test]
    public void IsValid_False_WhenTitleIsEmpty()
    {
        var vm = new TaskEditViewModel();
        vm.Title = "";
        Assert.That(vm.IsValid, Is.False);
    }

    [Test]
    public void IsValid_True_WhenTitleIsSet()
    {
        var vm = new TaskEditViewModel();
        vm.Title = "Тестовая задача";
        Assert.That(vm.IsValid, Is.True);
    }

    [Test]
    public void TitleError_SetWhenTitleTooLong()
    {
        var vm = new TaskEditViewModel();
        vm.Title = new string('A', 101);
        Assert.That(vm.TitleError, Is.Not.Null);
        Assert.That(vm.IsValid, Is.False);
    }

    [Test]
    public void ToModel_ReturnsCorrectTask_ForNewTask()
    {
        var vm = new TaskEditViewModel
        {
            Title = "  Тест  ",
            Description = "  Описание  ",
            IsCompleted = true
        };

        var task = vm.ToModel();

        Assert.That(task.Title, Is.EqualTo("Тест"));
        Assert.That(task.Description, Is.EqualTo("Описание"));
        Assert.That(task.IsCompleted, Is.True);
        Assert.That(task.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void ToModel_PreservesId_WhenEditing()
    {
        var original = new TodoTask { Title = "Оригинал", Id = Guid.NewGuid() };
        var vm = new TaskEditViewModel(original) { Title = "Изменено" };

        var task = vm.ToModel();

        Assert.That(task.Id, Is.EqualTo(original.Id));
    }

    [Test]
    public void IsNew_True_ForNewTask()
    {
        var vm = new TaskEditViewModel();
        Assert.That(vm.IsNew, Is.True);
    }

    [Test]
    public void IsNew_False_WhenEditingExistingTask()
    {
        var original = new TodoTask { Title = "Задача" };
        var vm = new TaskEditViewModel(original);
        Assert.That(vm.IsNew, Is.False);
    }
}
