using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            // connection with the client
            NetTcpBinding bindingClient = new NetTcpBinding();
            string addressClient = "net.tcp://localhost:9998/DataManagementService";

            ServiceHost serviceHost = new ServiceHost(typeof(DataManagement));
            serviceHost.AddServiceEndpoint(typeof(IDataManagement), bindingClient, addressClient);
            serviceHost.Open();

            Console.WriteLine("Data management service started...");


            // connection with domain controller
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/DomainControllerService";
            byte[] token = null;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {

                    short challenge = proxy.startAuthetication(addressClient);
                    //token = proxy.SendResponse(challenge.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();

        }
    }
    
}
