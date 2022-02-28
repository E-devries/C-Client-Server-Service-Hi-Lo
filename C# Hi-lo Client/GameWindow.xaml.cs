///
/// FILE          : GameWindow.xaml.cs
/// PROJECT       : WMP_A05_Client
/// PROGRAMMER    : Elizabeth deVries
/// FIRST VERSION : 2021-11-10
/// DESCRIPTION   : The methods in this file handle the event logic for the GameWindow.
///               : In this window, the user plays a hi-lo game with the server. 
///               : The user's game state is saved by the server, which involves guessing between a given range until the right number is guessed.
///               : If the user wins, they can play a new game or end the game. They also have the ability to end the game at any time,
///               : which sends a request to the server first. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HiLo_Client
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private int session;
        private bool readyToClose;  // indicates whether the client is ready to close

        /// <summary>
        /// METHOD NAME : GameWindow
        /// DESCRIPTION : This constructor instantiates the game window and also sets it up to play the first round of hi-lo based on server response.
        /// </summary>
        /// <param name="serverResponse"></param>
        public GameWindow(string serverResponse)
        {
            InitializeComponent();
            readyToClose = false;   // when the client requests to end the game and the server confirms it, this is set to true
            UpdateScreen(serverResponse);
 
        }

        private void EnterSubmit_Click(object sender, RoutedEventArgs e)
        {
            // validate that it is a positive int
            string guess = enterField.Text;
            int parsedGuess = 0;

            if (Int32.TryParse(guess, out parsedGuess) == true)
            {
                // send to server
                ClientPlayGame continueGame = new ClientPlayGame();
                continueGame.SessionID = session;
                string serverResponse = continueGame.SendGuess(parsedGuess);

                // update screen
                UpdateScreen(serverResponse);
            }
            else
            {
                MessageBox.Show("You must enter an integer!", "Error: Invalid Format");
            }
        }

        private void Game_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (readyToClose == true)
            {
                e.Cancel = false;
            }
            else
            {
                Handle_Closing();
            }
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// METHOD NAME : Handle_Closing
        /// DESCRIPTION : This method handles the logic for closing the window, whether through the X button, by clicking on end game, 
        ///             : or by choosing not to play again. 
        ///             : the server is asked for confirmation first, the end of the game is processed on the server's end, and then the game ends on the client end.
        /// </summary>
        private void Handle_Closing()
        {
            // send the end request
            ClientPlayGame endGame = new ClientPlayGame();
            endGame.SessionID = session;
            string response = endGame.SendCloseGame();

            // retrieve the result
            endGame.ParseResponse(response);
            if (endGame.EndGame == true)
            {
                MessageBox.Show("Game Successfully ended!", "Thanks for playing!");
                readyToClose = true;
            }
            else
            {
                MessageBox.Show("The game failed to end on the server side!", "Server Error");
            }
        }

        /// <summary>
        /// METHOD NAME : UpdateScreen
        /// METHOD DESCRIPTION : This function takes a server response and updates the game window's messages to reflect the new status of the game.
        /// </summary>
        /// <param name="response"></param>
        private void UpdateScreen(string response)
        {
            // populate window with information based on initial response from server
            ClientPlayGame startGame = new ClientPlayGame();
            startGame.ParseResponse(response);

            // set up header and instruction
            gameHeader.Text = "Hi, " + startGame.UserName + "!";

            // check for won game, show dialog box if yes
            if (startGame.WonGame == true)
            {
                MessageBoxResult result = MessageBox.Show("You have won the game! Would you like to play again?", "Congratulations!", MessageBoxButton.YesNo);
               
                // if yes, then start new game, if no then attempt to end.
                if (result == MessageBoxResult.Yes)
                {
                    string newGameResponse = startGame.SendNewGame();
                    UpdateScreen(newGameResponse);

                }
                else
                {
                   Close();
                }
            }
            else
            {
                gameInstruction.Text = "Please guess an integer between " + startGame.MinNumber.ToString() + " and " + startGame.MaxNumber.ToString();
                session = startGame.SessionID;

                // check for out of range error from the server (business rule validation)
                if (startGame.RangeError == true)
                {
                    serverError.Text = "You entered a value out of range! Please enter a value in the above range, inclusive.";
                }
                else
                {
                    serverError.Text = "";
                }
            }

        }
    }
}
