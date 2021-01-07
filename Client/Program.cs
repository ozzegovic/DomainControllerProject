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
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            EndpointAddress endpointAddressDC = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("DomainController"));

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
                using (DCProxy proxy = new DCProxy(binding, endpointAddressDC))
                {
                    secret = sha256Hash.ComputeHash(pwBytes);

                    short salt = proxy.StartClientAuthentication(serviceName);
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

            try
            {
                using (ServiceProxy proxy = new ServiceProxy(bindingService, sessionData.ServiceAddress))
                {
                    char c;
                    string key;
                    byte[] encryptedKey;
                    string value;
                    byte[] encryptedValue;
                    byte[] sessionKey = _3DESAlgorithm.Decrypt(sessionData.SessionKey, secret);

                    do
                    {
                        Console.WriteLine();
                        Console.WriteLine("Choose an action:");
                        Console.WriteLine("\t- 'w' to write");
                        Console.WriteLine("\t- 'r' to read");
                        Console.WriteLine("\t- 'x' to exit");
                        c = char.ToLower(Console.ReadKey().KeyChar);
                        Console.WriteLine();

                        switch (c)
                        {
                            case 'r':
                                Console.WriteLine("Key of value to read:");
                                key = Console.ReadLine();

                                Console.WriteLine("Encrypting and sending READ request...");
                                encryptedKey = _3DESAlgorithm.Encrypt(key, sessionKey);
                                encryptedValue = proxy.Read(encryptedKey);
                                if (encryptedValue == null)
                                {
                                    break;
                                }
                                value = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedValue, sessionKey));
                                Console.WriteLine("Received value: " + value);
                                break;

                            case 'w':
                                Console.WriteLine("Key of value to write:");
                                key = Console.ReadLine();
                                Console.WriteLine("Value to write:");
                                value = Console.ReadLine();

                                Console.WriteLine("Encrypting and sending WRITE request...");
                                encryptedKey = _3DESAlgorithm.Encrypt(key, sessionKey);
                                encryptedValue = _3DESAlgorithm.Encrypt(value, sessionKey);
                                if (proxy.Write(encryptedKey, encryptedValue))
                                {
                                    Console.WriteLine("Value written successfully.");
                                }
                                break;

                            default:
                                break;
                        }

                    } while (c != 'x');
                }
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Communication error. Please restart. ");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine("Please restart.");
                Console.ReadLine();
            }

        }
    }
}
