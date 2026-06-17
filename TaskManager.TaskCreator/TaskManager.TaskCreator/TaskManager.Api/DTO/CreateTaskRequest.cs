using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Api.DTO
{
    public class CreateTaskRequest
    {
        public string description { get; set; } = string.Empty;
    }
}
