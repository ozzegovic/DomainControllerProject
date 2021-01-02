﻿using System;
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
        short startAuthetication(string username, string service);

        // compare the response with the user password from the database
        // return session key and address of the service
        // TO DO: remove username, add sessions 
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        Tuple<byte[], string> SendResponse(string username, byte[] response);

    }

    [ServiceContract]
    public interface IDomainControllerService 
    {
        // if service exists, return challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short startAuthetication(string serviceName);

        // compare the response with the service password from the database
        // TO DO: remove username, add sessions 
        // return true if authenticated, session key is sent only after there is an authenticated client
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool SendResponse(byte[] response);

    }


}
