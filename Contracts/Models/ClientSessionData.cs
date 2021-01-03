using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    [Serializable]
    public class ClientSessionData
    {
        public byte[] SessionKey { get; set; }
        public string ServiceAddress { get; set; }

        public ClientSessionData(byte[] sessionKey, string serviceAddress)
        {
            SessionKey = sessionKey;
            ServiceAddress = serviceAddress;
        }
    }
}
