using Contracts;
using Contracts.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataManagement : ServiceData, IDataManagement, IDataManagementDC
    {
        private static Dictionary<string, byte[]> userSessions = new Dictionary<string, byte[]>();
        private SHA256 sha256Hash = SHA256.Create();

        // decrypt request
        // read data from database
        // encrypt read values and send back to the client
        public byte[] Read(byte[] encryptedKey)
        {
            Console.WriteLine("Received encrypted READ request");

            string[] identityData = ServiceSecurityContext.Current.WindowsIdentity.Name.Split('\\');
            if(identityData.Count() != 2)
            {
                throw new Exception("Service error: Unable to parse username.");
            }

            string user = identityData[1];

            Console.WriteLine("User requesting the service: ");
            Console.WriteLine(user);

            byte[] sessionKey = GetSessionKey(user);

            string key = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedKey, sessionKey)).Trim('\0');
            string value = Database.Read(key);

            byte[] encryptedValue = _3DESAlgorithm.Encrypt(value, sessionKey);

            return encryptedValue;
        }

        private byte[] GetSessionKey(string user)
        {
            if (userSessions.ContainsKey(user))
            {
                return userSessions[user];
            }
            else
            {
                throw new FaultException<SecurityException>(new SecurityException("Security error: Client-Service session not established."));
            }
        }

        // decrypt request
        // write data to the database
        public bool Write(byte[] encryptedKey, byte[] encryptedValue)
        {
            Console.WriteLine("Received encrypted WRITE request");

            string[] identityData = ServiceSecurityContext.Current.WindowsIdentity.Name.Split('\\');
            if (identityData.Count() != 2)
            {
                throw new Exception("Service error: Unable to parse username.");
            }

            string user = identityData[1];

            Console.WriteLine("User requesting the service: ");
            Console.WriteLine(user);

            byte[] sessionKey = GetSessionKey(user);

            string key = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedKey, sessionKey)).Trim('\0');
            string value = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedValue, sessionKey)).Trim('\0');

            Database.Write(key, value);

            return true;
        }


        // Domain Controller sends session key after a client requested the service
        // TODO: Set service to offline if it disconnects
        public bool SendSessionKey(string user, byte[] encryptedSessionKey)
        {
            Console.WriteLine("Received encrypted session key");

            if(serviceSecret == null)
            {
                throw new Exception("Service error: service secret not set.");
            }

            byte[] sessionKey = _3DESAlgorithm.Decrypt(encryptedSessionKey, serviceSecret);
            userSessions.Add(user, sessionKey);

            return true;
        }
    }
}
