using Contracts;
using Contracts.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
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
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string user = Formatter.ParseName(windowsIdentity.Name);

            Console.WriteLine("User requesting the service: ");
            Console.WriteLine(user);

            byte[] sessionKey = GetSessionKey(user);
            string key;
            try
            {
                key = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedKey, sessionKey)).Trim('\0');
            }
            catch (Exception e)
            {
                Console.WriteLine("Data Error: {0}", e.Message);
                throw new FaultException<DataException>(new DataException("Data Error: Key cannot be  null, empty, or whitespace"));
            }
            try
            {
                string value = Database.Read(key);

                byte[] encryptedValue = _3DESAlgorithm.Encrypt(value, sessionKey);

                return encryptedValue;
            }
            catch (FaultException<DataException> e)
            {
                Console.WriteLine("Data Error: {0}", e.Detail.Message);
                throw new FaultException<DataException>(new DataException(e.Detail.Message));
            }

            catch (Exception e)
            {

                throw new FaultException<DataException>(new DataException(e.Message));
            }

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

            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string user = Formatter.ParseName(windowsIdentity.Name);

            Console.WriteLine("User requesting the service: ");
            Console.WriteLine(user);

            byte[] sessionKey = GetSessionKey(user);
            string key;
            string value;
            try
            {
                 key = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedKey, sessionKey)).Trim('\0');
                 value = Encoding.ASCII.GetString(_3DESAlgorithm.Decrypt(encryptedValue, sessionKey)).Trim('\0');
            }
            catch(Exception e)
            {
                Console.WriteLine("Data Error: {0}", e.Message);
                throw new FaultException<DataException>(new DataException("Data Error: Key/Value cannot be  null, empty, or whitespace"));
            }
            try
            {
                
                Database.Write(key, value);
                return true;
            }
            catch (FaultException<DataException> e)
            {
                Console.WriteLine("Data Error: {0}", e.Detail.Message);
                throw new FaultException<DataException>(new DataException(e.Detail.Message));
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                throw new FaultException<DataException>(new DataException(e.Message));
            }
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
            if (userSessions.ContainsKey(user))
            {
                userSessions[user] = sessionKey;
                Console.WriteLine("Updated user session key.");
                return true;
            }
            else
            {
                userSessions.Add(user, sessionKey);
                Console.WriteLine("Saved user session key.");
                return true;
            }
        }
    }
}
