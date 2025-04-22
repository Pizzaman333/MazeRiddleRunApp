using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace RiddleMazeRun.Controls;

public sealed partial class ThemeChangerControl : UserControl
{
    public event EventHandler<ThemeChangeRequestedEventArgs> ThemeChangeRequested;

    public ThemeChangerControl()
    {
        this.InitializeComponent();
    }

    private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        bool requestedThemeIsLight = ThemeToggle.IsOn;
        ThemeChangeRequested?.Invoke(this, new ThemeChangeRequestedEventArgs(requestedThemeIsLight));
    }

    public void SetToggleState(bool isLight)
    {
        ThemeToggle.IsOn = isLight;
    }
}

public class ThemeChangeRequestedEventArgs : RoutedEventArgs
{
    public bool RequestedThemeIsLight { get; }

    public ThemeChangeRequestedEventArgs(bool requestedThemeIsLight)
    {
        RequestedThemeIsLight = requestedThemeIsLight;
    }
}

