using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Lab3.Models;

namespace Lab3.Services;

public class JsonTaskRepository : ITaskRepository
{
    private static readonly string DataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Lab3", "tasks.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private List<TodoTask> cache = [];

    public async Task<IEnumerable<TodoTask>> GetAllAsync()
    {
        await SimulateNetworkDelay();
        cache = await LoadFromDisk();
        return cache.AsReadOnly();
    }

    public async Task AddAsync(TodoTask task)
    {
        await SimulateNetworkDelay();
        cache = await LoadFromDisk();
        cache.Add(task);
        await SaveToDisk(cache);
    }

    public async Task UpdateAsync(TodoTask task)
    {
        await SimulateNetworkDelay();
        cache = await LoadFromDisk();
        var idx = cache.FindIndex(t => t.Id == task.Id);
        if (idx >= 0)
            cache[idx] = task;
        await SaveToDisk(cache);
    }

    public async Task DeleteAsync(Guid id)
    {
        await SimulateNetworkDelay();
        cache = await LoadFromDisk();
        cache.RemoveAll(t => t.Id == id);
        await SaveToDisk(cache);
    }

    public async Task DeleteManyAsync(IEnumerable<Guid> ids)
    {
        await SimulateNetworkDelay();
        var idSet = ids.ToHashSet();
        cache = await LoadFromDisk();
        cache.RemoveAll(t => idSet.Contains(t.Id));
        await SaveToDisk(cache);
    }

    private static async Task<List<TodoTask>> LoadFromDisk()
    {
        if (!File.Exists(DataPath))
            return [];
        try
        {
            var json = await File.ReadAllTextAsync(DataPath);
            return JsonSerializer.Deserialize<List<TodoTask>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static async Task SaveToDisk(List<TodoTask> tasks)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DataPath)!);
        var json = JsonSerializer.Serialize(tasks, JsonOptions);
        await File.WriteAllTextAsync(DataPath, json);
    }

    private static Task SimulateNetworkDelay()
    {
        var ms = Random.Shared.Next(500, 1001);
        return Task.Delay(ms);
    }
}
