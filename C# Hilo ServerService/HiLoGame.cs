///
/// FILE          : Program.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This code is re-used from 'C# Client Server Hi-lo - Server'.
///               : This file controls the main program logic for the server side of WMP A05. It plays a Hi-Lo game with the user who communicates
///               : with the server through a client program. The server handles game logic and stores information about the user. 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HiLo_serverService
{
    /// <summary>
    /// CLASS NAME  : HiLoGame
    /// DESCRIPTION : This class models a HiLo Game. It contains a static container object to hold sessions from multiple clients at the same time.
    ///             : Every time the client sends a request, the HiLo game is instantiated, and the session is created or retrieved from the container.
    ///             : The game consists of prompting the client to guess a number within the stated range. Every time the client guesses a number,
    ///             : they are informed on whether or not the guess was too low, too high, or correct. If the guess is within range but not correct,
    ///             : a new range is generated. If the guess is not in range, the client is told and the range is not updated. If the client is correct, 
    ///             : they are given the oppurtunity to play a new game. 
    /// </summary>
    class HiloGame : Session
    {
        // === Constants === //
        const int EMPTY = 0;
        const char PAIR_DELIMITER = '&';
        const char KEY_VALUE_DELIMITER = '=';

        const string SESSION_KEY = "SESSION";
        const string USER_KEY = "USERNAME";
        const string GUESS_KEY = "GUESS";
        const string END_KEY = "ENDGAME";
        const string WON_KEY = "WONGAME";


        // === Data members === //

        // a static data structure to hold session data
        static private Dictionary<int, Session> sessionDictionary = new Dictionary<int, Session>();

        private int sessionID;      // the key for the session
        private int guessNumber;    // the current guess from the user
        private bool endGame;       // indicates whether the game has been won
        private static object lockDictionary = new object();    // a lock for the dictionary


        // === Methods === //

        /// <summary>
        /// METHOD NAME : gameData
        /// DESCRIPTION : This is a constructor that takes the gameData string sent to it and parses it for game information. 
        ///             : It then calls a method to create or find a dictionary entry for the session state in order to set the Game up.
        ///             : The string is based upon an agreed upon protocol where elements are delimited by '&' key-values are delimited by '=' 
        ///             : The possible keys to receive are SESSION, USERNAME, GUESS, ENDGAME, and WONGAME
        /// </summary>
        /// <param name="gameData"></param>
        public HiloGame(string gameData)
        {
            // start data members off with safe defaults
            sessionID = EMPTY;
            maxNumber = Int32.Parse(ConfigurationManager.AppSettings.Get("trueMaximumGuess"));
            minNumber = Int32.Parse(ConfigurationManager.AppSettings.Get("trueMinimumGuess"));
            correctNumber = EMPTY;
            guessNumber = EMPTY;
            userName = "";
            endGame = false;
            wonGame = false;

            //parse the string for data
            string[] splitData = gameData.Split(PAIR_DELIMITER);

            //update object
            for (int i = 0; i < splitData.Length; i++)
            {
                // for each key/value pair, break down further into array of 2
                string[] keyValue = splitData[i].Split(KEY_VALUE_DELIMITER);

                // first part of array will be the key, check for which values to update
                switch (keyValue[0])
                {
                    case SESSION_KEY:
                        sessionID = Int32.Parse(keyValue[1]);
                        break;

                    case USER_KEY:
                        userName = keyValue[1];
                        break;

                    case GUESS_KEY:
                        guessNumber = Int32.Parse(keyValue[1]);
                        break;

                    case END_KEY:
                        if (Equals(keyValue[1], "true") == true)
                        {
                            endGame = true;
                        }
                        else
                        {
                            endGame = false;
                        }
                        break;

                    case WON_KEY:
                        if (Equals(keyValue[1], "true") == true)
                        {
                            wonGame = true;
                        }
                        else
                        {
                            wonGame = false;
                        }
                        break;

                    default:
                        break;

                }
            }

            // if session does not exist, create it and add to dictionary, otherwise search for it and write over object data where applicable
            OpenGameState();

        }

        /// <summary>
        /// METHOD NAME : OpenGame
        /// DESCRIPTION : This method checks the sessionID to see if it needs to create or find a dictionary entry.
        ///             : If the sessionID is 0, a new ID is generated and a dictionary entry is added using the object data.
        ///             : If the sessionID isn't 0, it means that a session already exists, and the session is pulled from the dictionary 
        ///             : and used to update the HiLo object
        /// </summary>
        private void OpenGameState()
        {
            lock (lockDictionary)
            {
                // grab the values from the session and populate the object using sessionID data member to find the right session
                if (sessionID == EMPTY)
                {
                    // create new dictionary entry with object's current data
                    sessionID = sessionDictionary.Count + 1;
                    Session newSession = new Session(maxNumber, minNumber, correctNumber, userName, wonGame);
                    sessionDictionary.Add(sessionID, newSession);
                }
                else
                {
                    // find the session and update accordingly
                    Session previousSession = sessionDictionary[sessionID];

                    maxNumber = previousSession.MaxNumber;
                    minNumber = previousSession.MinNumber;
                    correctNumber = previousSession.CorrectNumber;
                    userName = previousSession.UserName;
                    wonGame = previousSession.WonGame;
                }
            }

            return;
        }

        /// <summary>
        /// METHOD NAME : PlayGame
        /// DESCRIPTION : This method runs through the game logic for the HiLoGame class. Based on the state of the session, 
        ///             : a random number is generated, the guess is evaluated, or the game is ended. 
        ///             
        /// </summary>
        /// <returns>string response : a string to send back to the user representing the server's response. 
        ///                          : It follows an agreed upon protocol where elements are delimited by '&', 
        ///                          : the first header is 'A05_HILO, and all other elements are key-value pairs delimited by '='.
        ///                          : the possible keys are ENDGAME, CORRECTNUMBER, WONGAME, ERROR, MINNUMBER, and MAXNUMBER. 
        ///                          : ENDGAME, WONGAME, MINNUMBER, and MAXNUMBER represent the data members of the same name,
        ///                          : while CORRECTNUMBER indicates whether or not a winning number was generated in this session, and ERROR indicates if the guess was out of range.
        ///  </returns>
        public string PlayGame()
        {
            string response = "A05_HILO";

            // check for endGame first, send string that indicates end of game if so
            if (endGame == true)
            {
                response += "&ENDGAME=true";
            }
            else
            {
                // first set sessionID so that client can be identified
                response += "&SESSION=" + sessionID.ToString();
                response += "&USERNAME=" + userName;

                // if wonGame is true but endGame is false, reset game values
                // since this is before the logic that dictates if the guess is correct, we know that the game was won in the last play 
                if (wonGame == true)
                {
                    wonGame = false;
                    correctNumber = EMPTY;
                    maxNumber = Int32.Parse(ConfigurationManager.AppSettings.Get("trueMaximumGuess"));
                    minNumber = Int32.Parse(ConfigurationManager.AppSettings.Get("trueMinimumGuess"));
                }

                // check for whether or not the win condition has been generated yet
                if (correctNumber == EMPTY)
                {
                    // generate random number w/in range and add to object
                    Random r = new Random();
                    correctNumber = r.Next(minNumber, maxNumber);

                    // set response
                    response += "&CORRECTNUMBER=set";
                }

                // check for whether or not guess exists yet
                if (guessNumber != EMPTY)
                {
                    // check guess against range and correct number
                    if (guessNumber <= maxNumber && guessNumber >= minNumber)
                    {
                        // check for win condition
                        if (guessNumber != correctNumber)
                        {
                            // guess is in range but wrong, update range and inform user
                            if (guessNumber < correctNumber)
                            {
                                minNumber = guessNumber + 1;
                            }
                            if (guessNumber > correctNumber)
                            {
                                maxNumber = guessNumber - 1;
                            }
                        }
                        else
                        {
                            // set wonGame and add response that indicates win
                            wonGame = true;
                            response += "&WONGAME=true";
                        }
                    }
                    else
                    {
                        // add response that indicates out of range error
                        response += "&ERROR=outOfRange";
                    }
                }

                // add range to response
                response += "&MAXNUMBER=" + maxNumber.ToString();
                response += "&MINNUMBER=" + minNumber.ToString();
            }
            ManageGameState();
            return response;
        }

        /// <summary>
        /// METHOD NAME : ManageGameState
        /// DESCRIPTION : This method manages the state of the game by saving over the dictionary entry if the game is not supposed to end,
        ///             : or deleting the entry if the game is asked to end. 
        /// </summary>
        private void ManageGameState()
        {
            lock (lockDictionary)
            {
                //delete the session entirely
                // use sessionID to delete right entry
                if (endGame == true)
                {
                    sessionDictionary.Remove(sessionID);
                }
                else
                {
                    Session saveSession = new Session(maxNumber, minNumber, correctNumber, userName, wonGame);
                    sessionDictionary[sessionID] = saveSession;
                }
            }

            return;
        }
    }
}
