using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class DNSActiveServices
    {
        // dnsActiveServices contains all the started services

        // TO DO: Update the dictionary, save service identity instead bool value
        public static Dictionary<string, bool> dnsActiveServices = new Dictionary<string, bool>();


    }
}
