   M S B A M L   `   `   `  ����  battleship_client!    battleship_client.RoomsPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      $��Stretch=�5      $��Stretch=�6=   $	��320��6Y   $	��355��6i   .��5      � -   5        $
��grid��5      ��5   
   �� 5	        �� ImageSource$;  5/Resources/fk_battle___fk_vs__dqs_ii_by_ondraedan.jpg��6    � ColumnDefinitions 5   
   �� 5      $	��41* �6   �� 5      $	��30* �6    � RowDefinitions 5   
   �� 5      $��60 �6   �� 5      $��54 �6   �� 5      �� 5      $��60 �6   .+�5   
   �� $��16(�6   $��Juegos Disponibles:��6   $��Stretch=�6<   $��
10,10,10,0q�6Z   $	��Top=�6n   $��34��6�    J� RenderTransformOrigin$ -2.328,-1.071&�6�   $��1��6�   $��2��6�   �� 5   
   -   5        $��createButton��5      $��20(�6%   $��Crear��63   $��Stretch=�6C   $��
10,10,10,0q�6a   $	��Top=�6u   $��40��6�   $��3��6�   &   System.Windows.Controls.DataGrid  5   
   -   5        $��	roomsGrid��5         AutoGenerateColumns .  $��Stretch=�6X   $��Stretch=�6v   $��10q�6�   $��2��6�      CanUserAddRows .  $��2��6�    J� Opacity$ 0.655Q�6�   ��6$   ��    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Jugador��6%   	   CanUserReorder	 . 
   CanUserResize
 .    CanUserSort .    Width5  /System.Windows.Controls.DataGridLengthConverter$ * 5   @      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 5   6   �� *Name+5   J     5      $ Timpo de Creacion��6%   	 . 
 .  . $ * 5   @    .  5   @   �� *CreationTime+5   J   5      �� 5   
   -   5        $��
joinButton��5      $��20(�6#   $��Unirse��61   $��Stretch=�6B   $��
10,10,10,0q�6`   $	��Top=�6t   $��40��6�   $��3��6�   $��1��6�   �� 5    
   -   5        $��welcome��5       $��BATALLA NAVAL��6   $��20(�67   $��Center=�6E   $��Stretch=�6i   $��
10,10,10,0q�6�   $	��Top=�6�   $��40��6�   $��2��6�   $��Stencil)�6�   5!      5"      