using Contracts;
using DomainController.AuthenticationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.Proxy
{
    public class AuthProxy : ChannelFactory<IAuthenticationService>, IAuthenticationService, IDisposable
    {
        IAuthenticationService factory;

        public AuthProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        // checking if user exists in the database
        //if exists send back a challenge to confirm the password
        public short AuthenticateUser(string username)
        {
            try
            {
                short challenge = factory.AuthenticateUser(username);
                return challenge;
            }
            catch (FaultException<SecurityException> e)
            {
               
                throw new FaultException<SecurityException>(new SecurityException(e.Detail.Message));
                
            }
            catch (Exception e)
            {
               
                throw new FaultException<SecurityException>(new SecurityException(e.Message));
            }
        }

        // after receiving response from the client/service
        // encrypt the sent challenge with the stored password hash
        // if the received response and the result are the same, user authentication is complete
        // Logs result
        public bool CheckPassword(UserRequest userRequest, byte[] response)
        {
            try
            {
                return factory.CheckPassword(userRequest, response);
            }
            catch (FaultException<SecurityException> e)
            {
               
                throw new FaultException<SecurityException>(new SecurityException(e.Detail.Message));

            }
            catch (Exception e)
            {         
                throw new FaultException<SecurityException>(new SecurityException(e.Message));
            }
        }
    }
}
