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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace battleship_common
{
    [DataContract]
    public class Message
    {
        [DataMember]
        private string message;

        [DataMember]
        private string author;

        [DataMember]
        private DateTime creationTime;

        public Message(string author, DateTime creationTime, string message)
        {
            this.author = author;
            this.message = message;
            this.creationTime = creationTime;
        }

        [DataMember]
        public string Author
        {
            get { return author; }
            set { }
        }

        [DataMember]
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { }
        }

        [DataMember]
        public string Text
        {
            get { return message; }
            set { }
        }
    }
}
