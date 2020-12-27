using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IDomainControllerClient : IDomainController
    {
        //if user exists, return challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short startAuthetication(string username, string service);

    }

    [ServiceContract]
    public interface IDomainControllerService : IDomainController
    {
        // if service exists, return challenge
        // check later how to authenticate a service
        // is there a difference between a service account and user account authentication
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short startAuthetication(string serviceName);
    }

    [ServiceContract]
    public interface IDomainController
    {
        // compare the response with the user password from the database
        // TO DO: remove username, add sessions 
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        Tuple<byte[], string> SendResponse(string username, byte[] response);
    }
}
