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

        public bool Read(byte[] encryptedData)
        {
            try
            {
                return factory.Read(encryptedData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return false;
            }
        }

        public bool Write(byte[] encryptedData)
        {
            try
            {
                return factory.Write(encryptedData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return false;
            }
        }


    }
}
