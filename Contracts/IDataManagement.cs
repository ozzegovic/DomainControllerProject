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
        [FaultContract(typeof(DataException))]
        [FaultContract(typeof(SecurityException))]
        byte[] Read(byte[] key);

        [OperationContract]
        [FaultContract(typeof(DataException))]
        [FaultContract(typeof(SecurityException))]
        bool Write(byte[] key, byte[] value);
    }

    // available to DomainController
    [ServiceContract]
    public interface IDataManagementDC
    {
        [OperationContract]
        bool SendSessionKey(string user, byte[] encryptedSessionKey);
    }

}
