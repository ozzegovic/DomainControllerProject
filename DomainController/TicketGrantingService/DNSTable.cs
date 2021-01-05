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
        internal static Dictionary<string, Service> dnsTable = new Dictionary<string, Service>();

        static DNSTable()
        {
            string name0 = "wcfService";
            string name1 = "wcfService1";

            dnsTable.Add(name0, new Service(name0, "net.tcp://localhost:9900"));
            dnsTable.Add(name1, new Service(name1, "net.tcp://localhost:9901"));
        }
    }
}
