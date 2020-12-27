using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    // available to Client
    [ServiceContract]
    public interface IDataManagement
    {

        [OperationContract]
        bool Read(byte[] token);

        [OperationContract]
        bool Write(byte[] token);
    }

    // available to DomainController
    [ServiceContract]
    public interface IDataManagementDC
    {
        [OperationContract]
        bool SendSessionKey(byte[] sessionKey);
    }

}
