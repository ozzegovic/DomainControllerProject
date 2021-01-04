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

            string username;
            string password;
            Console.WriteLine("Enter username:");
            username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            password = Console.ReadLine();
            byte[] pwBytes = Encoding.ASCII.GetBytes(password);
            byte[] key;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {
                    key = sha256Hash.ComputeHash(pwBytes);
                    short salt = proxy.startAuthetication(username, "DataManagementService");
                    byte[] response = cr.Encrypt(key, salt);

                    sessionData = proxy.SendResponse(response);
                    Console.WriteLine($"Found service address: {sessionData.ServiceAddress}");
                }


                NetTcpBinding bindingService = new NetTcpBinding();
                using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionData.ServiceAddress))
                {

                    string data = "test";
                    byte[] decryptedKey = _3DESAlgorithm.Decrypt(sessionData.SessionKey, key);
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
