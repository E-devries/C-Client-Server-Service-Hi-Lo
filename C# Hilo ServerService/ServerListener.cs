///
/// FILE          : ServerListener.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This is code re-used from 'C# Client Server Hi-lo - Server' with a few modifications.
///               : The class in this file acts as a server and creates a TcpListener class to listen for connections.
///               : It continuously listens for a connection, packages request objects, and sends them to the business layer of the program
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace HiLo_serverService
{

    /// <summary>
    /// CLASS NAME  : ServerListener
    /// DESCRIPTION : This class acts as the listening end of the server program. It continuously listens for a connection and manages messages sent by multiple clients.
    ///             : It responds to the request and ends the connection
    /// </summary>
    class ServerListener
    {
        //=== Data Members ===//
        const int BYTE_ARRAY_SIZE = 256;

        private int port;

        public static volatile bool endProgram = false;



        //=== Methods ===//

        /// <summary>
        /// METHOD NAME : ServerListener
        /// DESCRIPTION : This constructor instantiates the server object with a given port
        /// </summary>
        /// <param name="portNew"></param>
        public ServerListener(int portNew)
        {
            port = portNew;
        }

        public void Listen()
        {
            TcpListener server = null;
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(Read);
            Thread readThread = null;


            try
            {

                // get the server address, which is the localhost
                string addressString = ConfigurationManager.AppSettings.Get("server");
                IPAddress localAddress = IPAddress.Parse(addressString);

                // create the listenser and start listening
                server = new TcpListener(localAddress, port);
                server.Start();

                // loop while listening
                while (endProgram == false)
                {
                    // check if there is a pennding request first
                    if (server.Pending() == true)
                    {
                        // accept client
                        TcpClient client = server.AcceptTcpClient();

                        if (client.Connected == true)
                        {
                            // start thread to read data so that we can keep listening
                            Logger.Log("The server has accepted a client.");

                            readThread = new Thread(threadStart);
                            readThread.Start(client);
                        }
                    }

                }

            }
            catch (SocketException err)
            {
                Logger.Log(err.Message);
            }
            catch (Exception err)
            {
                Logger.Log(err.Message);
            }
            finally
            {

                // join read thread if it got started
                if (readThread != null && readThread.IsAlive == true)
                {
                    readThread.Join();
                }

                // close the server
                server.Stop();
            }

        }

        public void Read(object clientObject)
        {
            // create data buffer
            Byte[] bytes = new Byte[BYTE_ARRAY_SIZE];
            String data = "";

            // convert client object back into TcpClient class
            TcpClient client = (TcpClient)clientObject;

            // creatre stream for client
            NetworkStream stream = client.GetStream();

            // receive the data
            int i = stream.Read(bytes, 0, bytes.Length);
            // translate back to ASCII
            data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);


            // process data
            HiloGame game = new HiloGame(data);
            string response = game.PlayGame();

            // set up response 
            byte[] message = System.Text.Encoding.ASCII.GetBytes(response);
            stream.Write(message, 0, message.Length);

            // close client now that message is taken and sent back
            client.Close();

        }
    }


}
