using Microsoft.UI.Xaml.Controls;
using System;

namespace RiddleMazeRun.Pages;
public sealed partial class MainGamePage : Page
{
    /// <summary>
    /// Подія, яка ініціюється при натисканні кнопки переходу до вибору рівнів.
    /// </summary>
    public event EventHandler<object?> NavigateToLevelSelectionRequested;

    /// <summary>
    /// Ініціалізує новий екземпляр класу MainGamePage
    /// </summary>
    public MainGamePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Обробляє натискання кнопки "Story", викликаючи подію "NavigateToLevelSelectionRequested
    /// </summary>
    private void StoryButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        NavigateToLevelSelectionRequested?.Invoke(this, null);
    }
}