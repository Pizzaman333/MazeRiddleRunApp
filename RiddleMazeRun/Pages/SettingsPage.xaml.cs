using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RiddleMazeRun.Controls;
using RiddleMazeRun.Services;
using System;

namespace RiddleMazeRun.Pages;

public sealed partial class SettingsPage : Page
{
    public event EventHandler<ThemeChangeRequestedEventArgs>? ThemeChangeRequested;

    public SettingsPage()
    {
        this.InitializeComponent();
        ThemeChangerControl.ThemeChangeRequested += (s, e) =>
        {
            // Forward the event
            ThemeChangeRequested?.Invoke(this, e);
            //var brush = (RadialGradientBrush)Application.Current.Resources["BgCircularGradientBrush"];
            //SettingsGrid.Background = brush;



        };
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //ThemeToggle.IsOn = AppSettings.Current.CurrentTheme == ElementTheme.Light;
        ThemeChangerControl.SetToggleState(AppSettings.Current.CurrentTheme == ElementTheme.Light);

    }
}
