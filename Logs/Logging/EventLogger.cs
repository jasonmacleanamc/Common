using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace AMCCommon
{
    class EventLogger
    {
        
        private string Application;
        private string EventLogName;

        ///
        /// Constructor
        ///
        /// The application doing the logging /// The log to write to in the Event Viewer 
        public EventLogger(string app, string log)
        {
            Application = app;
            EventLogName = log;

            // Create the event log if it doesn't exist
            if (!EventLog.SourceExists(Application))
            {
                EventLog.CreateEventSource(Application, EventLogName);
            }
        }

        ///
        /// Write to the event log
        ///
        /// The message to write         
        public void WriteToEventLog(string message, string type)
        {
            switch (type.ToUpper())
            {
                case "INFO":
                    EventLog.WriteEntry(Application, message, EventLogEntryType.Information);
                    break;
                case "ERROR":
                    EventLog.WriteEntry(Application, message, EventLogEntryType.Error);
                    break;
                case "WARN":
                    EventLog.WriteEntry(Application, message, EventLogEntryType.Warning);
                    break;
                default:
                    EventLog.WriteEntry(Application, message, EventLogEntryType.Information);
                    break;
            }
        }
    }
}



