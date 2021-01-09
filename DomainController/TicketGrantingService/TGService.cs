using Contracts;
using DomainController.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class TGService : ITicketGrantingService
    {

        // checks if the service exists in the dnsTable and returns the full address
        // Client then uses this address to connect to the service
        // Logs result
        public string GetServiceAddress(string serviceName)
        {
            if (!DNSTable.dnsTable.ContainsKey(serviceName))
            {
                try
                {
                    Audit.ValidationFailure(serviceName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
            else
            {
                try
                {
                    Audit.ValidationSuccess(serviceName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return DNSTable.dnsTable[serviceName].Address + "/" + serviceName;
            }
        }
        // Domain Contoller then uses this username to get the correct pass hash in order to encrypt the session key
        public string GetServiceUser(string serviceName)
        {
            if (!DNSTable.dnsTable.ContainsKey(serviceName))
            {

                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
            else
            {

                return DNSTable.dnsTable[serviceName].StartedBy;
            }
        }

        // Set service to active
        public bool ActivateService(string serviceName, string username)
        {
            if (DNSTable.dnsTable.ContainsKey(serviceName))
            {
                if (DNSTable.dnsTable[serviceName].Active)
                {
                    throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service already online."));
                }
                else
                {
                    DNSTable.dnsTable[serviceName].Active = true;
                    DNSTable.dnsTable[serviceName].StartedBy = username;
                    return true;
                }
            }
            else
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
        }
        

        // Set service to inactive
        public bool DeactivateService(string serviceName)
        {
            if (DNSTable.dnsTable.ContainsKey(serviceName))
            {
                if (!DNSTable.dnsTable[serviceName].Active)
                {
                    throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service already offline."));
                }
                else
                {
                    DNSTable.dnsTable[serviceName].Active = false;
                    DNSTable.dnsTable[serviceName].StartedBy = "";
                    return true;
                }
            }
            else
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
        }

        // checks if existing service is started
        public bool IsServiceOnline(string serviceName)
        { 
            if (DNSTable.dnsTable.ContainsKey(serviceName))
            {
                return DNSTable.dnsTable[serviceName].Active;
            }
            else
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
        }

        // generate session key
        public string GenerateSessionKey()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] key = new byte[24]; // For a 192-bit key
            rng.GetBytes(key);

            return Encoding.ASCII.GetString(key);
        }
    }
}
