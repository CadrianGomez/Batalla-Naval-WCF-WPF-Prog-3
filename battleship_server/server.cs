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
using System.Configuration;
using System.ServiceModel;

namespace battleship_server
{
    class Server
    {
        static void Main(string[] args)
        {
            Uri uri = new Uri(ConfigurationManager.AppSettings["addr"]);
            ServiceHost host = new ServiceHost(typeof(BattleshipService), uri);
            host.Open();
            Console.WriteLine("Presione la Tecla ENTER para salir");
            Console.ReadLine();
            host.Abort();
            host.Close(); 
        }
    }
}
