   M S B A M L   `   `   `  ����D  @Messenger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Messenger.ViewsMessenger.Views  5      U QMaterialDesignThemes.Wpf, Version=3.0.1.920, Culture=neutral, PublicKeyToken=nulleHclr-namespace:MaterialDesignThemes.Wpf;assembly=MaterialDesignThemes.WpfMaterialDesignThemes.Wpf 5
          Messenger.Views.MainForm   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      FmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      /localclr-namespace:Messenger.Views     5      Y UPrism.Unity.Wpf, Version=7.2.0.1422, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59S OPrism.Wpf, Version=7.2.0.1422, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5	      RwpfHclr-namespace:MaterialDesignThemes.Wpf;assembly=MaterialDesignThemes.Wpf  5
      -   5        !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel  .    MaterialDesignFont#���   5      $	��410��5      $	��500��6   .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Loaded��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *MainFormControlFormLoaded+5      .��5      �� z�5   
   �� 5      -   5        zoom��5      .+�5   
   �� -   5         ��Name$( "DimBackgroundWhileLoadingRectangle��5      
�������$	��500��6Q   $	��390��6]    �� Left$ 0��6j    K� ZIndex$ 1��6z    J� Opacity$	 0.9Q�6�   y�6�   �� *DimBackgroundWhileLoading+	 �� FallbackValue$	 	Collapsed��� 5   
   -   5        
 �Name$!
 DeterminateCircularProgress��5       % !MaterialDesignCircularProgressBar#��[ 5      $	��150��5      $	��150��6'   $	 175��5      
 �� Top$	 130��6,   $ 1��6=    � IsIndeterminate . y�5       �� *DimBackgroundWhileLoading+$	 	Collapsed���� 5%   
   $
��Left=�6   $	��410��5&      $��Stretch=�5'      $	��500��5(      .+�5)      � $	��200��6   $	��500��6    $��5q�6,    � ColumnDefinitions 5*      �� 5+      $��1* �6'   �� 5,      $��80 �6'   .+�5.      � $��1��6    � RowDefinitions 5/      �� 50      $��45 �6(   �� 51      $��45 �6(   .+�53      #  MaterialDesignThemes.Wpf.Card  $��	0,0,7.5,0q�6*   $��Center=�6=   $	��Top=�6Z   +  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth1=�6r   $��60��6�   $��40��6�   ���	#FFC3C3C3.��54      �  55      �� 56   "   $��1* �60   �� 57   "   $��1* �60   .+�59      �� $��参加者数��6(   $��WrapWithOverflow=�64   $��Center=�6T   $��Center=�6q   $
��Bold"�6�   $��12(�6�   �� 5:      $��1��6(   $��WrapWithOverflow=�6R   $��Center=�6r   $��Center=�6�   $
��Bold"�6�   $��12(�6�   ��65   �� *ClientCount+6�   5;      5<      �� 5=      $��1��6   $��最新の情
報に変更��6*   $��0q�6e   $��	0,0,7.5,0q�6q   $��60��6�   $��40��6�   $
��Bold"�6�   $��12(�6�   ��6B   �� *RefreshCommand+5C      5D      �� 5E      $��1q�6    �� CornerRadius$ 4��6-   
���   �$	��420��6R   $
��Left=�6^   ._�5F      �� -   5        $��scrollViewer��5F   #   $	��200��67   $	��420��6D   $
��Auto=�6P   $
��Auto=�6s   .��5G      ��  �� SelectedValuePath$ Index��5H   `   ��5G   #   �� *ParticipantList+ �� SelectedIndex 5H   #   �� *ListViewSelectedIndex+ �� Mode$ TwoWay=���5I      �� 5J   "    �� 
TargetType" ��6(   .�5K   &   ��  �� Property � �� �� Value$
 Left=�6S   �� 5L   &    � ��$ 30��6?   �� 5M   &    � �� 5N   *   �� 5O   .    �� 
TargetType" ��6>   .R�5Q   2   ��  ��Name$ Border��69   $��0,-1,0,0q�6I   #��z��5R   5     MaterialDesignDivider#���  5S   5   $��0,0,0,1q�5T   5    J� SnapsToDevicePixels# z 5U   5   ._�5V   6   �� #��z��6K   $��0q�6�   5W   3    �� Triggers 5X   2   P� 5Y   6    P� Property �@��
IsSelected P� Value . . �5Z   :   �� # � ��6W    � ��5[   7   P� 5\   6    �@J�IsMouseOver . . �5]   :   �� # � ��6W    � ��5^   7   5`   /   5b   '   5c   #    � 	Resources 5e        HeaderStyle��%;  &	       �� 5f   "   " ��6<   .�5g   &   ��  � ��$ Center=�6S   �� 5h   &    � ��
 �����6C   �� 5i   &    � ��
 �����6C   �� 5j   &    � ��$ 14(�6A   �� 5k   &    � ��$ 	Meirio UI)�6C   �� 5l   &    � ��$
 Bold"�6C   �� 5m   &    � ��$ 30��6?   5o   #   5p       �� View 5q      �� 5r   "   .P�5s   &   �� $��ユーザー��65    �� Width$	 100��6C     �� HeaderContainerStyle#  �  6O   ! �� CellTemplate! 5t   *   �� 5u   .   .R�5v   2   �� $
��user��6<   " �� TextAlignment$" Center=�6b   $��14(�6y   $��5q�6�   # �� Padding$# 3q�6�   ��6H   �� *
UserName+6�   5�   /   5�   '   �� 5�   &   $��グループ��65   $	 100��6C   #  �  6O   ! 5�   *   �� 5�   .   .R�5�   2   �� $��group��6<   $
" Left=�6d   $��14(�6y   $��5q�6�   $# 3q�6�   ��6I   �� *	GroupName+6�   5�   /   5�   '   �� 5�   &   $��	ホスト��65   $	 150��6B   #  �  6N   ! 5�   *   �� 5�   .   .R�5�   2   �� $
��host��6<   $
" Left=�6b   $��14(�6w   $��5q�6�   $# 3q�6�   ��6H   �� *
UserHost+6�   5�   /   5�   '   �� 5�   &   $��IPアドレス��65   $	 150��6E   #  �  6Q   ! 5�   *   �� 5�   .   .R�5�   2   �� $��ip��6<   $
" Left=�6^   $��14(�6s   $��5q�6�   $# 3q�6�   ��6F   �� *UserIP+6�   5�   /   5�   '   �� 5�   &   $��ログオン��65   $	 150��6C   #  �  6O   ! 5�   *   �� 5�   .   .R�5�   2   �� $��logon��6<   $
" Left=�6h   $��14(�6}   $��5q�6�   $# 3q�6�   ��6I   �� *UserLogInName+6�   5�   /   5�   '   5�   #   5�      5�      5�      5�      �� 5�      $��5q�6   $��1q�6    $ 4��64   
���   �$
��Left=�6Y   ._�5�      �� $	��120��6   $	��485��6,   $
��Auto=�68   .��5�      �� -   5        $��TextBox��5�      $	��485��5�      $��16(�5�      $��1,0,1,0q�5�      $ �� TextWrapping$
$ Wrap=�5�      % �� AcceptsReturn% . ��5�      �� *	Message+.��5�   /   5�      5�      5�      � 5�      $	��500��6   $��5q�6   .+�5�      �� $��送信��6   $	��100��6N   ��6&   �� *MessageSendCommand+6N   5�        5�      $��0,5,0,0q�6"   $��Center=�63   $��Center=�6P   $ Depth1=�6k   $	��500��6�   $��20��6�   ���	#FFC3C3C3.��5�      �  5�      �� 5�      $��1* �6+   �� 5�      $��3* �6+   .+�5�      �� $# 2,0,0,0q�6    $
��Wrap=�6M   $
��Left=�6a   $��Center=�6|   $
��Bold"�6�   $��12(�6�   ��62   �� *	LogStatus+6�   �� 5�      $��1��6    $# 0,0,5,0q�60   $
��Wrap=�6b   $��Right=�6v   $��Center=�6�   $
��Bold"�6�   $��12(�6�   ��6B   �� *RightLogStatus+6�   5�      5�      5�      5�      5�      