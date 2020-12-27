using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class DNSTable
    {
        // dnsTable contains all the supported services
        // dnsActiveServices contains all the started services

        // TO DO: change dns table to IPAddress:Hostname
        // TO DO: add service to dnsActiveServices after service authentication
        internal static Dictionary<string, string> dnsTable = new Dictionary<string, string>();

        static DNSTable()
        {
            dnsTable.Add("DataManagementService", "net.tcp://localhost:9998");
        }
    }
}
