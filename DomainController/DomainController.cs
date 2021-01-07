using Contracts;
using Contracts.Cryptography;
using Contracts.Models;
using DomainController.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace DomainController
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class DomainController : IDomainControllerClient, IDomainControllerService
    {
        AuthProxy authProxy;
        TGSProxy tgsProxy;
        ServiceProxy serviceProxy;

        string key;

        // SendResponse: gets the response and then compares it to the stored hash in the database
        public ClientSessionData SendResponse(byte[] response)
        {
            string sessionId = OperationContext.Current.SessionId;
            if (!Database.usersRequestsDB.ContainsKey(sessionId))
            {
                throw new FaultException<SecurityException>(new SecurityException("Domain Controller: Session failed."));
            }

            bool authenticated = false;
            using (authProxy = new AuthProxy(new NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(Database.usersRequestsDB[sessionId], response);

                }
                catch (FaultException<SecurityException> ex)
                {
                    Console.WriteLine(ex.Detail.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Message));
                   
                }
            }

            string serviceAddress;
            string serviceName = Database.usersRequestsDB[sessionId].RequestedService;
            if (!authenticated)
                throw new FaultException<SecurityException>(new SecurityException("Authentication Service error: User failed to authenticate."));
            else
            {
                using (tgsProxy = new TGSProxy(new NetTcpBinding(), "net.tcp://localhost:10001/TGService"))
                {
                    try
                    {
                        serviceAddress = tgsProxy.GetServiceAddress(serviceName);
                        Console.WriteLine($"Ticket Granting Service: Requested {serviceName} found. Address: {serviceAddress}.");

                        if (!tgsProxy.IsServiceOnline(serviceName))
                        {
                            throw new FaultException<SecurityException>(new SecurityException($"Ticket Granting Service: {serviceName} is offline"));
                        }
                        Console.WriteLine($"Ticket Granting Service: {serviceAddress} is active.");

                        Console.WriteLine("Ticket Granting Service: Generating session key ...");
                        key = tgsProxy.GenerateSessionKey();
                        byte[] pwHash = Database.usersDB[Database.usersRequestsDB[sessionId].Username];
                        Database.usersRequestsDB[sessionId].SessionKey = _3DESAlgorithm.Encrypt(key, pwHash);

                    }
                    catch (FaultException<SecurityException> ex)
                    {
                        Console.WriteLine(ex.Detail.Message);
                        throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
            bool keySent = false;
            EndpointAddress endpointAddressService = new EndpointAddress(new Uri(serviceAddress), EndpointIdentity.CreateUpnIdentity(serviceName));

            using (serviceProxy = new ServiceProxy(new NetTcpBinding(), endpointAddressService))
            {
                try
                {
                    Console.WriteLine("Domain controller: Sending session key to the service...");
                    byte[] pwHash = Database.usersDB[Database.usersRequestsDB[sessionId].Username];
                    string user = Database.usersRequestsDB[sessionId].Username;
                    keySent = serviceProxy.SendSessionKey(user, _3DESAlgorithm.Encrypt(key, pwHash));
                }
                catch (FaultException<SecurityException> ex)
                {
                    Console.WriteLine(ex.Detail.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex.Message);
                    serviceProxy.Abort();
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }

            }

            if (keySent)
            {
                Console.WriteLine("Domain controller: Sending session key and service address to the client...");
                return new ClientSessionData(Database.usersRequestsDB[sessionId].SessionKey, serviceAddress);
            }
            else
            {
                using (tgsProxy = new TGSProxy(new NetTcpBinding(), "net.tcp://localhost:10001/TGService"))
                {
                    try
                    {
                        Console.WriteLine($"Domain controller: Deactivating {serviceName} service...");
                        tgsProxy.DeactivateService(serviceName);

                    }
                    catch (FaultException<SecurityException> ex)
                    {
                        Console.WriteLine(ex.Detail.Message);
                        throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception(ex.Message);
                    }

                }
            }
            throw new FaultException<SecurityException>(new SecurityException("Domain controller: Service not active."));
        }

        // client authentication
        // start challenge response protocol if username exists
        public short StartClientAuthentication(string service)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string sessionId = OperationContext.Current.SessionId;
            Database.usersRequestsDB.Add(sessionId, new UserRequest());
            //save requested service
            Database.usersRequestsDB[sessionId].RequestedService = service;
            Database.usersRequestsDB[sessionId].Username = Formatter.ParseName(windowsIdentity.Name);

            using (authProxy = new AuthProxy(new NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    Database.usersRequestsDB[sessionId].Challenge = authProxy.AuthenticateUser(Database.usersRequestsDB[sessionId].Username);
                    return Database.usersRequestsDB[sessionId].Challenge;
                }
                catch (FaultException<SecurityException> ex)
                {
                    Console.WriteLine(ex.Detail.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Message));

                }
            }
        }

        // service authentication
        // challenge response protocol
        // check if service account exists
        // return challenge if exists
        public short StartServiceAuthentication(string serviceName)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string sessionId = OperationContext.Current.SessionId;
            Database.usersRequestsDB.Add(sessionId, new UserRequest());
            Database.usersRequestsDB[sessionId].Username = Formatter.ParseName(windowsIdentity.Name);

            Database.usersRequestsDB[sessionId].RequestedService = serviceName;
            using (authProxy = new AuthProxy(new NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    Database.usersRequestsDB[sessionId].Challenge = authProxy.AuthenticateUser(Database.usersRequestsDB[sessionId].Username);
                    return Database.usersRequestsDB[sessionId].Challenge;
                }
                catch (FaultException<SecurityException> ex)
                {
                    Console.WriteLine(ex.Detail.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Message));

                }
            }
        }


        // service sends an encrypted response 
        // DC forwards it to AS for password validation
        // after confirmation, forward to TGS 
        public bool SendResponseService(byte[] response)
        {
            string sessionId = OperationContext.Current.SessionId;
            UserRequest userRequest;
            if (!Database.usersRequestsDB.TryGetValue(sessionId, out userRequest))
            {
                throw new FaultException<SecurityException>(new SecurityException("Domain Controller: Session failed."));
            }

            bool authenticated = false;
            using (authProxy = new AuthProxy(new NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(userRequest, response);

                }
                catch (FaultException<SecurityException> ex)
                {
                    Console.WriteLine(ex.Detail.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new FaultException<SecurityException>(new SecurityException(ex.Message));

                }
            }

            
            if (!authenticated)
                throw new FaultException<SecurityException>(new SecurityException("Authentication Service error: Service failed to authenticate."));
            else
            {
                using (tgsProxy = new TGSProxy(new NetTcpBinding(), "net.tcp://localhost:10001/TGService"))
                {
                    try
                    {
                        string serviceAddress = tgsProxy.GetServiceAddress(userRequest.RequestedService);
                        Console.WriteLine($"Ticket Granting Service: Requested {userRequest.RequestedService} found. Address: {serviceAddress}.");

                        if(tgsProxy.IsServiceOnline(userRequest.RequestedService))
                        {
                            return true;
                        }

                        tgsProxy.ActivateService(userRequest.RequestedService);
                        Console.WriteLine($"Ticket Granting Service: {userRequest.RequestedService} activated.");

                        Console.WriteLine($"Ticket Granting Service: Sending confirmation to {serviceAddress}...");
                        return true;
                    }
                    catch (FaultException<SecurityException> ex)
                    {
                        Console.WriteLine(ex.Detail.Message);
                        throw new FaultException<SecurityException>(new SecurityException(ex.Detail.Message));

                    }
                    catch(CommunicationException ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new FaultException<SecurityException>(new SecurityException(ex.Message));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception(ex.Message);
                    }
                }
            }
        }


    }
}
