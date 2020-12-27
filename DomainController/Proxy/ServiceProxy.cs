using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    public class ServiceProxy : IDataManagementDC
    {
        // authenticated client requested a DataManagement service (service is active)
        // TO DO: send encrypted session key to the service
        public bool SendSessionKey(byte[] sessionKey)
        {
            throw new NotImplementedException();
        }
    }
}
