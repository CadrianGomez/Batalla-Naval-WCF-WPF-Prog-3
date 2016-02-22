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
using battleship_common;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace battleship_server
{
    class Field
    {
        public Field()
        {
            field = new Cell[10, 10];
            Clear();
            
        }
        private Cell[,] field;

        public Cell GetCell(int x, int y)
        {
            return 0 <= x && x < 10 && 0 <= y && y < 10 ? field[x, y] : Cell.Empty;
        }

        public void SetCell(int x, int y, Cell cell_type)
        {
            if (0 <= x && x < 10 && 0 <= y && y < 10)
            {
                field[x, y] = cell_type;
            }
        }
        public void Clear()
        {
            int i, j;
            for (i = 0; i < 10; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    field[i, j] = Cell.Empty;
                }
            }
        }
    }

    class Point
    {
        public Point()
        {
            x = y = 0;
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
    }

    class Game
    {
        public Game(Client opponent)
        {
            this.opponent = opponent;
            this.field = new Field();
            my_turn = false;
            ships = 10;
        }
        private Client opponent;
        public Client Opponent
        {
            get
            {
                return opponent;
            }
        }
        public void SetField(bool []field)
        {
            int pos = 0, i, j;
            for (i = 0; i < 10; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    this.field.SetCell(j, i, field[pos] ? Cell.Ship : Cell.Empty);
                    pos++;
                }
            }
            ships = 10;
        }
        public Field field;
        public int four_count;
        public int three_count;
        public int two_count;
        public int one_count;
        public bool my_turn;
        public int ships;
    }


    class Client
    {
        private static List<Room> _rooms = new List<Room>();
        private IClientCallback _callback;
        private string _GUID;
        private string _name;
        private Room _room;
        private Game _game;
        private bool _ready;

        public Client(IClientCallback callback, string name)
        {
            this._callback = callback;
            this._name = name;
            this._GUID = Guid.NewGuid().ToString();
            
            callback.LogIn(this._GUID);
            foreach (var room in Client.Rooms)
            {
                callback.RoomCreated(room);
            }
            _room = null;
            _game = null;
            _ready = false;
        }

        public IClientCallback Callback
        {
            get { return _callback; }
        }

        public bool CheckGUID(string GUID)
        {
            return _GUID == GUID;
        }

        public string Name
        {
            get { return _name; }
        }

        public Room CreateRoom()
        {
            if (_room != null || _game != null)
            {
                return null;
            }
            _room = new Room(_name, DateTime.Now);
            _rooms.Add(_room);
            return _room;
        }

        public bool DeleteRoom()
        {
            if (_room == null)
            {
                return false;
            }
            _rooms.Remove(_room);
            _room = null;
            return true;
        }

        public void LeaveGame()
        {
            if (_game != null)
            {
                _game.Opponent.YouCheated();
            }
            _game = null;
        }

        public void YouCheated()
        {
            _game = null;
            try
            {
                Callback.YouCheated();
            }
            catch (Exception) { };
        }

        public void JoinTo(Client opponent)
        {
            _game = opponent.LetsPlay(this);
            _ready = false;
        }

        public Game LetsPlay(Client opponent)
        {
            if (!HaveGame)
            {
                _game = new Game(opponent);
            }
            _ready = false;
            return new Game(this);
        }

        public void SendMessage(string message)
        {
            Message mess = new Message(_name, DateTime.Now, message);
            _game.Opponent.RecieveMessage(mess);
            RecieveMessage(mess);
        }

        public void RecieveMessage(Message mess)
        {
            try
            {
                _callback.TransferMessage(mess);
            }
            catch (Exception) { };
        }

        public void DoTurn(int x, int y)
        {
            if (_game != null)
            {
                if (_game.my_turn)
                {
                    _game.my_turn = false;
                    _game.Opponent.GetTurn(x, y);
                }
            }
        }

        private bool ListContained(List<Point> list, int x, int y)
        {
            foreach (var point in list)
            {
                if (point.x == x && point.y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void GetTurn(int x, int y)
        {
            if (_game != null)
            {
                if (_game.field.GetCell(x, y) == Cell.Empty)
                {
                    _game.field.SetCell(x, y, Cell.Missed);
                    try
                    {
                        _callback.UpdateYourField(x, y, Cell.Missed);
                    }
                    catch (Exception) { }
                    try
                    {
                        _game.Opponent.UpdateOpponentField(x, y, Cell.Missed);
                    }
                    catch (Exception) { }
                    YouTurn();
                    return;
                }
                if (_game.field.GetCell(x, y) == Cell.DeadShip || _game.field.GetCell(x, y) == Cell.Fire || _game.field.GetCell(x, y) == Cell.Missed)
                {
                    _game.Opponent.AlreadyClicked();
                    _game.Opponent.YouTurn();
                    return;
                }
                List<Point> list = new List<Point>();
                list.Add(new Point(x, y));
                _game.field.SetCell(x, y, Cell.Fire);
                bool killed = true;
                for (int i = 0; i < list.Count; i++)
                {
                    int lx = list[i].x;
                    int ly = list[i].y;
                    if (lx > 0)
                    {
                        if (_game.field.GetCell(lx-1, ly) == Cell.Ship)
                        {
                            killed = false;
                            break;
                        }
                        if (_game.field.GetCell(lx-1, ly) == Cell.Fire && !ListContained(list, lx-1, ly))
                        {
                            list.Add(new Point(lx - 1, ly));
                        }
                    }
                    if (lx < 9)
                    {
                        if (_game.field.GetCell(lx + 1, ly) == Cell.Ship)
                        {
                            killed = false;
                            break;
                        }
                        if (_game.field.GetCell(lx + 1, ly) == Cell.Fire && !ListContained(list, lx + 1, ly))
                        {
                            list.Add(new Point(lx + 1, ly));
                        }
                    }
                    if (ly > 0)
                    {
                        if (_game.field.GetCell(lx, y-1) == Cell.Ship)
                        {
                            killed = false;
                            break;
                        }
                        if (_game.field.GetCell(lx, ly-1) == Cell.Fire && !ListContained(list, lx, ly-1))
                        {
                            list.Add(new Point(lx, ly-1));
                        }
                    }
                    if (ly < 9)
                    {
                        if (_game.field.GetCell(lx, ly+1) == Cell.Ship)
                        {
                            killed = false;
                            break;
                        }
                        if (_game.field.GetCell(lx, ly+1) == Cell.Fire && !ListContained(list, lx, ly+1))
                        {
                            list.Add(new Point(lx, ly+1));
                        }
                    }
                }
                if (!killed)
                {
                    try
                    {
                        _callback.UpdateYourField(x, y, Cell.Fire);
                    }
                    catch (Exception) { }
                    try
                    {
                        _game.Opponent.UpdateOpponentField(x, y, Cell.Fire);
                    }
                    catch (Exception) { }
                    _game.Opponent.YouTurn();
                    return;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    int lx = list[i].x;
                    int ly = list[i].y; 
                    if (lx > 0)
                    {
                        if (_game.field.GetCell(lx - 1, ly) == Cell.Empty)
                        {
                            _game.field.SetCell(lx - 1, ly, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx - 1, ly, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx - 1, ly, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (lx > 0 && ly > 0)
                    {
                        if (_game.field.GetCell(lx - 1, ly-1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx - 1, ly-1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx - 1, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx - 1, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (ly > 0)
                    {
                        if (_game.field.GetCell(lx, ly-1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx, ly-1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (ly > 0 && lx < 9)
                    {
                        if (_game.field.GetCell(lx+1, ly-1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx+1, ly-1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx+1, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx+1, ly-1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (lx < 9)
                    {
                        if (_game.field.GetCell(lx+1, ly) == Cell.Empty)
                        {
                            _game.field.SetCell(lx+1, ly, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx+1, ly, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx+1, ly, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }

                    if (lx < 9 && ly < 9)
                    {
                        if (_game.field.GetCell(lx+1, ly+1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx+1, ly+1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx+1, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx+1, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (ly < 9)
                    {
                        if (_game.field.GetCell(lx, ly+1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx, ly+1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    if (lx > 0 && ly < 9)
                    {
                        if (_game.field.GetCell(lx-1, ly+1) == Cell.Empty)
                        {
                            _game.field.SetCell(lx-1, ly+1, Cell.Missed);
                            try
                            {
                                _callback.UpdateYourField(lx-1, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                            try
                            {
                                _game.Opponent.UpdateOpponentField(lx-1, ly+1, Cell.Missed);
                            }
                            catch (Exception) { }
                        }
                    }
                    _game.field.SetCell(lx, ly, Cell.DeadShip);
                    try
                    {
                        _callback.UpdateYourField(lx, ly, Cell.DeadShip);
                    }
                    catch (Exception) { }
                    try
                    {
                        _game.Opponent.UpdateOpponentField(lx, ly, Cell.DeadShip);
                    }
                    catch (Exception) { }
                }
                _game.ships--;
                if (_game.ships == 0)
                {
                    try
                    {
                        _game.Opponent.Win();
                    }
                    catch (Exception) { }
                    try
                    {
                        _callback.Loose();
                    }
                    catch (Exception) { }
                    Client client = _game.Opponent;
                    _game = null;
                    client.JoinTo(this);
                    return;
                }
                _game.Opponent.YouTurn();
                return;
            }
        }

        public void UpdateOpponentField(int x, int y, Cell type)
        {
            try
            {
                _callback.UpdateOpponentField(x, y, type);
            }
            catch (Exception) { }
        }

        public void AlreadyClicked()
        {
            try
            {
                _callback.AlreadyClicked();
            }
            catch (Exception) { }
        }

        public void Win()
        {
            try
            {
                _callback.Win();
            }
            catch (Exception) { }
        }

        public void YouTurn()
        {
            _game.my_turn = true;
            try
            {
                _callback.YouTurn();
            }
            catch (Exception) { }
        }

        public void SetField(bool[] field)
        {
            if (_game != null)
            {
                _game.SetField(field);
                _ready = true;
                if (!IsOpponentReady())
                    _game.my_turn = true;
            }
        }

        public bool IsOpponentReady()
        {
            if (_game != null)
            {
                return _game.Opponent.Ready; 
            }
            return false;
        }

        public string Opponent()
        {
            if (_game != null)
            {
                return _game.Opponent.Name;
            }
            return "";
        }

        public static List<Room> Rooms
        {
            get
            {
                return _rooms;
            }
        }

        public bool HaveRoom
        {
            get
            {
                return _room != null;
            }
        }

        public bool HaveGame
        {
            get
            {
                return _game != null;
            }
        }

        public bool Ready
        {
            get
            {
                return _ready;
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BattleshipService : IBattleshipService
    {
        Dictionary<string, Client> clientsDictionary = new Dictionary<string, Client>();

        public void Join(string name)
        {
            if (!clientsDictionary.ContainsKey(name))
            {
                Client newClient = null;
                try
                {
                    newClient = new Client(OperationContext.Current.GetCallbackChannel<IClientCallback>(), name);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error {0}!", name);
                    return;
                }
                clientsDictionary.Add(name, newClient);
                Console.WriteLine("Cliente {0} se unio!", name);
            }
            else
            {
                Console.WriteLine("Ese nombre ya existe{0}!", name);
                try
                {
                    OperationContext.Current.GetCallbackChannel<IClientCallback>().UserNameExists();
                }
                catch (Exception) { }
            }
        }

        public void CreateRoom(string name, string GUID)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                Room room = clientsDictionary[name].CreateRoom();
                if (room != null)
                {
                    Console.WriteLine("Cliente {0} creo un juego!", name);
                    List<Client> failed = new List<Client>();
                    foreach (var client in clientsDictionary.Values)
                    {
                        try
                        {
                            client.Callback.RoomCreated(room);
                        }
                        catch (Exception e)
                        {
                            failed.Add(client);
                        }
                    }
                    SecureDeleteClients(failed);
                    return;
                }
                Console.WriteLine("Cliente {0} quiere crear un juego pero ya existe un juego creado!", name);
                try
                {
                    clientsDictionary[name].Callback.FatalError("Sala creada!!");
                }
                catch (Exception)
                {
                    SecureDeleteClient(clientsDictionary[name]);
                }
                return;
            }
            Console.WriteLine("Cliente desconocido: ({0}, {1}) quiere crear un juego!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: quien te conoce papa!");
            }
            catch (Exception) { }
        }

        public void Leave(string name, string GUID)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                SecureDeleteClient(clientsDictionary[name]);
                return;
            }
            Console.WriteLine("Esto no debio pasar!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: quien te conoce papá!");
            }
            catch (Exception) { }
        }

        private void SecureDeleteClient(Client client)
        {
            clientsDictionary.Remove(client.Name);
            client.LeaveGame();
            bool deleted = client.DeleteRoom();
            if (deleted)
            {
                Console.WriteLine("Cliente {0} borro el juego !", client.Name);
                List<Client> failed = new List<Client>();
                foreach (var iclient in clientsDictionary.Values)
                {
                    try
                    {
                        iclient.Callback.RoomDeleted(client.Name);
                    }
                    catch (Exception)
                    {
                        failed.Add(client);
                    }
                }
                SecureDeleteClients(failed);
            }
            Console.WriteLine("Cliente {0} abandono!", client.Name);
            return;
        }

        private void SecureDeleteClients(List<Client> clients)
        {
            if (clients.Count == 0)
            {
                return;
            }
            List<Client> failed = new List<Client>();
            foreach (var client in clients)
            {
                clientsDictionary.Remove(client.Name);
            }
            foreach (var client in clients)
            {
                client.LeaveGame();
                bool deleted = client.DeleteRoom();
                if (deleted)
                {
                    Console.WriteLine("Cliente {0} juego borrado!", client.Name);
                    foreach (var iclient in clientsDictionary.Values)
                    {
                        try
                        {
                            iclient.Callback.RoomDeleted(client.Name);
                        }
                        catch (Exception)
                        {
                            if (!failed.Contains(client))
                                failed.Add(client);
                        }
                    }
                    continue;
                }
                Console.WriteLine("Cliente {0} abandono!", client.Name);
            }
            SecureDeleteClients(failed);
        }

        public void DeleteRoom(string name, string GUID)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                bool deleted = clientsDictionary[name].DeleteRoom();
                if (deleted)
                {
                    Console.WriteLine("Cliente {0} borro juego!", name);
                    List<Client> failed = new List<Client>();
                    foreach (var client in clientsDictionary.Values)
                    {
                        try
                        {
                            client.Callback.RoomDeleted(name);
                        }
                        catch (Exception)
                        {
                            failed.Add(client);
                        }
                    }
                    SecureDeleteClients(failed);
                    return;
                }           
                if (clientsDictionary[name].HaveGame)
                {
                    Console.WriteLine("Cliente {0} esta jugando ahora!)", name);
                    return;
                }
                Console.WriteLine("Cliente {0} Quiere borrar el juego, pero el juego no existe", name);
                try
                {
                    clientsDictionary[name].Callback.FatalError("La Sala no existe!");
                }
                catch (Exception)
                {
                    SecureDeleteClient(clientsDictionary[name]);
                }
                return;
            }
            Console.WriteLine("Cliente desconocido: ({0}, {1}) quiere eliminar el juego", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: quien te conoce papá!");
            }
            catch (Exception) { }
        }

        public void JoinGame(string name, string GUID, string opponent_name)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                if (clientsDictionary.ContainsKey(opponent_name))
                {
                    if (clientsDictionary[opponent_name].HaveRoom && !clientsDictionary[opponent_name].HaveGame)
                    {
                        clientsDictionary[opponent_name].DeleteRoom();
                        Console.WriteLine("Cliente {0} elimino juego", name);
                        List<Client> failed = new List<Client>();
                        foreach (var client in clientsDictionary.Values)
                        {
                            //if (client.Name == opponent_name)
                            //    continue;
                            try
                            {
                                client.Callback.RoomDeleted(opponent_name);
                            }
                            catch (Exception)
                            {
                                failed.Add(client);
                            }
                        }
                        SecureDeleteClients(failed);
                        try
                        {
                            clientsDictionary[name].Callback.PrepareToGame(opponent_name);
                        }
                        catch (Exception)
                        {
                            SecureDeleteClient(clientsDictionary[name]);
                            return;
                        }
                        clientsDictionary[name].JoinTo(clientsDictionary[opponent_name]);
                        try
                        {
                            clientsDictionary[opponent_name].Callback.PrepareToGame(name);
                        }
                        catch (Exception)
                        {
                            SecureDeleteClient(clientsDictionary[opponent_name]);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            clientsDictionary[name].Callback.GameNotExists();
                        }
                        catch (Exception)
                        {
                            SecureDeleteClient(clientsDictionary[name]);
                            return;
                        }
                    }
                    return;
                }
                Console.WriteLine("Cliente {0} quizo unirse pero el rival no existe", name);
                try
                {
                    clientsDictionary[name].Callback.FatalError("Oponente no existe!");
                }
                catch (Exception)
                {
                    SecureDeleteClient(clientsDictionary[name]);
                }
                return;
            }
            Console.WriteLine("Cliente desconocido: ({0}, {1}) quiere unirse al juego!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("el Servidor te dice: quien te conoce papá!");
            }
            catch (Exception) { }
        }

        public void ReadyForGame(string name, string GUID, bool[] field)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                if (!clientsDictionary[name].HaveGame)
                {
                    Console.WriteLine("Cliente {0} no pertenece al juego!", name);
                    try
                    {
                        clientsDictionary[name].Callback.FatalError("Servidor: nadie quiere jugar con vos!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                if (field.Length != 100)
                {
                    Console.WriteLine("Cliente {0} envio un tablero erroneo (size != 100)!", name);
                    try
                    {
                        clientsDictionary[name].Callback.BadField("Tamaño incorrecto del tablero!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                int[] ships_count = new int[5];
                byte[] cell_status = new byte[100];
                for (int i = 0; i < 100; i++)
                {
                    if (field[i] && cell_status[i] == 2)
                    {
                        Console.WriteLine("Cliente {0} envio un tablero inavalido (Los barcos no se pueden tocar)!", name, GUID);
                        try
                        {
                            clientsDictionary[name].Callback.BadField("Los barcos no se pueden tocar! sino explotan!");
                        }
                        catch (Exception)
                        {
                            SecureDeleteClient(clientsDictionary[name]);
                        }
                        return;
                    }
                    if (field[i] && cell_status[i] == 0)
                    {
                        int horizontal_length = 1;
                        cell_status[i] = 1;
                        if (i < 90)
                            cell_status[i + 10] = 2;
                        int j = i+1;
                        while (j % 10 != 0 && field[j])
                        {
                            horizontal_length += 1;
                            cell_status[j] = 1;
                            if (j < 90)
                                cell_status[j + 10] = 2; //2 es un entorno de buques
                            j++;
                        }
                        if (j % 10 != 0)
                        {
                            cell_status[j] = 2;
                            if (j < 90)
                                cell_status[j + 10] = 2;
                        }
                        if (horizontal_length > 1)
                        {
                            if (horizontal_length > 4)
                            {
                                Console.WriteLine("Cliente {0} coloco un barco demasiado largo > 4!", name, GUID);
                                try
                                {
                                    clientsDictionary[name].Callback.BadField("Eso no es barco es un Mounstruo! Tamaño de barco incorrecto");
                                }
                                catch (Exception)
                                {
                                    SecureDeleteClient(clientsDictionary[name]);
                                }
                                return;
                            }
                            ships_count[horizontal_length]++;
                        }
                        else
                        {
                            int vertical_length = 1;
                            cell_status[i] = 1;
                            if (i % 10 != 0)
                                cell_status[i - 1] = 2;
                            if (i % 10 != 9)
                                cell_status[i + 1] = 2;
                            j = i+10;
                            while (j < 100 && field[j])
                            {
                                vertical_length += 1;
                                cell_status[j] = 1;
                                if (j % 10 != 0)
                                    cell_status[j - 1] = 2;
                                if (j % 10 != 9)
                                    cell_status[j + 1] = 2;
                                j += 10;
                            }
                            if (j < 100)
                            {
                                cell_status[j] = 2;
                                if (j % 10 != 0)
                                    cell_status[j - 1] = 2;
                                if (j % 10 != 9)
                                    cell_status[j + 1] = 2;
                            }
                            if (vertical_length > 4)
                            {
                                Console.WriteLine("Cliente {0} coloco un barco demasiado largo  > 4!", name, GUID);
                                try
                                {
                                    clientsDictionary[name].Callback.BadField("Eso no es barco es un Mounstruo! Tamaño de barco incorrecto");
                                }
                                catch (Exception)
                                {
                                    SecureDeleteClient(clientsDictionary[name]);
                                }
                                return;
                            }
                            ships_count[vertical_length]++;
                        }
                    }
                }
                if (ships_count[1] != 4)
                {
                    Console.WriteLine("Cliente {0} envio un tablero con un numero incorrecto de barcos  (número de barcos equivocadas)!", name, GUID);
                    try
                    {
                        clientsDictionary[name].Callback.BadField("A menos que seas EEUU con su flota pacificadora, no podes tener tantos barcos!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                if (ships_count[2] != 3)
                {
                    Console.WriteLine("Client {0}envio un tablero con un numero incorrecto de barcos  (número de barcos equivocadas)!", name, GUID);
                    try
                    {
                        clientsDictionary[name].Callback.BadField("A menos que seas EEUU con su flota pacificadora, no podes tener tantos barcos!!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                if (ships_count[3] != 2)
                {
                    Console.WriteLine("Cliente {0} envio un tablero con un numero incorrecto de barcos  (número de barcos equivocadas)!", name, GUID);
                    try
                    {
                        clientsDictionary[name].Callback.BadField("A menos que seas EEUU con su flota pacificadora, no podes tener tantos barcos!!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                if (ships_count[4] != 1)
                {
                    Console.WriteLine("Cliente {0} envio un tablero con un numero incorrecto de barcos (número de barcos equivocadas)!", name, GUID);
                    try
                    {
                        clientsDictionary[name].Callback.BadField("A menos que seas EEUU con su flota pacificadora, no podes tener tantos barcos!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                Console.WriteLine("Client {0} envio un tablero correcto!", name, GUID);
                try
                {
                    clientsDictionary[name].Callback.GoodField();
                }
                catch (Exception)
                {
                    SecureDeleteClient(clientsDictionary[name]);
                    return;
                }
                clientsDictionary[name].SetField(field);
                if (clientsDictionary[name].IsOpponentReady())
                {
                    try
                    {
                        clientsDictionary[name].Callback.StartGame();
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                        return;
                    }
                    try
                    {
                        clientsDictionary[clientsDictionary[name].Opponent()].Callback.StartGame();
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    try
                    {
                        clientsDictionary[clientsDictionary[name].Opponent()].Callback.YouTurn();
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }

                    return;
                }
                return;
            }
            Console.WriteLine("cliente desconocido: ({0}, {1}) quiere unirse al juego!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: no te conozco!");
            }
            catch (Exception)
            { }
        }

        public void SendMessage(string name, string GUID, string text)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                if (!clientsDictionary[name].HaveGame)
                {
                    Console.WriteLine("Cliente {0} no esta jugando ahora!)", name);
                    try
                    {
                        clientsDictionary[name].Callback.FatalError("Usted no está jugando ahora!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                Console.WriteLine("Cliente {0} envio un mensajae!", name);
                clientsDictionary[name].SendMessage(text);
                return;
            }
            Console.WriteLine("cliente desconocido: ({0}, {1}) quiere enviar mensaje!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: no te conozco!");
            }
            catch (Exception) { }
        }

        public void Turn(string name, string GUID, ShootType type, int x, int y)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                if (!clientsDictionary[name].HaveGame)
                {
                    Console.WriteLine("Cliente {0} no esta jugando ahora!)", name);
                    try
                    {
                        clientsDictionary[name].Callback.FatalError("Usted no está jugando ahora!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                Console.WriteLine("Cliente {0} hizo un turno!", name);
                clientsDictionary[name].DoTurn(x, y);
                return;
            }
            Console.WriteLine("Cliente desconocido: ({0}, {1}) quiere disparar (todos quieren)!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("el Servidor dijo: no te conozco!");
            }
            catch (Exception) { }
        }

        public void LeaveGame(string name, string GUID)
        {
            if (clientsDictionary.ContainsKey(name) && clientsDictionary[name].CheckGUID(GUID))
            {
                if (!clientsDictionary[name].HaveGame)
                {
                    Console.WriteLine("Cliente {0} no esta jugando ahora!", name);
                    try
                    {
                        clientsDictionary[name].Callback.FatalError("Usted no está jugando ahora!");
                    }
                    catch (Exception)
                    {
                        SecureDeleteClient(clientsDictionary[name]);
                    }
                    return;
                }
                Console.WriteLine("Client {0} abandono el juego!", name);
                clientsDictionary[name].LeaveGame();
                return;
            }
            Console.WriteLine("Cliente desconocido: ({0}, {1}) quiere abandonar el juego!", name, GUID);
            try
            {
                OperationContext.Current.GetCallbackChannel<IClientCallback>().FatalError("Servidor: no te conozco!");
            }
            catch (Exception) { }
        }
    }
}
