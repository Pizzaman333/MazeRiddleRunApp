using Microsoft.UI.Xaml.Controls;
using RiddleMazeRun.Services;
using System;
using System.Diagnostics;

namespace RiddleMazeRun.Pages;

public sealed partial class LevelSelectionPage : Page
{
    public event EventHandler<object?> NavigateToLevelRequested;
    /// <summary>
    /// Ініціалізує новий екземпляр класу LevelSelectionPage
    /// </summary>
    public LevelSelectionPage()
    {
        InitializeComponent();
        Loaded += LevelSelectionPage_Loaded;
    }

    private void LevelSelectionPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Debug.WriteLine($"Username: {SessionManager.CurrentUser.Username}\nLevel: {SessionManager.CurrentUser.CompletedLevels}");
        int currentLvl = SessionManager.CurrentUser.CompletedLevels;

        Btn1.IsEnabled = currentLvl >= 0;
        Btn2.IsEnabled = currentLvl >= 1;
        Btn3.IsEnabled = currentLvl >= 2;
        Btn4.IsEnabled = currentLvl >= 3;
        Btn5.IsEnabled = currentLvl >= 4;

        CommingSoonBtn.IsEnabled = false;
    }

    private void Btn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int levelNumber))
        {
            NavigateToLevelRequested?.Invoke(this, levelNumber);
        }
    }
}
