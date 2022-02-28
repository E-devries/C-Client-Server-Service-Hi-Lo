///
/// FILE          : Program.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This code launches the service and allows it to run
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HiLo_serverService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HiLoServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
