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
    public class DomainController : IDomainControllerClient, IDomainControllerService
    {
        AuthProxy authProxy;
        TGSProxy tgsProxy;
        string requestedService;
        string serviceFound;
        // TO DO: generate session key after authentication and service check
        byte[] sessionKey;

        // SendResponse: gets the response and then compares it to the stored hash in the database
        public Tuple<byte[], string> SendResponse(string username, byte[] response)
        {
            bool authenticated = false;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(username, response);
                  
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
                        serviceFound = tgsProxy.ServiceExists(requestedService);
                        Console.WriteLine($"Ticket Granting Service: Requested {requestedService} found. Details: {serviceFound}.");

                        serviceFound = tgsProxy.CheckOnlineService(serviceFound);
                        Console.WriteLine($"Ticket Granting Service: {serviceFound} is active.");

                        Console.WriteLine("Ticket Granting Service: Sending session key and service address to the client...");
                        // return all info to the client
                        return new Tuple<byte[], string>(sessionKey, serviceFound);
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

        // client authentication
        // start challenge response protocol if username exists
        public short startAuthetication(string username, string service)
        {
            //save requested service
            requestedService = service;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    short challenge = authProxy.AuthenticateUser(username);
                    return challenge;
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
        string serviceUser;
        // service authentication
        // challenge response protocol
        // check if service account exists
        // return challenge if exists
        public short startAuthetication(string serviceName)
        {
            serviceUser = serviceName;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    short challenge = authProxy.AuthenticateUser(serviceName);
                    return challenge;
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
        public bool SendResponse(byte[] response)
        {
            bool authenticated = false;
            string svcFound;
            using (authProxy = new AuthProxy(new System.ServiceModel.NetTcpBinding(), "net.tcp://localhost:10000/AuthService"))
            {
                try
                {
                    authenticated = authProxy.CheckPassword(serviceUser, response);

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
                        svcFound = tgsProxy.ServiceExists(serviceUser);
                        Console.WriteLine($"Ticket Granting Service: Requested {serviceUser} found. Details: {svcFound}.");
                        Console.WriteLine($"Ticket Granting Service: Sending confirmation to {serviceUser}...");

                        tgsProxy.AddOnlineService(svcFound);
                        Console.WriteLine($"Ticket Granting Service: {svcFound} added to active services list.");
                        // return authentication confirmation
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
