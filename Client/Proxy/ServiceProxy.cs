using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Proxy
{
    public class ServiceProxy : ChannelFactory<IDataManagement>, IDataManagement, IDisposable
    {
        IDataManagement factory;

        public ServiceProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public byte[] Read(byte[] key)
        {
            try
            {
                return factory.Read(key);
            }
            catch (FaultException<DataException> e)
            {
                Console.WriteLine("Data Error: {0}", e.Detail.Message);
                return null;
            }
            catch (CommunicationException e)
            {
                throw new Exception($"Communication error: {e.Message}");
            }

            catch (Exception e)
            {
                throw new Exception($"Error: {e.Message}");
            }
        }

        public bool Write(byte[] key, byte[] value)
        {
            try
            {
                return factory.Write(key, value);
            }
            catch (FaultException<DataException> e)
            {
                Console.WriteLine("Data Error: {0}", e.Detail.Message);
                return false;
            }
            catch (CommunicationException e)
            {
                throw new Exception($"Communication error: {e.Message}");
            }

            catch (Exception e)
            {
                throw new Exception($"Error: {e.Message}");
            }
        }


    }
}
