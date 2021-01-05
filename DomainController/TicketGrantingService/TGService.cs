﻿using Contracts;
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
        public string GetServiceAddress(string serviceName)
        {
            if (!DNSTable.dnsTable.ContainsKey(serviceName))
            {
                throw new FaultException<SecurityException>(new SecurityException("Ticket Granting Service: Service not found."));
            }
            else
                return DNSTable.dnsTable[serviceName].Address + "/" + serviceName;
        }

        // Set service to active
        public bool ActivateService(string serviceName)
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

        public string GenerateSessionKey()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] key = new byte[24]; // For a 192-bit key
            rng.GetBytes(key);

            return Encoding.ASCII.GetString(key);
        }
    }
}
