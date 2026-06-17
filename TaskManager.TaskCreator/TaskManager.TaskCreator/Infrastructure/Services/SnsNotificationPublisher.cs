using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManager.TaskCreator.Application.Interfaces;
using TaskManager.TaskCreator.Domain.Models;

namespace TaskManager.Infra.Services
{
    
        public class SnsNotificationPublisher : IMessagePublisher
        {
            private readonly IAmazonSimpleNotificationService _sns;
            private readonly string _topicArn = "arn:aws:sns:us-east-1:000000000000:TaskEvents-SNS";

            public SnsNotificationPublisher(IAmazonSimpleNotificationService sns) => _sns = sns;

            public async Task PublishTaskCreatedAsync(TaskItem task) => await PublishEventAsync("TaskCreated", task);
            public async Task PublishTaskDeletedAsync(TaskItem task) => await PublishEventAsync("TaskDeleted", task);

            private async Task PublishEventAsync(string type, TaskItem task)
            {
                var message = JsonSerializer.Serialize(new
                {
                    Type = type,
                    TaskId = task.TaskId,
                    Description = task.Description,
                    CreatedAt = task.CreatedAt
                });

                await _sns.PublishAsync(new PublishRequest
                {
                    TopicArn = _topicArn,
                    Message = message
                });
            }
        }
    }

