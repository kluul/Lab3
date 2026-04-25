using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab3.Models;
using Lab3.Services;

namespace Lab3.Tests;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TodoTask> _tasks = [];

    public void Seed(params TodoTask[] tasks) => _tasks.AddRange(tasks);

    public Task<IEnumerable<TodoTask>> GetAllAsync() =>
        Task.FromResult<IEnumerable<TodoTask>>(_tasks.ToList());

    public Task AddAsync(TodoTask task) { _tasks.Add(task); return Task.CompletedTask; }

    public Task UpdateAsync(TodoTask task)
    {
        var idx = _tasks.FindIndex(t => t.Id == task.Id);
        if (idx >= 0) _tasks[idx] = task;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id) { _tasks.RemoveAll(t => t.Id == id); return Task.CompletedTask; }

    public Task DeleteManyAsync(IEnumerable<Guid> ids)
    {
        var set = ids.ToHashSet();
        _tasks.RemoveAll(t => set.Contains(t.Id));
        return Task.CompletedTask;
    }
}
