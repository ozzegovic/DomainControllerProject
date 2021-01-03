using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DataManagement : IDataManagement, IDataManagementDC
    {
        byte[] key;
        byte[] decryptedKey;
        SHA256 sha256Hash = SHA256.Create();

        // TO DO: Change return type to byte[]
        // decrypt request
        // read data from database
        // encrypt read values and send back to the client
        public bool Read(byte[] encryptedData)
        {
            Console.WriteLine("Received encrypted READ request");

            byte[] decryptedData = _3DESAlgorithm.Decrypt(encryptedData, decryptedKey);
            Console.WriteLine($"DEcrypted key: {BitConverter.ToString(decryptedKey)}");

            Console.WriteLine($"DEcrypted data: {ASCIIEncoding.ASCII.GetString(decryptedData)}");

            return true;
        }

        // TO DO: Change return type to byte[]
        // decrypt request
        // write data to the database
        // encrypt data and send back to the client
        public bool Write(byte[] encryptedData)
        {
            Console.WriteLine("Received encrypted WRITE request");

            byte[] decryptedData = _3DESAlgorithm.Decrypt(encryptedData, decryptedKey);
            Console.WriteLine($"DEcrypted key: {BitConverter.ToString(decryptedKey)}");

            Console.WriteLine($"DEcrypted data: {ASCIIEncoding.ASCII.GetString(decryptedData)}");

            return true;
        }


        // Domain Controller sends session key after a client requested it
        // TO DO: Domain controller can remove it from the active services list if it cannot connect/doesnt receive any answer
        public bool SendSessionKey(byte[] sessionKey)
        {
            Console.WriteLine("Received encrypted session key");

            key = sha256Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes("pass"));
            decryptedKey = _3DESAlgorithm.Decrypt(sessionKey, key);
            Console.WriteLine($"DEcrypted key: {BitConverter.ToString(decryptedKey)}");

            return true;
        }
    }
}
