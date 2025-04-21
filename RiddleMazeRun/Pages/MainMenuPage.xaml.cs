using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace RiddleMazeRun.Pages;

public sealed partial class MainMenuPage : Page
{
    public MainMenuPage()
    {
        this.InitializeComponent();



        var brush = (RadialGradientBrush)Application.Current.Resources["BgCircularGradientBrush"];
        MainGrid.Background = brush;
    }

    public event Action<Page> NavigationRequested;

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        ConfirmExitControl.Show();
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new MainGamePage());
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new AboutPage());
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new SettingsPage());
    }

    private void HowToPlayButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new HowToPlayPage());
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new HelpPage());
    }
}
