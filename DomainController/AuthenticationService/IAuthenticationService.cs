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
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        short AuthenticateUser(string username);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool CheckPassword(UserRequest userRequest, byte[] response);
    }
}
