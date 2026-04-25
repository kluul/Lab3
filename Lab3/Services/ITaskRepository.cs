using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lab3.Models;

namespace Lab3.Services;

public interface ITaskRepository
{
    Task<IEnumerable<TodoTask>> GetAllAsync();
    Task AddAsync(TodoTask task);
    Task UpdateAsync(TodoTask task);
    Task DeleteAsync(Guid id);
    Task DeleteManyAsync(IEnumerable<Guid> ids);
}
