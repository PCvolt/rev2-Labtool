   M S B A M L   `   `   `  ����R  Nrev2-Labtool(Framework), Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:rev2_Labtool_Framework_rev2_Labtool_Framework_  5   	   (    "rev2_Labtool_Framework_.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:rev2_Labtool_Framework_     5   	   -   5          � Title$  rev2-Labtool (v0.1.1)��5   	   $	��307��6'   $	��397��64    � WindowStartupLocation$ CenterScreen=�5
       � Topmost . .��5      ]� .+�5      o� $	��Top=�6	   .@�5      m� -   5        $��Github��5      67   5      � 5      $��0,0,0,10q�6	   .+�5      �� $
��Info��6   $��10,0,10,140q�6   .��5      �  � ColumnDefinitions 5      �� 5      $	��188 �6   �� 5      $
��188* �6   .+�5      �� -   5         ��Name$ p1CharLabel��5      $��Potemkin��6"   $
��Left=�65   $��	10,10,0,0q�6P   $	��Top=�6c   �� 5      -   5        $ p1DefModifLabel��5      $��[x1.00]��6&   $
��Left=�68   $��	64,10,0,0q�6S   $	��Top=�6f   �� 5      -   5        $ p1MeterLabel��5      $	��100��6#   $
��Left=�61   $��
123,10,0,0q�6L   $	��Top=�6`   �� 5      -   5        $ 	p1HPLabel��5      $��HP: 420��6!   $
��Left=�63   $��	10,36,0,0q�6N   $	��Top=�6a   �� 5      -   5        $ p1GutsLabel��5      $��[x1.00]��6#   $
��Left=�65   $��	64,36,0,0q�6P   $	��Top=�6c    J� RenderTransformOrigin$ 	1.889,0.5&�6{   �� 5      -   5        $ p1RISCLabel��5      $��0/128��6"   $
��Left=�62   $��	10,59,0,0q�6M   $	��Top=�6`   �� 5      -	   5        $ p1DizzyLabel��5      $��0/dizzy��6#   $
��Left=�65   $��
123,59,0,0q�6P   $	��Top=�6d   �� 5      -
   5        $ c��5      $��P1 pos��6   $
��Left=�6)   $��
132,41,0,0q�6D   $	��Top=�6X   5!      5"      �� 5#      $��
Frame Data��6   $��10,113,10,0q�6"   .��5$      � .+�5%      �� -   5        $ frameAdvantageLabel��5%      $��Frame Advantage: ��6*   $
��Left=�6F   $	��Top=�6a   $��0,10,0,0q�6y   �� 5&      -   5        $ 	gapsLabel��5&      $��Gaps:��6    $
��Left=�60   $��	36,33,0,0q�6K   $	��Top=�6^   $ 0.974,0.154&�6v   �� 5(      -   5        $ faLabel��5(      $�� ��6   $
��Left=�6)   $��
140,38,0,0q�6D   $	��Top=�6X   �� 5)      -   5         ��Name$ gapsTextblock��5)      $��
0,64,254,0q�6&   $�� ��6:    �� TextWrapping$
 Wrap=�6B    �� 
IsReadOnly . 6V   �� 5*      -   5        	 ��Name$	 clearButton��5*      $��Clear��6#   $
��Left=�63   $��0,36,0,0q�6N   $	��Top=�6`   6x   5+      5,      5-      5.      5/      