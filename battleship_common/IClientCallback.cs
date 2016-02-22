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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace battleship_common
{
    public enum ShootType {Shoot};

    public enum Cell
    {
        Empty, Ship, Fire, DeadShip, Missed
    }

    [ServiceContract]
    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void LogIn(string GUID);

        [OperationContract(IsOneWay = true)]
        void UserNameExists();

        [OperationContract(IsOneWay = true)]
        void RoomCreated(Room room);

        [OperationContract(IsOneWay = true)]
        void RoomDeleted(string name);

        [OperationContract(IsOneWay = true)]
        void FatalError(string error);

        [OperationContract(IsOneWay = true)]
        void GameNotExists();

        [OperationContract(IsOneWay = true)]
        void PrepareToGame(string opponent_name);

        [OperationContract(IsOneWay = true)]
        void TransferMessage(Message mess);

        [OperationContract(IsOneWay = true)]
        void GoodField();

        [OperationContract(IsOneWay = true)]
        void BadField(string comment);

        [OperationContract(IsOneWay = true)]
        void StartGame();

        [OperationContract(IsOneWay = true)]
        void YouTurn();

        [OperationContract(IsOneWay = true)]
        void UpdateYourField(int x, int y, Cell state);

        [OperationContract(IsOneWay = true)]
        void AlreadyClicked();

        [OperationContract(IsOneWay = true)]
        void UpdateOpponentField(int x, int y, Cell state);

        [OperationContract(IsOneWay = true)]
        void Win();

        [OperationContract(IsOneWay = true)]
        void Loose();

        [OperationContract(IsOneWay = true)]
        void YouCheated();
    }
}

