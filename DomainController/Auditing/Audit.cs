using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainController.Auditing
{
    public class Audit
    {
        private static EventLog eventLog = null;
        private const string SourceName = "DomainController.Auditing";
        private const string LogName = "DCSecurityLog";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                eventLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                eventLog = null;
                Console.WriteLine("Error while creating Event Log handle: {0}", e.Message);
            }
        }

        public static void AuthenticationSuccess(string username)
        {
            if (eventLog != null)
            {
                string eventString = AuditEvents.AuthenticationSuccess;
                string message = string.Format(eventString, username);
                eventLog.WriteEntry(message);
            }
            else
            {
                throw new Exception("Error while writing event " + AuditEventTypes.AuthenticationSuccess.ToString());
            }
        }

        public static void AuthenticationFailure(string username)
        {
            if (eventLog != null)
            {
                string eventString = AuditEvents.AuthenticationFailure;
                string message = string.Format(eventString, username);
                eventLog.WriteEntry(message);
            }
            else
            {
                throw new Exception("Error while writing event " + AuditEventTypes.AuthenticationFailure.ToString());
            }
        }

        public static void ValidationSuccess(string serviceName)
        {
            if (eventLog != null)
            {
                string eventString = AuditEvents.ValidationSuccess;
                string message = string.Format(eventString, serviceName);
                eventLog.WriteEntry(message);
            }
            else
            {
                throw new Exception("Error while writing event " + AuditEventTypes.ValidationSuccess.ToString());
            }
        }

        public static void ValidationFailure(string serviceName)
        {
            if (eventLog != null)
            {
                string eventString = AuditEvents.ValidationFailure;
                string message = string.Format(eventString, serviceName);
                eventLog.WriteEntry(message);
            }
            else
            {
                throw new Exception("Error while writing event " + AuditEventTypes.ValidationFailure.ToString());
            }
        }
    }
}
