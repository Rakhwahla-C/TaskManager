using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TaskManager.TaskAuditor.Application.Interfaces;
using TaskManager.TaskAuditor;
using TaskManager.TaskAuditor.Models;

namespace TaskManager.TaskAuditor.Infrastructure
{
    public class DeserializeData : IDeserializeData
    {
        public  Task<TaskEvent> Deserialize(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is empty", nameof(message));

            try
            {
                var task = JsonSerializer.Deserialize<TaskEvent>(message,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (task is null)
                    throw new InvalidDataException("Failed to deserialize data");

                return Task.FromResult(task);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid JSON format.", ex);
            }
        }
    }
}
