   M S B A M L   `   `   `  ����  Test0    *Paelife.KinectFramework.Samples.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
MainWindow��5   	   $	��400��6   $	��600��6)   .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $
��Auto �6   �� 5	      $
��Auto �6   .+�5   
   (� $��0��6    (� Stretch$ Uniform=�6   .��5      �� -   5        $��image1��5      $	��640��6"   $	��480��6.   5      �� 5   
   -   5        $��textBoxMessagesCenter��5   J   $��0��6   $��64��6!   $��Center=�6-   $��Center=�6g   $��DISCONNECTED��6�   $
��Bold"�6�   $��48(�6�   $y�	Collapsed=�6�   
���  ��6�   �� 5   
   -   5        $��textBoxUsername��5   :   $��1��6   $
��Left=�6   $��5,0,0,0q�6Q   $��Center=�6b   $	��120��6}   $��insert your username��6�   �� 5   
   -   5        $��buttonSaveFace��5   l   $��1��6   $��	Save Face��6   $��23��62   $
��Left=�6>   $��	130,0,0,0q�6Y   $��Center=�6�   $	��100��6�   6�   �� 5   
   -   5        $��buttonRecognizeFace��5   _   $��1��6   $��Recognize Face��6   $��23��67   $��Right=�6C   $��Center=�6z   $��0,0,5,0q�6�   $	��100��6�   6�   �� 5   
   -   5        $��textBlockFaceRecognition��5   ]   $��1��6   $��23��6!   $
��Left=�6-   $��240,10,0,10q�6H   $��Face recognition ready.��6}   $��Center=�6�   $
��Bold"�6�   �� 5   
   -   5        $��textBlockStatus��5   <   $��2��6   $
��Left=�6!   $"��Number of Detected Users = 0��6S   $��Center=�6w   $
��Bold"�6�   $��14(�6�   �� 5   
   -	   5        $��textBlockFace��5   =   $��2��6   $��Right=�6!   $!��Face Tracking Status = None��6R   $��Center=�6u   $
��Bold"�6�   $��14(�6�   5      5      