using Microsoft.UI.Xaml;

namespace RiddleMazeRun;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    //private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
    //{
    //    if (ThemeToggle.IsOn)
    //    {
    //        // Set the theme to Dark for the root container
    //        DaddyGrid.RequestedTheme = ElementTheme.Light;
    //    }
    //    else
    //    {
    //        // Set the theme to Light for the root container
    //        DaddyGrid.RequestedTheme = ElementTheme.Dark;
    //    }
    //}

    //private void ExitButton_Click(object sender, RoutedEventArgs e)
    //{
    //    // Close the main window
    //    Application.Current.Exit();
    //}
}
