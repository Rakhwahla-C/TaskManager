using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.TaskAuditor.Models
{
    public class TaskEvent
    {
        public string Type { get; set; } = string.Empty;
        public string TaskId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CreatedAt { get; set; }
    }
}
