using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    public class ServiceProxy : ChannelFactory<IDataManagementDC>, IDataManagementDC, IDisposable
    {
        // authenticated client requested a DataManagement service (service is active)

        IDataManagementDC factory;

        public ServiceProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public ServiceProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public bool SendSessionKey(string user, byte[] encryptedSessionKey)
        {

            try
            {
                factory.SendSessionKey(user, encryptedSessionKey);
                return true;
            }
            catch (FaultException<SecurityException> e)
            {

                throw new FaultException<SecurityException>(new SecurityException(e.Detail.Message));

            }
            catch (CommunicationException e)
            {

                throw new CommunicationException("Could not connect to service.");
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
