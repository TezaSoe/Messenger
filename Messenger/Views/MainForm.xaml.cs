using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Messenger.Views
{
    /// <summary>
    /// MainForm.xaml の相互作用ロジック
    /// </summary>
    public partial class MainForm : UserControl
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            DimBackgroundWhileLoadingRectangle.Visibility = Visibility.Visible;
            DeterminateCircularProgress.Visibility = Visibility.Visible;
        }
    }
}
