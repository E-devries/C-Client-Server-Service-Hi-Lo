///
/// FILE    : MainWindow.xaml.cs
/// PROJECT : C# Client Server Hi-lo - Client
/// PROGRAMMER : Elizabeth deVries
/// FIRST VERSION : 2021-11-10
/// DESCRIPTION   : This file contains the code behind logic for the StartWindow of the A05 client.
///               : It contains logic for the play game and settings button. The Settings button allows the user to set name, server, and port,
///               : While the play game button directs the user to the game. If the user has not entered a name but presses play, 
///               : they will be prompted to visit settings first.

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace WMP_A05_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// METHOD NAME : settingsButton_Click
        /// DESCRIPTION : this method directs the user to a new form where they are able to enter a name, port, and server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
        }

        /// <summary>
        /// METHOD NAME : playGameButton_Click
        /// DESCRIPTION : this method directs the user to a new form where they will be able to play the hilo game.
        ///             : If the user has not entered a name yet, they will be prompted to do so via the settings button instead. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playGameButton_Click(object sender, RoutedEventArgs e)
        {

            // if there is no username, show a message box and do not proceed
            if (ConfigurationManager.AppSettings.Get("currentUserName") == "")
            {
                MessageBox.Show("You must enter a username in the settings to play the game.", "Error: Missing Username");
            }
            else
            {
                // start the client so that we can check if a connection will work
                ClientPlayGame game = new ClientPlayGame();

                // because it is the first time, we need to get the name from the settings.
                string name = ConfigurationManager.AppSettings.Get("currentUserName");
                string serverResponse = game.SendName(name);
                
                // if connection works, continue to game window
                if (game.Connected == true)
                {
                    // change game window to main window and close this one.
                    Application.Current.MainWindow = new GameWindow(serverResponse);
                    Application.Current.MainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Could not connect to the server! Make sure your port and server are correct!", "Connection Error");
                }



                
            }
        }

    }
}
