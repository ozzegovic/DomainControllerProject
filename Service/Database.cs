using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Database
    {
        private static Dictionary<string, string> Data = new Dictionary<string, string>();

        // Data instance in file is represented by a key/value pair
        // Data instances in file are separated by newline
        internal static void Load(string serviceName)
        {
            string filename = GetFilePath(serviceName);

            if (!File.Exists(filename))
            {
                Console.WriteLine("First time running, database is empty.");
                File.Create(filename);
                return;
            }

            using (TextReader tr = new StreamReader(filename))
            {
                Console.WriteLine("Loading database...");

                string line;
                int count = 0;
                while ((line = tr.ReadLine()) != null)
                {
                    count++;
                    string[] parts = line.Split(':');

                    // when using || operator in an if statement, if the first value is true, other values aren't checked
                    if(parts.Count() != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                    {
                        Console.WriteLine($"Error parsing line {count}, skipping...");
                        continue;
                    }

                    if (Data.ContainsKey(parts[0]))
                    {
                        Console.WriteLine($"Key from line {count} already loaded, skipping...");
                        continue;
                    }

                    Data.Add(parts[0].Trim(), parts[1].Trim());
                }

                if(Data.Count == 0)
                {
                    Console.WriteLine("Database empty.");
                }
                else
                {
                    Console.WriteLine("Database loaded.");
                }
            }
        }

        // File is cleared (truncated) before saving new data
        internal static void Save(string serviceName)
        {
            if(Data.Count == 0)
            {
                Console.WriteLine("No data, skipping save...");
            }

            string filename = GetFilePath(serviceName);

            if (!File.Exists(filename))
            {
                Console.WriteLine("Database doesn't exist, creating...");
                File.Create(filename);
                Console.WriteLine("Database created.");
            }

            Console.WriteLine("Saving data...");
            FileInfo fi = new FileInfo(filename);
            using (TextWriter tw = new StreamWriter(fi.Open(FileMode.Truncate)))
            {
                foreach(var pair in Data)
                {
                    tw.WriteLine(pair.Key + ":" + pair.Value);
                }
            }
            Console.WriteLine("Data saved to file at " + fi.FullName);
        }

        public static void Write(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Data management error: Key cannot be null, empty, or whitespace");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Data management error: Value cannot be null, empty, or whitespace");
            }

            if (Data.ContainsKey(key))
            {
                Data[key] = value;
            }
            else
            {
                Data.Add(key, value);
            }
        }
        internal static string Read(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Data management error: Key cannot be null, empty, or whitespace");
            }

            if (Data.ContainsKey(key))
            {
                return Data[key];
            }
            else
            {
                throw new Exception("Data management error: Key doesn't exist in database.");
            }
        }

        // Generates path to C:\Documents and Settings\%USER NAME%\Application Data\DataManagement\
        private static string GetFilePath(string serviceName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + serviceName + ".txt";
            return path;
        }
    }
}
