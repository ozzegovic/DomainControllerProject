using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.TicketGrantingService
{
    public class Service
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }

        public Service(string name, string addr)
        {
            Name = name;
            Address = addr;
            Active = false;
        }
    }
}
