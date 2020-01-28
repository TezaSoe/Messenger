using System;
using System.ComponentModel;
using System.Windows;
using ContextMenu = System.Windows.Controls.ContextMenu;
using System.Windows.Forms;

namespace Messenger.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifier = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();

            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Main.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            ni.MouseDown +=
                delegate (object sender, MouseEventArgs e)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
                        menu.IsOpen = true;
                    }
                };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // setting cancel to true will cancel the close request
            // so the application is not closed
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        private void Menu_Open(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
