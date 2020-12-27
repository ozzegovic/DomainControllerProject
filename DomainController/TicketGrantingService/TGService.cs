using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class TGService : ITicketGrantingService
    {

        // TO DO: check if the service is in dnsActiveServices list
        // currently: checks if the service exists in the dnsTable and returns the full address
        // Client then uses this address to connect to the service
        public string ServiceExists(string serviceAddress)
        {
            if (!DNSTable.dnsTable.ContainsKey(serviceAddress))
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
            else
                return DNSTable.dnsTable[serviceAddress] + "/" + serviceAddress;
        }
    }
}
