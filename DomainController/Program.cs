using Contracts;
using DomainController.AuthenticationService;
using DomainController.TicketGrantingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    class Program
    {
        static void Main(string[] args)
        {
            // communication protocol
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            string address = "net.tcp://localhost:9999/DomainControllerClient";
            string addressService = "net.tcp://localhost:9999/DomainControllerService";

            ServiceHost serviceHost = new ServiceHost(typeof(DomainController));
            serviceHost.AddServiceEndpoint(typeof(IDomainControllerClient), binding, address);
            serviceHost.AddServiceEndpoint(typeof(IDomainControllerService), binding, addressService);

            serviceHost.Open();

            //Authentication Service Hosts
            ServiceHost ASHost = new ServiceHost(typeof(AuthService));
            ASHost.AddServiceEndpoint(typeof(IAuthenticationService), binding, "net.tcp://localhost:10000/AuthService");
            ASHost.Open();

            //Ticket granting service Hosts
            ServiceHost TGSHost = new ServiceHost(typeof(TGService));
            TGSHost.AddServiceEndpoint(typeof(ITicketGrantingService), binding, "net.tcp://localhost:10001/TGService");
            TGSHost.Open();

            Console.WriteLine("Server domain controller client started...");
            Console.WriteLine("Server domain controller service started...");
            Console.ReadLine();
        }
    }
}
