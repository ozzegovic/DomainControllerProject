﻿using Contracts;
using Contracts.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    class Program : ServiceData
    {
        static void Main(string[] args)
        {
            SHA256 sha256Hash = SHA256.Create();

            // connection with domain controller
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            string address = "net.tcp://localhost:9999/DomainControllerService";
            EndpointAddress endpointAddressDC = new EndpointAddress(new Uri(address), EndpointIdentity.CreateUpnIdentity("DomainController"));


            string username = Environment.UserName;
            Console.WriteLine("Logging in as " + username +"\n");

            Console.WriteLine("Enter password");
            string password = Console.ReadLine();

            Console.WriteLine("Enter service name:"); //which service to start
            string serviceName = Console.ReadLine();

            // this is only needed for testing since every service instance is run on localhost
            // in production every service instance should have it's unique address
            short port;
            switch (serviceName)
            {
                case "wcfService":
                    port = 9900;
                    break;
                case "wcfService1":
                    port = 9901;
                    break;
                default:
                    port = 9902;
                    break;
            }

            string addressClient = $"net.tcp://localhost:{port}/" + serviceName;


            byte[] pwBytes = Encoding.ASCII.GetBytes(password);
            ChallengeResponse cr = new ChallengeResponse();

            bool successful = false;

            using (DCProxy proxy = new DCProxy(binding, endpointAddressDC))
            {
                try
                {
                    serviceSecret = sha256Hash.ComputeHash(pwBytes);
                    short salt = proxy.StartServiceAuthentication(serviceName);
                    byte[] response = cr.Encrypt(serviceSecret, salt);

                    successful = proxy.SendResponseService(response); // if not successful it will throw an exception 
                    Database.Load(username);
                    Console.WriteLine();
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press return to exit.");
                    Console.ReadLine();
                    return;
                }

            }
           


            Console.WriteLine("Starting service...");

            // connection with the client and the Domain Controller
            NetTcpBinding bindingClient = new NetTcpBinding();
            bindingClient.Security.Mode = SecurityMode.Transport;
            bindingClient.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingClient.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost serviceHost = new ServiceHost(typeof(DataManagement));
            serviceHost.AddServiceEndpoint(typeof(IDataManagement), bindingClient, addressClient);

            //for receiving the session key
            serviceHost.AddServiceEndpoint(typeof(IDataManagementDC), bindingClient, addressClient);
            
            try
            {
                serviceHost.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start service. Error: {0}", e.Message);
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Service {serviceName} started. Press return to exit.");

            Console.ReadLine();

            Console.WriteLine("Closing...");

            Database.Save(username);
            serviceHost.Close();
            Thread.Sleep(2000);
        }
    }
    
}
