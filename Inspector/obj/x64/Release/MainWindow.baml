   M S B A M L   `   `   `  ���� D  @Inspector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Inspector	Inspector      Inspector.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:Inspector       � Title$  
MainWindow��$��635.6��$��1036.4��.���  � RowDefinitions �� $
��0.5* ��� $	��10* �.+�� $��0��.+�c� .@��� -    ��Name$ CallSelector��$��Selector��$
��Left=�$	��Top=��� -   $ 	changexml��$��Change into xml��$
��Left=�$	��Top=��� -   $ 	FindByXml��$��	FindByXml��$
��Left=�$	��Top=��� -   $ CallHandler��$��CallHandler��$
��Left=�$	��Top=��� -   $ Recorder��$��Recorder��$
��Left=�$	��Top=��� -   $ ClearRecordList��$��ClearRecordList��$
��Left=�$	��Top=��� -   $ MessageSender��$��MessageSender��$
��Left=�$	��Top=�� $��1�� � ColumnDefinitions �� $��1* ��� $��2* �.+�� $��0�� �� $��1* ��� $��1* �.+�R� -    R�Name$ 	treeView1��$��0��$��0,0,0,0q��� -	    ��Name$ 	listView1��$��1��$
��Left=�$	��Top=�$��0,0,0,0q�� $��1�� �� $��1* ��� $��1* �.+��� $��0��$
��Left=�$��10,0,0,0q�$	��Top=�.@��� $	��Xml��.���� -
    ��Name$ XmlBox���� $��Record��.���� -    ��Name$ 
RecordList���� -   $ 	listView2��$��1��$��5,5,0,0q� �� View �� .P��� 