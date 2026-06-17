using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskCreator.Domain.Models;

namespace TaskManager.TaskCreator.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishTaskCreatedAsync(TaskItem task);
        Task PublishTaskDeletedAsync(TaskItem task);
    }
}
