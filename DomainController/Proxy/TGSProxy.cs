using DomainController.TicketGrantingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.Proxy
{
    public class TGSProxy : ChannelFactory<ITicketGrantingService>, ITicketGrantingService, IDisposable
    {
        ITicketGrantingService factory;

        public TGSProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        // currently: checks if the service exists in the dnsTable and returns the full address
        // Client then uses this address to connect to the service
        public string ServiceExists(string serviceAddress)
        {
            try
            {
                return factory.ServiceExists(serviceAddress);
            }
            catch(Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
