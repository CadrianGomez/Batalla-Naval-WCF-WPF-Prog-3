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
using System.Collections.Generic;
using System.ServiceModel;


namespace battleship_server
{
    [ServiceContract(CallbackContract = typeof(IClientCallback), SessionMode = SessionMode.Required)]
    public interface IBattleshipService
    {
        [OperationContract(IsOneWay = true)]
        void Join(string name);

        [OperationContract(IsOneWay = true)]//, IsTerminating = true)]
        void Leave(string name, string GUID);

        [OperationContract(IsOneWay = true)]
        void CreateRoom(string name, string GUID);

        [OperationContract(IsOneWay = true)]
        void DeleteRoom(string name, string GUID);

        [OperationContract(IsOneWay = true)]
        void JoinGame(string name, string GUID, string oponent_name);

        [OperationContract(IsOneWay = true)]
        void LeaveGame(string name, string GUID);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string name, string GUID, string message);

        [OperationContract(IsOneWay = true)]
        void ReadyForGame(string name, string GUID, bool []field);

        [OperationContract(IsOneWay = true)]
        void Turn(string name, string GUID, ShootType type, int x, int y);
    }
}


