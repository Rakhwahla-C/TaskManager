using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using System.Text.Json;
using TaskManager.Api.DTO;
using TaskManager.TaskCreator.Application.Interfaces;
using TaskManager.TaskCreator.Application.Services;
using TaskManager.Infra.Services;
using TaskManager.Application.Interfaces;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TaskManager.TaskCreator.TaskCreator.Api
{
    public class Function
    {
        private readonly TaskService _service;
        IAmazonDynamoDB dynamoDb = new AmazonDynamoDBClient();
        IAmazonSimpleNotificationService snsClient = new AmazonSimpleNotificationServiceClient();

        public Function()
        {

            ITaskRepository repo = new DynamoTaskRepository(dynamoDb);
            IMessagePublisher publisher = new SnsNotificationPublisher(snsClient);

            _service = new TaskService(repo, publisher);
        }

        private static readonly Dictionary<string, string> CorsHeaders = new()
        {
            { "Access-Control-Allow-Origin", "http://localhost:3000" },
            { "Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,x-api-key" },
            { "Access-Control-Allow-Methods", "GET,POST,DELETE,PUT,OPTIONS" },
            { "Access-Control-Allow-Credentials", "true" }
        };
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Received {request.HttpMethod} request");


            if (request.HttpMethod == "OPTIONS")
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = CorsHeaders
                };
            }

            try
            {
                return request.HttpMethod switch
                {
                    "POST" => await CreateTaskAsync(request),
                    "GET" => await GetAllTasksAsync(),
                    "DELETE" => await DeleteTaskAsync(request),
                    "PUT" => await UpdateTaskAsync(request),
                    _ => new APIGatewayProxyResponse
                    {
                        StatusCode = 405,
                        Body = "Method Not Allowed",
                        Headers = CorsHeaders
                    }

                };
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}\n{ex.StackTrace}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonSerializer.Serialize(new { error = "Internal Server Error" }),
                    Headers = new Dictionary<string, string>(CorsHeaders)
                    {
                        { "Content-Type", "application/json" }
                    }
                };
            }

        }

        public async Task<APIGatewayProxyResponse> CreateTaskAsync(APIGatewayProxyRequest request)
        {
            var payload = JsonSerializer.Deserialize<CreateTaskRequest>(request.Body);
            var task = await _service.CreateTaskAsync(payload.description);

            return new APIGatewayProxyResponse
            {
                StatusCode = 201,
                Body = JsonSerializer.Serialize(task),
                Headers = new Dictionary<string, string>(CorsHeaders)
                    {
                        { "Content-Type", "application/json" }
                    }
            };
        }

        public async Task<APIGatewayProxyResponse> GetAllTasksAsync()
        {
            var tasks = await _service.GetAllTaskAsync();

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(tasks),
                Headers = new Dictionary<string, string>(CorsHeaders)
                    {
                        { "Content-Type", "application/json" }
                    }
            };
        }

        public async Task<APIGatewayProxyResponse> UpdateTaskAsync(APIGatewayProxyRequest request)
        {
            var taskId = request.PathParameters["id"];
            var payload = JsonSerializer.Deserialize<UpdateTaskRequest>(request.Body);
            var task = await _service.UpdateTaskAsync(taskId, payload.description);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(task),
                Headers = new Dictionary<string, string>(CorsHeaders)
                    {
                        { "Content-Type", "application/json" }
                    }
            };
        }

        public async Task<APIGatewayProxyResponse> DeleteTaskAsync(APIGatewayProxyRequest request)
        {
            var taskId = request.PathParameters["id"];
            await _service.DeleteTaskAsync(taskId);

            return new APIGatewayProxyResponse
            {
                StatusCode = 204,
                Headers = new Dictionary<string, string>(CorsHeaders)
                    {
                        { "Content-Type", "application/json" }
                    }
            };
        }
    }
}


