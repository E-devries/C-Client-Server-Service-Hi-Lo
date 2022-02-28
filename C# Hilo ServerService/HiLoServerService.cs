///
/// FILE          : HiLoServerService.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This code is re-used from 'C# Client Server Hi-lo - Server' with modifications to allow the server to run as a service.
///               : This file controls the main program logic for the server side of WMP A05. It plays a Hi-Lo game with the user who communicates
///               : with the server through a client program. The server handles game logic and stores information about the user. 


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace HiLo_serverService
{
    public partial class HiLoServerService : ServiceBase
    {
        // === Constants === //
        const string START_MESSAGE = "The HiLoServerService has started.";  // default logger message for OnStart()
        const string STOP_MESSAGE = "The HiLoServerService has stopped";    // default logger message for OnStop()

        // === Data Members === //
        private Thread serverThread;    // a thread to allow the server to take multiple clients
        private int port;               // the port that the server should use
        private ServerListener server;          // a server object to launch through threads

        public int Port {  get { return port; } }


        // === Methods === //

        /// <summary>
        /// METHOD NAME : HiLoServerService
        /// DESCRIPTION : THis is a constructor that initializes the service and sets values
        /// </summary>
        public HiLoServerService()
        {
            InitializeComponent();

            port = Int32.Parse(ConfigurationManager.AppSettings.Get("port"));
            server = new ServerListener(port);
            serverThread = new Thread(new ThreadStart(server.Listen));
        }

        /// <summary>
        /// METHOD NAME : OnStart
        /// DESCRIPTION : This method is an event fired when the service is asked to start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // start listening in a new thread
            serverThread.Start();
            Logger.Log(START_MESSAGE);
        }

        /// <summary>
        /// METHOD NAME : OnStop()
        /// DESCRIPTION : This 
        /// </summary>
        protected override void OnStop()
        {
            ServerListener.endProgram = true;
            serverThread.Join();
            Logger.Log(STOP_MESSAGE);
        }
    }
}
