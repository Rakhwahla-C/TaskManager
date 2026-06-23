using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskAuditor.Application.Interfaces;

namespace TaskManager.TaskAuditor.Application.Handler
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IMessageLogger _msgLogger;
        private readonly IDeserializeData _deserializer;
        public MessageHandler(IMessageLogger msgLogger, IDeserializeData deserialize)
        {
            _msgLogger = msgLogger;
            _deserializer = deserialize;
        }

        public async Task HandleMessageAsync(string message)
        {
            try
            {
                var task = await _deserializer.Deserialize(message);
                await _msgLogger.LogMessage(task);
            }catch(Exception ex)
            {
                throw new Exception("Failed to Create a Message for Task: ", ex);
            }

        }
    }
}
