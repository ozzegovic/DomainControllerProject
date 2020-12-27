using Client.Proxy;
using Contracts;
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
            Tuple<byte[], string> sessionTuple;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {

                    short challenge = proxy.startAuthetication("username1", "DataManagementService");

                    byte[] key = sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes("password1"));

                    byte[] response = _3DESAlgorithm.Encrypt(challenge.ToString(), key);

                    //sessionTuple =  session key, address of the requested service
                    sessionTuple = proxy.SendResponse("username1", response);
                    Console.WriteLine($"Found service address: {sessionTuple.Item2}");


                }

                NetTcpBinding bindingService = new NetTcpBinding();
                using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionTuple.Item2))
                {
                    
                    proxy.Read(sessionTuple.Item1);
                    proxy.Write(sessionTuple.Item1);

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
