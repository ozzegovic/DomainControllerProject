using Contracts;
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
            serviceHost.Open();

            Console.WriteLine("Data management service started...");

            SHA256 sha256Hash = SHA256.Create();

            // connection with domain controller
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/DomainControllerService";
            byte[] token = null;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {

                    short challenge = proxy.startAuthetication("DataManagementService");
                    byte[] passwordHash = sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes("pass"));

                    byte[] response = _3DESAlgorithm.Encrypt(challenge.ToString(), passwordHash);

                    bool sucess = proxy.SendResponse(response);
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
