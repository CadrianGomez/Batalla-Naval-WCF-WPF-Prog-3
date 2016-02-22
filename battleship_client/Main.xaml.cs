
/**
 * Este código es propiedad de su creador Alberto Robledo, Gomez Adrian , Portela Marcelo
 * UTN-FRre. Resistencia Chaco.
 *
 * Está protegido bajo la licencia LGPL v 2.1, cuya copia se puede encontrar en
 * el siguiente enlace: http://www.gnu.org/licenses/lgpl.txt
 *
 *
 * Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
 * Everyone is permitted to copy and distribute verbatim copies
 * of this license document, but changing it is not allowed.
 *
 *
 * This version of the GNU Lesser General Public License incorporates
 * the terms and conditions of version 3 of the GNU General Public
 * License, supplemented by the additional permissions listed below.
 *
 * Juego batalla naval usando WPF y WCF tegnologias de Microsoft
 *
 * Si desea contactarnos escríbanos a roblo53@hotmail.com
 */
using battleship_client.BattleshipServerRef;
using battleship_common;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace battleship_client
{
    //class ClientInstance
    //{
    //    ClientInstance(string name, string GUID);
    //}

    public partial class Main : Window, IBattleshipServiceCallback
    {
        private BattleshipServiceClient client;
        private string _GUID = "";
        private string _name;
        LoginPage loginPage;
        RoomsPage roomsPage;
        WaitPage waitPage;
        string chat;
        string opponent_name;
        //Begin code 
        public Main()
        {
            InitializeComponent();
            this.client = new BattleshipServiceClient(new InstanceContext(this));
            roomsPage = new RoomsPage(this);
            SetContent(roomsPage);
            loginPage = new LoginPage(this);
            ((RoomsPage)this.Content).grid.Children.Add(loginPage);
            loginPage.SetValue(Grid.RowSpanProperty, 4);
            loginPage.SetValue(Grid.ColumnSpanProperty, 2);
            this.Closing += Dispatcher_ShutdownStarted;
            chat = "";
        }

        private void rooms()
        {
            chat = "";
            roomsPage.ResetButtons();
            SetContent(roomsPage);
        }

        public void Join(string name)
        {
            try
            {
                this._name = name;
                client.Join(name);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void CreateRoom()
        {
            try
            {
                client.CreateRoom(_name, _GUID);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void DeleteRoom()
        {
            try
            {
                client.DeleteRoom(_name, _GUID);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        private void SetContent(UserControl nextPage)
        {
            this.Content = nextPage;
            this.MinHeight = nextPage.MinHeight + 40;
            this.MinWidth = nextPage.MinWidth + 20;
            this.Height = nextPage.MinHeight + 40;
            this.Width = nextPage.MinWidth + 20;
        }

        public void LogIn(string GUID)
        {
            this._GUID = GUID;
            ((RoomsPage)this.Content).grid.Children.Remove(loginPage);
            loginPage = null;
            roomsPage.SetUsername(_name);
        }

        public void UserNameExists()
        {
            MessageBox.Show("User name already exists on server!", "Retry!", MessageBoxButton.OK, MessageBoxImage.Warning);
            loginPage.Retry();
        }

        public void RoomCreated(battleship_common.Room room)
        {
            if (room.Name == _name)
            {
                waitPage = new WaitPage(this);
                ((RoomsPage)this.Content).grid.Children.Add(waitPage);
                waitPage.SetValue(Grid.RowSpanProperty, 4);
                waitPage.SetValue(Grid.ColumnSpanProperty, 2);
                return;
            }
            roomsPage.AddRoom(room);
        }

        public void RoomDeleted(string name)
        {
            if (name == _name)
            {

                roomsPage.ResetButtons();
                //SetContent(roomsPage);
                roomsPage.grid.Children.Remove(waitPage);
                return;
            }
            roomsPage.DeleteRoom(name);
        }

        public void JoinGame(string name)
        {
            try
            {
                client.JoinGame(_name, _GUID, name);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void LeaveGame()
        {
            try
            {
                client.LeaveGame(_name, _GUID);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
            rooms();
        }

        public void FatalError(string error)
        {
            MessageBox.Show(error, "Fatal error! Closing...", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        public void GameNotExists()
        {
            MessageBox.Show("The game not exists or already started. Try again...", "You late...", MessageBoxButton.OK, MessageBoxImage.Information);
            rooms();
        }

        public void GoodField()
        {
            if ((Content as PreparePage) != null)
            {
                (Content as PreparePage).GoodField();
            }
        }

        public void BadField(string comment)
        {
            if ((Content as PreparePage) != null)
            {
                (Content as PreparePage).BadField(comment);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                client.SendMessage(_name, _GUID, message);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void CheckField(bool[] field)
        {
            try
            {
                client.ReadyForGame(_name, _GUID, field);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void PrepareToGame(string opponent_name)
        {
            //MessageBox.Show("Prepare to game with" + opponent_name, "Goood!", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.opponent_name = opponent_name;
            SetContent(new PreparePage(this, opponent_name, chat));
        }

        public void StartGame()
        {
            if ((Content as PreparePage) != null)
            {
                SetContent((Content as PreparePage).GetGamePage());
            }
        }

        public void YouTurn()
        {
            if ((Content as GamePage) != null)
            {
                (Content as GamePage).YouTurn();
            }
        }

        public void DoTurn(int x, int y)
        {
            try
            {
                client.Turn(_name, _GUID, ShootType.Shoot, x, y);
            }
            catch (Exception exception)
            {
                CantConnectToServer(exception.Message);
            }
        }

        public void TransferMessage(battleship_client.BattleshipServerRef.Message message)
        {
            string mess = "[" + message.Author + "," + message.CreationTime.ToString("HH:mm:ss") + "]: " + message.Text;
            chat = chat + mess + "\n";
            if ((Content as PreparePage) != null)
            {
                (Content as PreparePage).PostMessage(mess);
            }

            if ((Content as GamePage) != null)
            {
                (Content as GamePage).PostMessage(mess);
            }
        }

        public void YouCheated()
        {
            rooms();
            MessageBox.Show("Your opponent cheated you...", "Sad but true...", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void CantConnectToServer(string message)
        {
            MessageBox.Show(message, "Can't connect to server!", MessageBoxButton.OK, MessageBoxImage.Error);
            client = null;
            Application.Current.Shutdown();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (_GUID != "" && client != null)
            {
                client.Leave(_name, _GUID);
            }
        }

        public void UpdateYourField(int x, int y, battleship_client.BattleshipServerRef.Cell state)
        {
            if ((Content as GamePage) != null)
            {
                (Content as GamePage).UpdateYourField(x, y, state);
            }
        }

        public void UpdateOpponentField(int x, int y, battleship_client.BattleshipServerRef.Cell state)
        {
            if ((Content as GamePage) != null)
            {
                (Content as GamePage).UpdateOpponentField(x, y, state);
            }
        }

        public void AlreadyClicked()
        {
            //MessageBox.Show("You are already clicks on this cell. Try another..", "Try again", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Win()
        {
            MessageBox.Show("Cool! You WIN!!!", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
            SetContent(new PreparePage(this, opponent_name, chat));
        }

        public void Loose()
        {
            MessageBox.Show("You are looser=)", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
            SetContent(new PreparePage(this, opponent_name, chat));
        }
    }
}
