using Contracts;
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

        public short startAuthetication(string username, string service)
        {
            try
            {
                return factory.startAuthetication(username, service);
            }

            catch (FaultException<SecurityException> e)
            {
               
                throw new Exception("Failed:" + e.Detail.Message);
            }
            catch (Exception e)
            {
               
                throw new Exception("Failed:" + e.Message);
            }
           
        }

        public Tuple<byte[],string> SendResponse(string username, byte[] response)
        {
            try
            {
                return factory.SendResponse(username, response);
            }
            catch (FaultException<SecurityException> e)
            {
                
                throw new Exception("Failed:" + e.Detail.Message);
            }
            catch (Exception e)
            {
               
                throw new Exception("Failed:" + e.Message);
            }
            
        }


    }
}
