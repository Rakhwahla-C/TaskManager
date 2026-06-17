
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TaskManager.TaskAuditor.Application.Interfaces;
using TaskManager.TaskAuditor.Infrastructure;
using TaskManager.TaskAuditor;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TaskManager.TaskAuditor
{
    public class Function : Startup
    {


        private readonly IMessageHandler _msgHandler;
        public Function()
        {
            _msgHandler = serviceProvider.GetRequiredService<IMessageHandler>();
        }

        public async Task FunctionHandler(SNSEvent snsEvent, ILambdaContext context)
        {
            foreach (var record in snsEvent.Records)
            {
                var snsMessage = record.Sns.Message;
                await _msgHandler.HandleMessageAsync(snsMessage);
            }

           
        }
    }

}
