using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskAuditor.Application.Interfaces;
using TaskManager.TaskAuditor;
using TaskManager.TaskAuditor.Models;

namespace TaskManager.TaskAuditor.Infrastructure
{
    public class MessageLogger : IMessageLogger
    {
        private readonly ILogger<MessageLogger> _logger;
        private static int totalDeleted = 0, totalCreated = 0;
        public MessageLogger(ILogger<MessageLogger> logger)
        {
            _logger = logger;
        }
        public Task LogMessage(TaskEvent taskEvent)
        {
            _logger.LogInformation("Task Event: \nType={Type} \nTaskId={TaskId} \nDescription={Description}",
                taskEvent.Type,
                taskEvent.TaskId,
                taskEvent.Description
                );

            
            if (taskEvent.Type == "TaskCreated")
                totalCreated++;
            else if(taskEvent.Type == "TaskDeleted")
                totalDeleted++;

            _logger.LogInformation("Metrics: TotalCreated={TotalCreated}, TotalDeleted={TotalDeleted}",
                totalCreated,
                totalDeleted
            );

            return Task.CompletedTask;
        }
        
    }
}
