using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RiddleMazeRun.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            this.InitializeComponent();

            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.PreferredMinimumWidth = 900;
            appWindowPresenter.PreferredMinimumHeight = 700;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the main window
            Application.Current.Exit();
        }
    }
}
