///
/// FILE : PlayGame.cs
/// PROJECT : C# Client Server Hi-Lo - Client
/// PROGRAMMER : ELizabeth deVries
/// FIRST VERSION : 2021-11-10
/// DESCRIPTION   : This file contains the PlayGame class, which takes the inputs passed on by the view layer and creates and parses the messages.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HiLo_Client
{
    /// <summary>
    /// CLASS NAME : PlayGame
    /// DESCRIPTION : This class creates the messages to send to the server to play the Hi-Lo game, and parses the response from the server
    ///             : in order to determine the status of the game. 
    ///             : It follows an agreed upon protocol where elements are delimited by '&', 
    ///             : the first header is 'A05_HILO, and all other elements are key-value pairs delimited by '='.
    ///             : the possible keys to receive are: WONGAME, SESSION, ERROR, ENDGAME, CORRECTNUMBER, MAXNUMBER, and MINNUMBER
    ///             : the possible keys to send are: SESSION, USERNAME, GUESS, ENDGAME, and WONGAME
    /// </summary>
    class ClientPlayGame
    {
        //=== Data Members ===//

        // constants
        const char PAIR_DELIMITER = '&';
        const char KEY_VALUE_DELIMITER = '=';

        const string WON_KEY = "WONGAME";
        const string SESSION_KEY = "SESSION";
        const string END_KEY = "ENDGAME";
        const string CORRECT_KEY = "CORRECTNUMBER";
        const string ERROR_KEY = "ERROR";
        const string MAX_KEY = "MAXNUMBER";
        const string MIN_KEY = "MINNUMBER";
        const string USER_KEY = "USERNAME";

        // non constants
        private string response;
        public string Response { get { return response; } }

        private bool connected = false;
        public bool Connected { get { return connected; } }

        private int sessionID;  // store the sessionID to send back to server
        public int SessionID { get { return sessionID; } set { sessionID = value; } }

        private bool wonGame;   // stores whether or not player has won
        public bool WonGame { get { return wonGame; } }

        private bool endGame;   // stores whether or not the user has decided to end the game
        public bool EndGame { get { return endGame; } set { endGame = value; } }

        private bool correctNumberSet;  // stores whether or not the winning number was generated
        public bool CorrectNumberSet { get { return correctNumberSet; } }

        private bool rangeError;        // stores whether or not the server has returned an error indicating the value was out of range
        public bool RangeError { get { return rangeError; } }

        private int minNumber;          // stores the minimum number determined by the server
        public int MinNumber { get { return minNumber; } }

        private int maxNumber;          // stores the maximum number determined by the server
        public int MaxNumber { get { return maxNumber; } }

        private string userName;        // stores the username, held by the server
        public string UserName { get { return userName; } }
        


        private string server;
        private int port;

        /// <summary>
        /// METHOD NAME : PlayGame
        /// DESCRIPTION : This constructor instantiates the ClientPlayGame object by setting up the data members
        /// </summary>
        public ClientPlayGame()
        {
            server = ConfigurationManager.AppSettings.Get("currentServer");
            port = Int32.Parse(ConfigurationManager.AppSettings.Get("currentPort"));

            connected = false;
            response = "";
            wonGame = false;
            endGame = false;
            correctNumberSet = false;
            rangeError = false;
            minNumber = 0;
            maxNumber = 0;
        }

        /// <summary>
        /// METHOD NAME : SendName
        /// DESCRIPTION : This method starts the game by sending the user's name to the server. It returns the server's response
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SendName(string name)
        {
            string message = "A05_HILO&USERNAME=" + name;

            return Send(message);
        }

        /// <summary>
        /// METHOD NAME : SendGuess
        /// DESCRIPTION : This method sends a guess number to the server. It returns the server's response
        /// </summary>
        /// <param name="guess"></param>
        /// <returns></returns>
        public string SendGuess(int guess)
        {
            string message = "A05_HILO&SESSION=" + sessionID + "&GUESS=" + guess.ToString();

            return Send(message);

        }

        /// <summary>
        /// METHOD NAME : SendCloseGame
        /// DESCRIPTION : This method sends an indication to the server to end the game session. It returns the server's confirmation.
        /// </summary>
        /// <returns></returns>
        public string SendCloseGame()
        {
            string message = "A05_HILO&Session=" + sessionID + "&ENDGAME=true";

            return Send(message);
        }

        ///
        /// METHOD NAME : SendNewGame
        /// DESCRIPTION : This method sends an indication to the server to re-start the game session. It returns the server's response.
        public string SendNewGame()
        {
            string message = "A05_HILO&Session=" + sessionID + "&ENDGAME=false&WONGAME=true";

            return Send(message);
        }

        /// <summary>
        /// METHOD NAME : Send
        /// DESCRIPTION : This method sends the message to the server, and gets a response.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string Send(string message)
        {
            ClientSpeaker client = new ClientSpeaker(server, port, message);
            // if it failed, indicate
            if (client.Success == false)
            {
                connected = false;
            }
            else
            {
                connected = true;
            }

            return client.Response;
        }

        /// <summary>
        /// METHOD NAME : ParseResponse
        /// DESCRIPTION : This method parses the response from the server and populates the object with the relevant information.
        /// </summary>
        /// <param name="response"></param>
        public void ParseResponse(string response)
        {
            // split the response into elements
            string[] splitData = response.Split(PAIR_DELIMITER);

            for (int i = 0; i < splitData.Length; i++)
            {
                // split each element into key and value
                string[] keyValue = splitData[i].Split(KEY_VALUE_DELIMITER);

                // check for keys to assign correct values
                switch(keyValue[0])
                {
                    case SESSION_KEY:
                        sessionID = Int32.Parse(keyValue[1]);
                        break;

                    case WON_KEY:
                        wonGame = true;
                        break;

                    case END_KEY:
                        endGame = true;
                        break;

                    case CORRECT_KEY:
                        correctNumberSet = true;
                        break;

                    case ERROR_KEY:
                        rangeError = true;
                        break;

                    case MAX_KEY:
                        maxNumber = Int32.Parse(keyValue[1]);
                        break;

                    case MIN_KEY:
                        minNumber = Int32.Parse(keyValue[1]);
                        break;

                    case USER_KEY:
                        userName = keyValue[1];
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
