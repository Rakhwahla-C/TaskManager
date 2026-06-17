using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskCreator.Domain.Models;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(string id);
        Task UpdateTaskAsync(string id, string newDescription);
        Task DeleteTaskAsync(string id);
        Task SaveAsync(TaskItem task);
    }
}
