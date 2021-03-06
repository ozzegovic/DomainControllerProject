﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    [ServiceContract]
    public interface ITicketGrantingService
    {
        // checks if the requested service exists in the dnsTable and returns the full address
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string GetServiceAddress(string serviceAddress);

        // gets the username of the account that started the service
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string GetServiceUser(string serviceAddress);


        // after service authentication, add it to the active services list
        // save username that started the service
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool ActivateService(string serviceAddress, string username);

        // could not connect to the service, set it to inactive
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool DeactivateService(string serviceAddress);

        // check if requested service is active
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool IsServiceOnline(string serviceAddress);

        //generate session key 
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string GenerateSessionKey();
    }
}
