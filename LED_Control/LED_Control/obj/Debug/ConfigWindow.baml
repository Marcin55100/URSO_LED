   M S B A M L   `   `   `  ����F  BLED_Control, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:LED_ControlLED_Control  5   	       LED_Control.ConfigWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:LED_Control     5   	     � Title$  Konfiguracja sieci��5   	   $	��350��6$   $	��525��61   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ listBox��5      $��0��6#   $
��Left=�6_   $	��230��6z   $��	10,10,0,0q�6�   $	��Top=�6�   $	��211��6�   �� 5   
   -   5         ��Name$ refreshButton��5      $��	Odśwież��6(   $
��Left=�6:   $��
80,259,0,0q�6U   $	��Top=�6i   $��75��6�   6�   �� 5   
   -   5         ��Name$ networkPanel��5      $��Center=�6+   $��248,10,10,191q�6F   $��2��6]   $y�Hidden=�6q   .+�5      �� -   5         ��Name$ passwordLabel��5      $��Podaj hasło:��6+   G� 5      -   5         G�Name$ passwordBox��5       G� PasswordChar$ *��6/   $
��Left=�6@   $��23��6[   $	��Top=�6g   $	��120��6   �� 5      -   5        $ connectButton��5      $��Połącz��6,   $
��Left=�6=   $	��Top=�6Y   $��75��6q   6|   �� 5      -   5        $ changeButton��5      $��Przełącz sieć��6+   $
��Left=�6C   $	��Top=�6^   $��75��6v   6�   �� 5      -   5        $ 	infoLabel��5      $!��Łączę ze sterownikiem...��6'   5      �� 5   
   $��1��6   $��Bottom=�6%   $��23��6@    �� Orientation$ 
Horizontal=�6L   .+�5      �� -	   5        $ 
backButton��5      $��Gotowe��6)   $��74��6<   $��23��6`   $	��Top=�6l   �� 5      -
   5        $ 
nextButton��5      $��Dalej��6)   $��40,0,0,0q�6:   $��74��6L   6W   5      5      5      