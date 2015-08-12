using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class MessageDTO
    {
        public string Message { get; set; }

        public MessageDTO(string message)
        {
            this.Message = message;
        }
    }
}
