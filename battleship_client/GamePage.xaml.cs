using battleship_client.BattleshipServerRef;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace battleship_client
{
    public partial class GamePage : UserControl
    {
        private Main main;
        private string opponent_name;
        private bool my_turn;
        bool[] field;
        public GamePage(Main main, string opponent_name, string initial_chat, bool []field)
        {
            InitializeComponent();
            this.main = main;
            textBlockMessages.Text = initial_chat;
            this.opponent_name = opponent_name;
            for (int i = 0; i < 100; i++)
            {
                if (field[i])
                {
                    Rectangle cell = (Rectangle)FindName(string.Format("mycell{0}", i));
                    Style style = this.FindResource("ShipCell") as Style;
                    cell.Style = style;
                }
            }
            my_turn = false;
            Greetings.Content = "Nombre del Rival: " + opponent_name + "; Turno: " + opponent_name;
            this.field = new bool[100];
        }

        public void PostMessage(string message)
        {
            textBlockMessages.Text = textBlockMessages.Text + message + "\n";
        }

        private void Cell_LeftClick(object sender, RoutedEventArgs e)
        {
            if (my_turn)
            {
                Rectangle cell = (Rectangle)sender;
                var coordinates = cell.Name.Remove(0, 6);
                int x = int.Parse(coordinates) % 10;
                int y = int.Parse(coordinates) / 10;
                if (field[x + y * 10])
                    return;
                my_turn = false;
                Greetings.Content = "Nombre del Rival: " + opponent_name + "; Turno: " + opponent_name;
                main.DoTurn(x, y);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.LeaveGame();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (messageInput.Text.Length > 0 || !string.IsNullOrEmpty(messageInput.Text))
            {
                main.SendMessage(messageInput.Text);
                messageInput.Clear();
            }
            else
                MessageBox.Show("Message is empty...", "Try again...", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void YouTurn()
        {
            Greetings.Content = "Nombre del Rival: " + opponent_name + "; Turn: tu";
            my_turn = true;
        }

        public void UpdateYourField(int x, int y, battleship_client.BattleshipServerRef.Cell state)
        {
            Rectangle cell = (Rectangle)FindName(string.Format("mycell{0}", x + y * 10));
            Style style = this.FindResource(state == Cell.DeadShip ? "DeadShipCell" :
                state == Cell.Empty ? "EmptyCell" :
                state == Cell.Fire ? "FireCell" :
                state == Cell.Missed ? "MissedCell" : 
                "ShipCell") as Style;
            cell.Style = style;
        }

        public void UpdateOpponentField(int x, int y, battleship_client.BattleshipServerRef.Cell state)
        {
            field[x + y * 10] = true;
            Rectangle cell = (Rectangle)FindName(string.Format("opcell{0}", x + y * 10));
            Style style = this.FindResource(state == Cell.DeadShip ? "DeadShipCell" :
                state == Cell.Empty ? "EmptyCell" :
                state == Cell.Fire ? "FireCell" :
                state == Cell.Missed ? "MissedCell" :
                "ShipCell") as Style;
            cell.Style = style;
        }
    }
}
