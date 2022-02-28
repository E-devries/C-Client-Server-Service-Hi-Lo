///
/// FILE          : Session.cs
/// PROJECT       : C# Client Server-Service Hi-Lo - Server
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-09
/// DESCRIPTION   : This code is re-used from 'C# Client Server Hi-lo - Server'
///               : The class in this file acts as a structure to hold information about the HiLoGame session. 
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiLo_serverService
{
    /// <summary>
    /// CLASS NAME : Session
    /// DESCRIPTION : This class contains information about the session of a HiLoGame. It contains the user's name, their current min and max ranges,
    ///             : the winning number, whether or not they have requested to end the game, and their current guess
    /// </summary>
    class Session
    {

        protected int maxNumber;      // the current max range for the client's game, inclusive
        public int MaxNumber { get { return maxNumber; } }

        protected int minNumber;      // the current minimum range for the client's game, inclusive
        public int MinNumber { get { return minNumber; } }

        protected int correctNumber;  // the winning number that the client is trying to guess
        public int CorrectNumber { get { return correctNumber; } }

        protected string userName;    // the name of the user
        public string UserName { get { return userName; } }

        protected bool wonGame;       // indicates whether or not the game is complete
        public bool WonGame { get { return wonGame; } }

        /// <summary>
        /// METHOD NAME : Session
        /// DESCRIPTION : This constructor creates a session populated by values determined in HiloGame
        /// </summary>
        /// <param name="maxNumberNew"></param>
        /// <param name="minNumberNew"></param>
        /// <param name="correctNumberNew"></param>
        /// <param name="guessNumberNew"></param>
        /// <param name="userNameNew"></param>
        /// <param name="endGameNew"></param>
        /// <param name="wonGameNew"></param>
        public Session(int maxNumberNew, int minNumberNew, int correctNumberNew, string userNameNew, bool wonGameNew)
        {
            maxNumber = maxNumberNew;
            minNumber = minNumberNew;
            correctNumber = correctNumberNew;
            userName = userNameNew;
            wonGame = wonGameNew;
        }

        /// <summary>
        /// METHOD NAME : Session
        /// DESCRIPTION : This constructor creates a Session without setting any parameters. 
        ///             : It is used when instantiating a HiLoGame, which will set the values in its own constructor.
        /// </summary>
        public Session()
        {

        }
    }
}
