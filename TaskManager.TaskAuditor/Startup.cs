using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskAuditor.Application.Handler;
using TaskManager.TaskAuditor.Application.Interfaces;
using TaskManager.TaskAuditor.Infrastructure;
using TaskManager.TaskAuditor;

namespace TaskManager.TaskAuditor { 
    public class Startup 
    { internal readonly IServiceProvider serviceProvider; public IServiceCollection ConfigureServices() 
        { var services = new ServiceCollection(); services.AddLogging(builder => {
            builder.AddConsole(); });
            services.AddSingleton<IDeserializeData, DeserializeData>();
            services.AddSingleton<IMessageLogger, MessageLogger>();
            services.AddSingleton<IMessageHandler, MessageHandler>();
            return services; 
        } 
        public Startup() 
        { var services = ConfigureServices();
            this.serviceProvider = services.BuildServiceProvider(); 
        }
    }
}
