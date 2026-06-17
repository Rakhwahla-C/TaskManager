using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.TaskCreator.Domain.Models
{
    public class TaskItem
    {
        public string TaskId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;

        public TaskItem(string taskId, string description)
        {
            TaskId = taskId;
            Description = description;
            CreatedAt = DateTime.UtcNow.ToString();
        }
    }
}
