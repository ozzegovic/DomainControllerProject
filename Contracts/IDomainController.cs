using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IDomainControllerClient 
    {
        //if user exists, return challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short StartClientAuthentication(string service);

        // compare the response with the user password from the database
        // return session key and address of the service
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        ClientSessionData SendResponse(byte[] response);

    }

    [ServiceContract]
    public interface IDomainControllerService 
    {
        // if service exists, return challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short StartServiceAuthentication(string serviceName);

        // compare the response with the service password from the database
        // return service address if found
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string SendResponseService(byte[] response);

    }


}
