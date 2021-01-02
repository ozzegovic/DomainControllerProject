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
        string ServiceExists(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool AddOnlineService(string serviceAddress);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string CheckOnlineService(string serviceAddress);

    }
}
