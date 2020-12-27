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


        public short startAuthetication(string serviceName)
        {
            try
            {
                return factory.startAuthetication(serviceName);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
          
        }
        //public byte[] SendResponse(string username, byte[] response)
        //{
        //    try
        //    {
        //         factory.SendResponse(response);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e.Message);
        //    }
        //    return null;
        //}
    }
}
