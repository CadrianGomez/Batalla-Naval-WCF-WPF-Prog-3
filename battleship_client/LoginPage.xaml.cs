using System;
using System.Windows;
using System.Windows.Controls;

namespace battleship_client
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        private Main main;
        public LoginPage(Main main)
        {
            InitializeComponent();
            this.main = main;
        }

        public void Retry()
        {
            Connect.IsEnabled = true;
            Login.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Connect.IsEnabled = false;
            Login.IsEnabled = false;
            main.Join(Login.Text);
        }
    }
}
