using Contracts;
using DomainController.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class DomainController : IDomainControllerClient, IDomainControllerService
    {
        AuthProxy authProxy;
        TGSProxy tgsProxy;
        ServiceProxy serviceProxy;

        string serviceFound;

        byte[] sessionKey;
        string key;

        // SendResponse: gets the response and then compares it to the stored hash in the database
        public Tuple<byte[], string> SendResponse(byte[] response)
        {
            string sessionId = OperationContext.Current.SessionId;
            if (!Database.usersRequestsDB.ContainsKey(sessionId))
            {
                throw new FaultException<SecurityException>(new SecurityException("Domain Controller: Session failed."));
            }

            bool authenticated = false;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(Database.usersRequestsDB[sessionId].Username,
                        Database.usersRequestsDB[sessionId].Challenge, response);

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
                throw new FaultException<SecurityException>(new SecurityException("Authentication Service error: User failed to authenticate."));
            else
            {
                using (tgsProxy = new TGSProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10001/TGService"))
                {
                    try
                    {
                        serviceFound = tgsProxy.ServiceExists(Database.usersRequestsDB[sessionId].RequestedService);
                        Console.WriteLine($"Ticket Granting Service: Requested {Database.usersRequestsDB[sessionId].RequestedService} found. Details: {serviceFound}.");

                        serviceFound = tgsProxy.CheckOnlineService(serviceFound);
                        Console.WriteLine($"Ticket Granting Service: {serviceFound} is active.");

                        Console.WriteLine("Ticket Granting Service: Generating session key ...");
                        key = tgsProxy.GenerateSessionKey();
                        Database.usersRequestsDB[sessionId].SessionKey = _3DESAlgorithm.Encrypt(key, Database.usersDB[Database.usersRequestsDB[sessionId].Username]);

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

            using (serviceProxy = new ServiceProxy(new NetTcpBinding(), serviceFound))
            {
                try
                {
                    Console.WriteLine("Domain controller: Sending session key to the service...");
                    serviceProxy.SendSessionKey(_3DESAlgorithm.Encrypt(key, Database.usersDB[Database.usersRequestsDB[sessionId].RequestedService]));
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
            Console.WriteLine("Domain controller: Sending session key and service address to the client...");
            return new Tuple<byte[], string>(Database.usersRequestsDB[sessionId].SessionKey, serviceFound);

        }

        // client authentication
        // start challenge response protocol if username exists
        public short startAuthetication(string username, string service)
        {

            string sessionId = OperationContext.Current.SessionId;
            Database.usersRequestsDB.Add(sessionId, new UserRequest());
            //save requested service
            Database.usersRequestsDB[sessionId].RequestedService = service;
            Database.usersRequestsDB[sessionId].Username = username;

            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    Database.usersRequestsDB[sessionId].Challenge = authProxy.AuthenticateUser(username);
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
        public short startAuthetication(string serviceName)
        {
            string sessionId = OperationContext.Current.SessionId;
            Database.usersRequestsDB.Add(sessionId, new UserRequest());
            Database.usersRequestsDB[sessionId].Username = serviceName;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
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
            if (!Database.usersRequestsDB.ContainsKey(sessionId))
            {
                throw new FaultException<SecurityException>(new SecurityException("Domain Controller: Session failed."));
            }

            bool authenticated = false;
            string svcFound;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(Database.usersRequestsDB[sessionId].Username,
                        Database.usersRequestsDB[sessionId].Challenge, response);

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
                using (tgsProxy = new TGSProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10001/TGService"))
                {
                    try
                    {
                        svcFound = tgsProxy.ServiceExists(Database.usersRequestsDB[sessionId].Username);
                        Console.WriteLine($"Ticket Granting Service: Requested {Database.usersRequestsDB[sessionId].Username} found. Details: {svcFound}.");

                        tgsProxy.AddOnlineService(svcFound);
                        Console.WriteLine($"Ticket Granting Service: {svcFound} added to active services list.");

                        // return authentication confirmation
                        Console.WriteLine($"Ticket Granting Service: Sending confirmation to {svcFound}...");
                        return true;
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
        }


    }
}
