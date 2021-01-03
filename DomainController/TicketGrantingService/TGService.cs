using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class TGService : ITicketGrantingService
    {

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

        // Add the service to the dnsActiveServices list
        // TO DO: instad serviceAddress,true add serviceAddres, serviceIdentity
        public bool AddOnlineService(string serviceAddress)
        {
            if (!DNSActiveServices.dnsActiveServices.ContainsKey(serviceAddress))
            {
                DNSActiveServices.dnsActiveServices.Add(serviceAddress, true);
                return true;
            }
            else
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service already online."));
        }


        // checks if existing service is started
        public string CheckOnlineService(string serviceAddress)
        {
            if (!DNSActiveServices.dnsActiveServices.ContainsKey(serviceAddress))
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service is not online."));
            }
            else
                return serviceAddress;
        }

        public string GenerateSessionKey()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] key = new byte[24]; // For a 192-bit key
            rng.GetBytes(key);

            return ASCIIEncoding.ASCII.GetString(key);
        }
    }
}
