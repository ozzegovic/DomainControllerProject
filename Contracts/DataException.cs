using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class DataException
    {
        private string message;

        [DataMember]
        public string Message { get => message; set => message = value; }

        public DataException(string message)
        {
            Message = message;
        }

        public DataException() : this("") { }
    }
}
