///
/// FILE : ClientSpeaker.cs
/// PROJECT : C# Client Server Hi-Lo - Client
/// PROGRAMMER : Elizabeth deVries
/// FIRST VERSION : 2021-11-10
/// DESCRIPTION   : The class in this file functions as the client to the WMP_A05_Server program. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WMP_A05_Client
{
    /// <summary>
    /// CLASS NAME  : ClientSpeaker
    /// DESCRIPTION : This class uses the TcpClient class to connect to a server.
    /// </summary>
    class ClientSpeaker
    {
        // === Data members === //
        const int BYTE_ARRAY_SIZE = 256;

        private string response;    // the response from the operation. In a successful connection, this is what the server responded. 
                                    // Otherwise, it is an exception message
        public string Response { get { return response; }  }

        private bool success;   // whether or not the connection succeeded
        public bool Success { get { return success; } }

        private string server;  // the server to connect to
        private int port;       // the port to use
        private string message; // the message to send


        /// <summary>
        /// METHOD NAME : ClientSpeaker
        /// DESCRIPTION : This constructor instantiates a connection to the server, using a given server, port, and message.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        public ClientSpeaker(string serverNew, int portNew, string messageNew)
        {
            server = serverNew;
            port = portNew;
            message = messageNew;
            success = false;
            response = "";

            try
            {
                Connect();
            }
            // if exception encountered, change the response to the error message
            catch (ArgumentNullException error)
            {
                response = error.Message;
                success = false;
            }
            catch (SocketException error)
            {
                response = error.Message;
                success = false;
            }

           
        }

        /// <summary>
        /// METHOD NAME : Connect
        /// DESCRIPTION : This method connects to the given server on the given port, and sends the given message. 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        private void Connect()
        {
            // Create the client 
            TcpClient client = new TcpClient(server, port);

            if (client.Connected == true)
            {
                success = true;

                // turn the message into a byte array
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);


                // write to the server
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                // receive the response
                data = new Byte[BYTE_ARRAY_SIZE];

                int i = stream.Read(data, 0, data.Length);
                response += System.Text.Encoding.ASCII.GetString(data, 0, i);


                // close the stream and client
                stream.Close();
            }
            client.Close();

            return;
        }


    }
}
