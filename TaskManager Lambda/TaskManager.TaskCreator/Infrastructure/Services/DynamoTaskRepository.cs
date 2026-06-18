
using TaskManager.Application.Interfaces;
using TaskManager.TaskCreator.Domain;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManager.TaskCreator.Domain.Models;

public class DynamoTaskRepository : ITaskRepository
{
    private readonly IAmazonDynamoDB _dynamoClient;
    private readonly string _tableName = "task-manager";

    public DynamoTaskRepository(IAmazonDynamoDB ddb) => _dynamoClient = ddb;

    public async Task SaveAsync(TaskItem task)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["id"] = new AttributeValue(task.TaskId),
            ["description"] = new AttributeValue(task.Description),
            ["createdAt"] = new AttributeValue(task.CreatedAt.ToString())
        };
        await _dynamoClient.PutItemAsync(_tableName, item);
    }
    

    public async Task<TaskItem> GetTaskByIdAsync(string taskId)
    {
        var response = await _dynamoClient.GetItemAsync(_tableName, new Dictionary<string, AttributeValue>
        {
            ["id"] = new AttributeValue(taskId)
        });

        return new TaskItem(
            response.Item["id"].S,
            response.Item["description"].S
        );
    }
    
    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        var scanResponse = await _dynamoClient.ScanAsync(new ScanRequest
        {
            TableName = _tableName
        });

        var tasks = scanResponse.Items.Select(item => 
        {
            var id = item["id"].S;
            var description = item.ContainsKey("description") ? item["description"].S : "Null";
            var createdAt = item.ContainsKey("createdAt") ? DateTime.Parse(item["createdAt"].S)
                : DateTime.MinValue;
            return new TaskItem(id, description);
        }).OrderByDescending(t => t.TaskId).ToList();

        return tasks;
    }

    public async Task DeleteTaskAsync(string taskId)
    {
        await _dynamoClient.DeleteItemAsync(_tableName, new Dictionary<string, AttributeValue>
        {
            ["id"] = new AttributeValue(taskId)
        });
    }

    public async Task UpdateTaskAsync(string taskId, string newDescription)
    {
        var response = await _dynamoClient.GetItemAsync(_tableName, new Dictionary<string, AttributeValue>
        {
            ["id"] = new AttributeValue(taskId)
        });

        var update = new UpdateItemRequest
        {
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = taskId } }
            },
            UpdateExpression = "SET description = :desc",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":desc", new AttributeValue { S = newDescription } }
            },
            ReturnValues = "UPDATED_NEW"
        };

        var responseDB = await _dynamoClient.UpdateItemAsync(update);



    }
}



