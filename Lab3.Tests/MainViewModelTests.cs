using System.Linq;
using System.Threading.Tasks;
using Lab3.Models;
using Lab3.ViewModels;
using NUnit.Framework;

namespace Lab3.Tests;

[TestFixture]
public class MainViewModelTests
{
    private FakeTaskRepository _repo = null!;
    private MainViewModel _vm = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new FakeTaskRepository();
        _vm = new MainViewModel(_repo)
        {
            ShowEditDialog = _ => true,
            ShowConfirmDialog = _ => true
        };
    }

    [Test]
    public async Task LoadCommand_PopulatesTasksList()
    {
        _repo.Seed(
            new TodoTask { Title = "Задача 1" },
            new TodoTask { Title = "Задача 2" });

        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task AddCommand_AddsTaskToList()
    {
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        _vm.ShowEditDialog = vm =>
        {
            vm.Title = "Новая задача";
            return true;
        };

        _vm.AddCommand.Execute(null);
        await Task.Delay(50);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
        Assert.That(_vm.Tasks[0].Title, Is.EqualTo("Новая задача"));
    }

    [Test]
    public async Task DeleteCommand_RemovesTaskFromList()
    {
        _repo.Seed(new TodoTask { Title = "Удали меня" });
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        var item = _vm.Tasks[0];
        _vm.DeleteCommand.Execute(item);
        await Task.Delay(50);

        Assert.That(_vm.Tasks, Is.Empty);
    }

    [Test]
    public async Task FilterActive_ShowsOnlyIncompleteTasks()
    {
        _repo.Seed(
            new TodoTask { Title = "Активная", IsCompleted = false },
            new TodoTask { Title = "Выполненная", IsCompleted = true });
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        _vm.SetFilterCommand.Execute(FilterStatus.Active);
        await Task.Delay(50);

        Assert.That(_vm.Tasks.All(t => !t.IsCompleted), Is.True);
        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task FilterCompleted_ShowsOnlyCompletedTasks()
    {
        _repo.Seed(
            new TodoTask { Title = "Активная", IsCompleted = false },
            new TodoTask { Title = "Выполненная", IsCompleted = true });
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        _vm.SetFilterCommand.Execute(FilterStatus.Completed);
        await Task.Delay(50);

        Assert.That(_vm.Tasks.All(t => t.IsCompleted), Is.True);
        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task DeleteSelectedCommand_RemovesSelectedTasks()
    {
        _repo.Seed(
            new TodoTask { Title = "Задача A" },
            new TodoTask { Title = "Задача B" },
            new TodoTask { Title = "Задача C" });
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        _vm.Tasks[0].IsSelected = true;
        _vm.Tasks[2].IsSelected = true;

        _vm.DeleteSelectedCommand.Execute(null);
        await Task.Delay(50);

        Assert.That(_vm.Tasks, Has.Count.EqualTo(1));
        Assert.That(_vm.Tasks[0].Title, Is.EqualTo("Задача B"));
    }

    [Test]
    public async Task EditCommand_UpdatesTask()
    {
        _repo.Seed(new TodoTask { Title = "Старое название" });
        _vm.LoadCommand.Execute(null);
        await Task.Delay(50);

        var item = _vm.Tasks[0];
        _vm.ShowEditDialog = vm =>
        {
            vm.Title = "Новое название";
            vm.IsCompleted = true;
            return true;
        };

        _vm.EditCommand.Execute(item);
        await Task.Delay(50);

        Assert.That(_vm.Tasks[0].Title, Is.EqualTo("Новое название"));
        Assert.That(_vm.Tasks[0].IsCompleted, Is.True);
    }
}
