using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace RiddleMazeRun;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
        appWindowPresenter.PreferredMinimumWidth = 1000;
        appWindowPresenter.PreferredMinimumHeight = 600;

        var brush = (RadialGradientBrush)Application.Current.Resources["BgCircularGradientBrush"];
        MainGrid.Background = brush;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Closing += AppWindow_Closing;
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (ConfirmExitControl.Opacity != 0) return;
        args.Cancel = true; // prevent it from closing
        ConfirmExitControl.Show();
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

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        ConfirmExitControl.Show();
    }
}
