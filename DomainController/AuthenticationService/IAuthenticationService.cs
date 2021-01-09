using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.AuthenticationService
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        // checking if user exists in the database
        // if user exists send a challenge
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short AuthenticateUser(string username);


        // after receiving response from the client/service
        // encrypt the sent challenge with the stored password hash
        // if the received response and the result are the same, user authentication is complete
        // Logs result
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool CheckPassword(UserRequest userRequest, byte[] response);
    }
}
