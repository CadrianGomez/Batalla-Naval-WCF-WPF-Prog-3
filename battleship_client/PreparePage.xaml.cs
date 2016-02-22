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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace battleship_client
{

    class SmartField
    {
        public SmartField()
        {
            field = new bool[100];
            for (int i = 0; i < 100; i++)
            {
                field[i] = false;
            }
        }
        private bool[] field;
        public void Set(int x, int y)
        {
            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                field[x + y * 10] = true;
            }
        }
        public bool Get(int x, int y)
        {
            if (x >= 0 && x < 10 && y >= 0 && y < 10)
            {
                return field[x + y * 10];
            }
            return true;
        }
    }


    public partial class PreparePage : UserControl
    {
        private Main main;
        private bool []field;
        private bool ready;
        private string opponent_name;
        int Acorazado = 3, Submarino = 2, PortaAviones = 1, Bote = 4;
        int Ships = 10;
        
        public PreparePage(Main main, string opponent_name, string initial_chat)
        {
            
            InitializeComponent();
            this.main = main;
            field = new bool[100];
            Greetings.Content = "Por favor, pon tus barcos. Está jugando con" + opponent_name;
            textBlockMessages.Text = initial_chat;
            ready = false;
            this.opponent_name = opponent_name;
            rbAcorazado.IsChecked = true;
            rbHorizontal.IsChecked = true;
        }

        public void PostMessage(string message)
        {
            textBlockMessages.Text = textBlockMessages.Text + message + "\n";
        }

        public void Retry()
        {
        }

        private void Cell_LeftClick(object sender, RoutedEventArgs e)
        {
            if (ready)
                return;
            //Rectangle cell = sender as Rectangle;
            //field[int.Parse(cell.Name.Remove(0, 4))] = true;
            //Style style = this.FindResource("FilledCell") as Style;
            //cell.Style = style;
            if (Ships != 0)
            {
                if (rbBote.IsChecked == true && Bote != 0)
                {
                    Rectangle cell = sender as Rectangle;
                    field[int.Parse(cell.Name.Remove(0, 4))] = true;
                    Style style = this.FindResource("FilledCell") as Style;
                    cell.Style = style;
                    Bote--;
                    Ships--;
                }

                if (rbPortaAviones.IsChecked == true && PortaAviones != 0)
                {
                    if (rbHorizontal.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos < 9)
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());

                        }
                        if (y <= 6)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda2 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda3 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            celda2.Style = style;
                            celda3.Style = style;
                            PortaAviones--;
                            Ships--;

                        }
                    }
                    if (rbVertical.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos < 9)
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());

                        }
                        if (x <= 6)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda2 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda3 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            celda2.Style = style;
                            celda3.Style = style;
                            PortaAviones--;
                            Ships--;

                        }


                    }
                }




                if (rbSubmarino.IsChecked == true && Submarino != 0)
                {
                    if (rbHorizontal.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos < 9)
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());

                        }
                        if (y <= 7)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda2 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            celda2.Style = style;
                            Submarino--;
                            Ships--;

                        }
                    }
                    if (rbVertical.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos < 9)
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());
                        }
                        if (x <= 7)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda2 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            celda2.Style = style;
                            Submarino--;
                            Ships--;

                        }


                    }

                }



                if (rbAcorazado.IsChecked == true && Acorazado != 0)
                {
                    if (rbHorizontal.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos <9 )
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());

                        }
                        if (y <= 8)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 1;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            Acorazado--;
                            Ships--;

                        }
                    }
                    if (rbVertical.IsChecked == true)
                    {
                        int x, y;
                        Rectangle cell = sender as Rectangle;
                        int pos = int.Parse(cell.Name.Remove(0, 4));
                        if (pos <9)
                        {
                            x = 0;
                            y = pos;
                        }
                        else
                        {
                            string value = cell.Name.Remove(0, 4);
                            x = int.Parse(value[0].ToString());
                            y = int.Parse(value[1].ToString());
                        }
                        if (x <= 8)
                        {
                            field[pos] = true;
                            Style style = this.FindResource("FilledCell") as Style;
                            pos += 10;
                            field[pos] = true;
                            Rectangle celda1 = battleField.FindName("cell" + pos) as Rectangle;
                            cell.Style = style;
                            celda1.Style = style;
                            Acorazado--;
                            Ships--;

                        }


                    }

                }
            }
            else
            {
                MessageBox.Show("No te quedan mas barcos");

            }

        }

        private void Cell_RightClick(object sender, RoutedEventArgs e)
        {
            //if (ready)
            //    return;
            //Rectangle cell = sender as Rectangle;
            //field[int.Parse(cell.Name.Remove(0, 4))] = false;
            //Style style = this.FindResource("EmptyCell") as Style;
            //cell.Style = style;
        }

        private void Cell_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ready)
                return;

            if (Ships != 0)
            {

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //Rectangle cell = sender as Rectangle;
                    //field[int.Parse(cell.Name.Remove(0, 4))] = true;
                    //Style style = this.FindResource("FilledCell") as Style;
                    //cell.Style = style;


                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    //Rectangle cell = sender as Rectangle;
                    //field[int.Parse(cell.Name.Remove(0, 4))] = false;
                    //Style style = this.FindResource("EmptyCell") as Style;
                    //cell.Style = style;
                }
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
                MessageBox.Show("Mensaje vacío...", "Try again...", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ready = true;
            ReadyButton.IsEnabled = false;
            RandomButton.IsEnabled = false;
            main.CheckField(field);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Random rand = new Random((int)DateTimeOffset.Now.UtcTicks);
            int ship_length = 4;
            for (int i = 0; i < 100; i++)
            {
                if (field[i])
                {
                    Rectangle cell = (Rectangle)FindName(string.Format("cell{0}", i));
                    cell.Style = this.FindResource("EmptyCell") as Style;
                    field[i] = false;
                }
            }
            SmartField sf = new SmartField();
            while (ship_length > 0)
            {
                for (int i = 0; i < 5 - ship_length; i++)
                {
                    int dx = 0, dy = 0, maxx = 10, maxy = 10;
                    if (rand.Next() % 2 == 0)
                    {
                        dx = 1;
                        maxx = 10 - ship_length + 1;
                    }
                    else
                    {
                        dy = 1;
                        maxy = 10 - ship_length + 1;
                    }
                    bool placed = false;
                    while (!placed)
                    {
                        int x = rand.Next(maxx);
                        int y = rand.Next(maxy);
                        bool empty = true;
                        for (int j = 0; j < ship_length; j++)
                        {
                            if (sf.Get(x + dx * j, y + dy * j))
                            {
                                empty = false;
                            }
                        }
                        if (empty)
                        {
                            placed = true;
                            for (int j = 0; j < ship_length; j++)
                            {
                                int place = x + dx * j + (y + dy * j) * 10;
                                field[place] = true;
                                Rectangle cell = (Rectangle)FindName(string.Format("cell{0}", place));
                                cell.Style = this.FindResource("FilledCell") as Style;
                            }
                            int k, l;
                            for (k = x - 1; k <= x + dx * (ship_length-1) + 1; k++)
                            {
                                for (l = y - 1; l <= y + dy * (ship_length-1) + 1; l++)
                                {
                                    sf.Set(k, l);
                                }
                            }
                        }
                    }
                }
                ship_length--;
            }
            Ships = 0;
        }

        public void GoodField()
        {
            MessageBox.Show("Por favor, espera a tu oponente!", "Bien!", MessageBoxButton.OK, MessageBoxImage.Information);
            Greetings.Content = "¡Bien! Espere a su oponente. Está jugando con " + opponent_name;
        }

        private void button_Click_4(object sender, RoutedEventArgs e)
        {
            Random rand = new Random((int)DateTimeOffset.Now.UtcTicks);
            Acorazado = 3;
            Submarino = 2;
            PortaAviones = 1;
            Bote = 4;
            Ships = 10;
            for (int i = 0; i < 100; i++)
            {
                if (field[i])
                {
                    Rectangle cell = (Rectangle)FindName(string.Format("cell{0}", i));
                    cell.Style = this.FindResource("EmptyCell") as Style;
                    field[i] = false;
                }
            }
        }

        public void BadField(string message)
        {
            MessageBox.Show(message, "Colocacion errónea...", MessageBoxButton.OK, MessageBoxImage.Warning);
            ready = false;
            ReadyButton.IsEnabled = true;
            RandomButton.IsEnabled = true;
        }

        public GamePage GetGamePage()
        {
            return new GamePage(main, opponent_name, textBlockMessages.Text, field);

        }

        private void rbSubmarino_Checked(object sender, RoutedEventArgs e)
        {

        }


    }
}
