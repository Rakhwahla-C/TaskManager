
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.TaskCreator.Application.Interfaces;
using TaskManager.TaskCreator.Domain.Models;

namespace TaskManager.TaskCreator.Application.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repo;
        private readonly IMessagePublisher _publisher;
        

        public TaskService(ITaskRepository repo, IMessagePublisher publisher)
        {
            _repo = repo;
            _publisher = publisher;
            
        }

        public async Task<TaskItem> CreateTaskAsync(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description cannot be empty");
            var task = new TaskItem(Guid.NewGuid().ToString(), description);
            await _repo.SaveAsync(task);
            await _publisher.PublishTaskCreatedAsync(task);
            return task;
        }

        public async Task<TaskItem> GetTaskAsync(string taskId)
        {
            var task = await _repo.GetTaskByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task {taskId} not found");
            return task;
        }

        public async Task<List<TaskItem>> GetAllTaskAsync()
        {
            return await _repo.GetAllTasksAsync();
        }



        public async Task<TaskItem> UpdateTaskAsync(string taskId, string newDescription)
        {
            var task = await _repo.GetTaskByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException($"Task {taskId} not found");

            task.Description = newDescription;
            await _repo.UpdateTaskAsync(taskId, newDescription);
            
            return task;
        }

        public async Task DeleteTaskAsync(string taskId)
        {
            var task = await _repo.GetTaskByIdAsync(taskId);
            if (task == null) throw new KeyNotFoundException($"Task {taskId} not found");


            await _repo.DeleteTaskAsync(taskId);
            await _publisher.PublishTaskDeletedAsync(task);
        }
    }
}
