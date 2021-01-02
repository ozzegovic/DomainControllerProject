using Contracts;
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
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        // after service authentication, add it to the active services list
        public bool AddOnlineService(string serviceAddress)
        {
            try
            {
                return factory.AddOnlineService(serviceAddress);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        //check if requested service is active
        public string CheckOnlineService(string serviceAddress)
        {
            try
            {
                return factory.CheckOnlineService(serviceAddress);
            }
            catch (FaultException<SecurityException> ex)
            {

                throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
