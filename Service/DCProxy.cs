using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DCProxy :ChannelFactory<IDomainControllerService>// IDomainControllerService
    {
        IDomainControllerService factory;

        public DCProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public DCProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public short StartServiceAuthentication(string serviceName)
        {
            try
            {
                return factory.StartServiceAuthentication(serviceName);
            }
            catch (FaultException<SecurityException> e)
            {
                throw new Exception(e.Detail.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
          
        }

        public string SendResponseService(byte[] response)
        {
            try
            {
                return factory.SendResponseService(response);
            }
            catch (FaultException<SecurityException> e)
            {
                throw new Exception(e.Detail.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
