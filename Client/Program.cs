using Client.Proxy;
using Contracts;
using Contracts.Cryptography;
using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //string addressService = "net.tcp://localhost:9998/Service";

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/DomainControllerClient";
            byte[] token = null;
            SHA256 sha256Hash = SHA256.Create();
            ChallengeResponse cr = new ChallengeResponse();
            ClientSessionData sessionData;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {
                    string key = sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes("password1")).ToString();
                    short salt = proxy.startAuthetication("username1", "DataManagementService");
                    byte[] response = cr.Encrypt(key, salt);

                    sessionData = proxy.SendResponse("username1", response);
                    Console.WriteLine($"Found service address: {sessionData.ServiceAddress}");
                }


                NetTcpBinding bindingService = new NetTcpBinding();
                using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionData.ServiceAddress))
                {
                    proxy.Read(sessionData.SessionKey);
                    proxy.Write(sessionData.SessionKey);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
