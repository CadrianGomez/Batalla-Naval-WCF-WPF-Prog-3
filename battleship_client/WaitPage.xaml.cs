using System;
using System.Windows;
using System.Windows.Controls;

namespace battleship_client
{
    public partial class WaitPage : UserControl
    {
        private Main main;
        public WaitPage(Main main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Back.IsEnabled = false;
            main.DeleteRoom();
        }
    }
}
