///
/// FILE : SettingsWindow.xaml.cs
/// PROJECT : C# Client Server Hi-Lo - Client
/// PROGRAMMER : Elizabeth deVries
/// FIRST VERSION : 2021-11-10
/// DESCRIPTION   : The methods in this file control the events in the SettingsWindow page. 
///               : In this page, the user has the ability to change the server, port, and name. 
///               : They also have the ability to restore defaults and return back to the main window.

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
using System.Configuration;
using System.Text.RegularExpressions;

namespace HiLo_Client
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            SetTextField();
        }

        /// <summary>
        /// METHOD NAME : returnButton_Click
        /// DESCRIPTION : This method handled the event of the user pressing the return button. 
        ///             : This causes the settings window to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// METHOD NAME : restoreButton_Click
        /// DESCRIPTION : This method restores the server, port, and username values to their defaults.
        ///             : In the case of username, it becomes blank
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            string defaultServer = ConfigurationManager.AppSettings.Get("defaultServer");
            ConfigurationManager.AppSettings.Set("currentServer", defaultServer);

            string defaultPort = ConfigurationManager.AppSettings.Get("defaultPort");
            ConfigurationManager.AppSettings.Set("currentPort", defaultPort);

            string defaultUserName = ConfigurationManager.AppSettings.Get("defaultUserName");
            ConfigurationManager.AppSettings.Set("currentUserName", defaultUserName);

            SetTextField();
        }

        /// <summary>
        /// METHOD NAME : saveServer_Click
        /// DESCRIPTION : This method saves what the user entered into the server field as the new server.
        ///             : since this 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveServer_Click(object sender, RoutedEventArgs e)
        {
            string newServer = serverField.Text;
            Regex serverRegex = new Regex("[^0-9.]");

            if (serverRegex.IsMatch(newServer) == false)
            {
                ConfigurationManager.AppSettings.Set("currentServer", newServer);
                saveMessage.Text = "The Server has been saved.";
            }
            else
            {
                saveMessage.Text = "Invalid Server. You need to enter an IP adress as numbers and dots only.";
            }
            
            
        }

        /// <summary>
        /// METHOD NAME : savePort_Click
        /// DESCRIPTION : This method saves what the user entered into the port field as the new server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePort_Click(object sender, RoutedEventArgs e)
        {
            string newPort = portField.Text;
            int portNum = 0;

            if (Int32.TryParse(newPort, out portNum) == true)
            {
                ConfigurationManager.AppSettings.Set("currentPort", newPort);

                saveMessage.Text = "The Port has been saved.";
            }
            else
            {
                saveMessage.Text = "Invalid Port. You must use an integer.";
            }

        }

        /// <summary>
        /// METHOD : saveUserName_Click
        /// DESCRIPTION : This method saves what the user entered into the usetname field as the new username.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveUserName_Click(object sender, RoutedEventArgs e)
        {
            string newUserName = userNameField.Text;
            ConfigurationManager.AppSettings.Set("currentUserName", newUserName);

            saveMessage.Text = "The username has been saved.";
        }

        /// <summary>
        /// METHOD NAME : SetTextField
        /// DESCRIPTION : This method sets all of the setting text fields to what the current values are.
        /// </summary>
        private void SetTextField()
        {
            // set text field boxes to settings to current values
            serverField.Text = ConfigurationManager.AppSettings.Get("currentServer");
            portField.Text = ConfigurationManager.AppSettings.Get("currentPort");
            userNameField.Text = ConfigurationManager.AppSettings.Get("currentUserName");
            saveMessage.Text = "";
        }
    }
}
