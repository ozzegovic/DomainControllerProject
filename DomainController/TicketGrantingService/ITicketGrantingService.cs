using Contracts;
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
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string GetServiceAddress(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool ActivateService(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool DeactivateService(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool IsServiceOnline(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string GenerateSessionKey();
    }
}
