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
            byte[] key;

            string username;
            string password;
            Console.WriteLine("Enter username:");
            username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            password = Console.ReadLine();

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {

                    short challenge = proxy.startAuthetication(username, "DataManagementService");

                    key = sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));

                    byte[] response = _3DESAlgorithm.Encrypt(challenge.ToString(), key);

                    //sessionTuple =  session key, address of the requested service
                    sessionTuple = proxy.SendResponse(response);
                    Console.WriteLine($"Found service address: {sessionTuple.Item2}");


                }

                NetTcpBinding bindingService = new NetTcpBinding();
                using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionTuple.Item2))
                {

                    string data = "test";
                    byte[] decryptedKey = _3DESAlgorithm.Decrypt(sessionTuple.Item1, key);
                    Console.WriteLine(BitConverter.ToString(decryptedKey));
                    byte[] encryptedData = _3DESAlgorithm.Encrypt(data, decryptedKey);
                    proxy.Read(encryptedData);
                    proxy.Write(encryptedData);

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
