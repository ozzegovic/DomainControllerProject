using Client.Proxy;
using Contracts;
using Contracts.Cryptography;
using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/DomainControllerClient";
            SHA256 sha256Hash = SHA256.Create();
            ChallengeResponse cr = new ChallengeResponse();
            ClientSessionData sessionData;

            string username = Environment.UserName;
            string password;
            Console.WriteLine("Logging in as " + username);

            Console.WriteLine("Enter service name: ");
            string serviceName = Console.ReadLine();

            Console.WriteLine("Enter password:");
            password = Console.ReadLine();

            byte[] pwBytes = Encoding.ASCII.GetBytes(password);
            byte[] secret;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {
                    secret = sha256Hash.ComputeHash(pwBytes);

                    short salt = proxy.startAuthetication(username, serviceName);
                    byte[] response = cr.Encrypt(secret, salt);

                    sessionData = proxy.SendResponse(response);
                    Console.WriteLine($"Found service address: {sessionData.ServiceAddress}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
                return;
            }

            NetTcpBinding bindingService = new NetTcpBinding();

            // TODO 
            // put in while loop with simple menu (write, read, exit)
            using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionData.ServiceAddress))
            {
                Console.WriteLine("Write key: ");
                string key = Console.ReadLine();
                Console.WriteLine("Write value: ");
                string value = Console.ReadLine();

                byte[] sessionKey = _3DESAlgorithm.Decrypt(sessionData.SessionKey, secret);

                byte[] encryptedKey = _3DESAlgorithm.Encrypt(key, sessionKey);
                byte[] encryptedValue = _3DESAlgorithm.Encrypt(value, sessionKey);

                proxy.Write(encryptedKey, encryptedValue);

                byte[] val = proxy.Read(encryptedKey);
                Console.WriteLine("Encrypted: " + Encoding.ASCII.GetString(val));
                Console.WriteLine("Decrypted: " + Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(val, sessionKey)));
            }

            Console.ReadLine();
        }
    }
}
