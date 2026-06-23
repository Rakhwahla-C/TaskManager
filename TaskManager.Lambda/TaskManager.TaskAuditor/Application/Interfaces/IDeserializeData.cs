using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskAuditor;
using TaskManager.TaskAuditor.Models;

namespace TaskManager.TaskAuditor.Application.Interfaces
{
    public interface IDeserializeData
    {
        Task<TaskEvent> Deserialize(string message);
    }
}
