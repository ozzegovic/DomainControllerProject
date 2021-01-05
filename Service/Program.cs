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
            string address = $"net.tcp://localhost:9999/DomainControllerService";

            string username = Environment.UserName;
            Console.WriteLine("Logging in as " + username);

            Console.WriteLine("Enter password");
            string password = Console.ReadLine();

            byte[] pwBytes = Encoding.ASCII.GetBytes(password);
            ChallengeResponse cr = new ChallengeResponse();

            string addressClient = string.Empty;

            try
            {
                using (DCProxy proxy = new DCProxy(binding, address))
                {
                    serviceSecret = sha256Hash.ComputeHash(pwBytes);
                    short salt = proxy.startAuthetication(username);
                    byte[] response = cr.Encrypt(serviceSecret, salt);

                    addressClient = proxy.SendResponseService(response);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (addressClient != string.Empty)
            {
                Database.Load(username);
            }
            else
            {
                Console.WriteLine("Failed to connect service to Domain Controller.");
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
                return;
            }

            // connection with the client
            NetTcpBinding bindingClient = new NetTcpBinding();
            ServiceHost serviceHost = new ServiceHost(typeof(DataManagement));
            serviceHost.AddServiceEndpoint(typeof(IDataManagement), bindingClient, addressClient);
            serviceHost.AddServiceEndpoint(typeof(IDataManagementDC), bindingClient, addressClient);
            serviceHost.Open();

            Console.WriteLine(username + " started...");

            Console.ReadLine();

            Database.Save(username);

            Console.WriteLine("Closing...");
            Thread.Sleep(2000);
        }
    }
    
}
