﻿using Microsoft.UI.Xaml;
using RiddleMazeRun.MyWindows;
using RiddleMazeRun.Services;

namespace RiddleMazeRun;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var authWindow = new AuthWindow();
        if (authWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = AppSettings.Current.CurrentTheme;
        }
        authWindow.Activate();
        authWindow.ExtendsContentIntoTitleBar = true;

        //var mainWindow = new MainWindow();
        //if (mainWindow.Content is FrameworkElement rootElement)
        //{
        //    rootElement.RequestedTheme = AppSettings.Current.CurrentTheme;
        //}
        //mainWindow.Activate();
        //mainWindow.ExtendsContentIntoTitleBar = true;
    }

    private Window? m_window;
}
