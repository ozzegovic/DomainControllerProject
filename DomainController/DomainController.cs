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

        // service authentication
        // challenge response protocol
        public short startAuthetication(string serviceName)
        {
            short challenge =  23;
            return challenge;
        }


    }
}
