using Contracts;
using DomainController.AuthenticationService;
using DomainController.TicketGrantingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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

            serviceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            serviceHost.Open();

            //Authentication Service Audit
            ServiceSecurityAuditBehavior asAudit = new ServiceSecurityAuditBehavior();
            asAudit.AuditLogLocation = AuditLogLocation.Application;
            asAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            //Authentication Service Hosts
            ServiceHost ASHost = new ServiceHost(typeof(AuthService));
            ASHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            ASHost.Description.Behaviors.Add(asAudit);
            ASHost.AddServiceEndpoint(typeof(IAuthenticationService), binding, "net.tcp://localhost:10000/AuthService");

            ASHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            ASHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            ASHost.Open();

            //Ticket Granting Service Audit
            ServiceSecurityAuditBehavior tgsAudit = new ServiceSecurityAuditBehavior();
            tgsAudit.AuditLogLocation = AuditLogLocation.Application;
            tgsAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            //Ticket granting service Hosts
            ServiceHost TGSHost = new ServiceHost(typeof(TGService));
            TGSHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            TGSHost.Description.Behaviors.Add(tgsAudit);
            TGSHost.AddServiceEndpoint(typeof(ITicketGrantingService), binding, "net.tcp://localhost:10001/TGService");

            TGSHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            TGSHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            TGSHost.Open();

            Console.WriteLine("Server domain controller client started...");
            Console.WriteLine("Server domain controller service started...");
            Console.ReadLine();

            ASHost.Close();
            TGSHost.Close();
            serviceHost.Close();
        }
    }
}
