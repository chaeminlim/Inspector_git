   M S B A M L   `   `   `  ����D  @Inspector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Inspector	Inspector  5   	       Inspector.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:Inspector     5   	     � Title$  
MainWindow��5   	   $��635.6��6   $��1036.4��6+   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��0.5* �6   �� 5      $	��10* �6   .+�5   
   � $��0��6   .+�5      c� .@�5      �� -   5         ��Name$ CallSelector��5      $��Selector��6/   $
��Left=�6B   $	��Top=�6^   6v   �� 5      -   5        $ 	changexml��5      $��Change into xml��6,   $
��Left=�6G   $	��Top=�6c   6|   �� 5      -   5        $ 	FindByXml��5      $��	FindByXml��6,   $
��Left=�6@   $	��Top=�6\   6t   �� 5      -   5        $ CallHandler��5      $��CallHandler��6.   $
��Left=�6D   $	��Top=�6_   6w   �� 5      -   5        $ Recorder��5      $��Recorder��6+   $
��Left=�6>   $	��Top=�6Y   6q   �� 5      -   5        $ ClearRecordList��5      $��ClearRecordList��62   $
��Left=�6L   $	��Top=�6g   6   �� 5      -   5        $ MessageSender��5      $��MessageSender��60   $
��Left=�6H   $	��Top=�6d   6}   5      5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��1* �6#   �� 5      $��2* �6#   .+�5!      � $��0��6    5"      �� 5#      $��1* �6$   �� 5$      $��1* �6$   .+�5&      R� -   5         R�Name$ 	treeView1��5&   (   $��0��6   $��0,0,0,0q�6p   �� 5'      -	   5         ��Name$ 	listView1��5'   )   $��1��6   $
��Left=�6<   $	��Top=�6X   $��0,0,0,0q�6p   5(      � 5)      $��1��6    5*      �� 5+      $��1* �6$   �� 5,      $��1* �6$   .+�5.      �� $��0��6   $
��Left=�6*   $��10,0,0,0q�6E   $	��Top=�6W   .@�5/      �� $	��Xml��6   .��50      �� -
   5         ��Name$ XmlBox��50   $   51      �� 52      $��Record��6   .��53      �� -   5         ��Name$ 
RecordList��53   "   54      55      56      �� 57      -   5        $ 	listView2��57   (   $��1��6   $��5,5,0,0q�6<    �� View 58      �� 59      .P�5:      �� 5;      5=      5?      5@      5A      5B      