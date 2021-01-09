using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    // available to Client
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

    // available to Service
    [ServiceContract]
    public interface IDomainControllerService 
    {
        // if user exists, return challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short StartServiceAuthentication(string serviceName);

        // compare the response with the service password from the database
        // validate the service and activate the service
        // return confirmation
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool SendResponseService(byte[] response);

    }


}
