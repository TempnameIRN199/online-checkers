   M S B A M L   `   `   `  ����C  ?Checkers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Checkers.ConvertersCheckers.Converters  5   
        Checkers.Controls.MainMenu   5      W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   
   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   
   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   
   @d2http://schemas.microsoft.com/expression/blend/2008     5   
   7	converter!clr-namespace:Checkers.Converters     5   
   $��Center=�5      $��Center=�6A     � 	Resources  5	      6   0Checkers.Converters.BooleanToVisibilityConverter   VisibilityConverter��%   &	          5
   
   61   5      .��5      ��  �� Orientation$ Vertical=�6   .+�5   
   �� -   5         ��Name$ 
checkLabel��5      ��5      �� 5      $��Checkers��6   $��25(�6,   $
��Bold"�6:   5      	� 5   
   -   5         	�Name$ friend��5       	� 	GroupName$ Opponent��6&   $��Against friend��6;    d� 	IsChecked$
 TrueY�6T   	� 5   
   -   5        $ computer��5      $ Opponent��6(   $��Against computer��6=   �� 5   
   $ Vertical=�6�   y�6   ��  �� ElementName$ computer�� �� Path$ 	IsChecked� �� 	Converter# [  .+�5      �� 	� 5      -   5        $ white��5      $ Color��6)   $��Start as white��6;   $
 TrueY�6T   	� 5      -   5        $ black��5      $ Color��6)   $��Start as black��6;   5      �� 5   
   -   5        	 ��Name$	 startButton��5      $��5q�6&   .��61   
Start Game6=   �� 5   
   -   5        $	 ConnectButton��5      $��5q�6(   .��63   	Connect6<   5      5      