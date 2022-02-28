///
/// FILE          : Logger.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This code is re-used from 'C# Client Server Hi-lo - Server'
///               : The class in this file acts as a logger and logs the service starting and stopping, as well as client interactions with the server
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace HiLo_serverService
{
    static class Logger
    {
        private static object loggerLock = new object();
        public static void Log(string message)
        {
            // set the message up
            message += " TIME: " + (DateTime.Now).ToString() + "\n";

            // run the text file logger
            string filePath = ConfigurationManager.AppSettings.Get("loggerFilePath");

            lock (loggerLock)
            {
                try
                {
                    File.AppendAllText(filePath, message);
                }
                catch(Exception err)
                {
                    // run the event log logger if the text log fails
                    EventLog serviceEventLog = new EventLog();
                    if (EventLog.SourceExists("HiLoServerServiceSource") == false)
                    {
                        EventLog.CreateEventSource("HiLoServerServiceSource", "HiLoServerServiceLog");
                    }
                    serviceEventLog.Source = "HiLoServerServiceSource";
                    serviceEventLog.Log = "HiLoServerServiceLog";
                    serviceEventLog.WriteEntry("Text logger failed: " + err.Message + "\nIntended logger message: " + message);
                }
                
            }
        }
    }
}
