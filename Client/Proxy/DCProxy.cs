using Contracts;
using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Proxy
{
    public class DCProxy : ChannelFactory<IDomainControllerClient>, IDomainControllerClient, IDisposable
    {
        IDomainControllerClient factory;

        public DCProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public DCProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {

            factory = this.CreateChannel();
        }
        public short StartClientAuthentication(string service)
        {
            try
            {
                return factory.StartClientAuthentication(service);
            }

            catch (FaultException<SecurityException> e)
            {
               
                throw new Exception("Failed: " + e.Detail.Message);
            }
            catch (Exception e)
            {
               
                throw new Exception("Failed: " + e.Message);
            }
           
        }

        public ClientSessionData SendResponse(byte[] response)
        {
            try
            {
                return factory.SendResponse(response);
            }
            catch (FaultException<SecurityException> e)
            {
                
                throw new Exception("Failed: " + e.Detail.Message);
            }
            catch (Exception e)
            {
               
                throw new Exception("Failed: " + e.Message);
            }
            
        }


    }
}
