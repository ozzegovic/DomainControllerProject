using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    public class UserRequest
    {
        string username;
        string requestedService;
        byte[] sessionKey;
        short challenge;

        public string Username { get => username; set => username = value; }
        public string RequestedService { get => requestedService; set => requestedService = value; }
        public byte[] SessionKey { get => sessionKey; set => sessionKey = value; }
        public short Challenge { get => challenge; set => challenge = value; }


    }
}
