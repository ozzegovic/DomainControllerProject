using Contracts;
using Contracts.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            serviceHost.AddServiceEndpoint(typeof(IDataManagementDC), bindingClient, addressClient);
            serviceHost.Open();

            Console.WriteLine("Data management service started...");

            SHA256 sha256Hash = SHA256.Create();

            // connection with domain controller
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/DomainControllerService";
            byte[] token = null;

            string username = "DataManagementService";
            string password = "pass";
            byte[] pwBytes = Encoding.ASCII.GetBytes(password);
            ChallengeResponse cr = new ChallengeResponse();

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {
                    byte[] key = sha256Hash.ComputeHash(pwBytes);
                    short salt = proxy.startAuthetication(username);
                    byte[] response = cr.Encrypt(key, salt);

                    bool success = proxy.SendResponseService(response);
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
