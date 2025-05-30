using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace RiddleMazeRun.Pages;
public sealed partial class HelpPage : Page
{
    /// <summary>
    /// Подія, яка викликається при натисканні кнопки "Як грати", щоб здійснити навігацію до відповідної сторінки.
    /// </summary>
    public event EventHandler<object?> NavigateToHowToPlayRequested;

    /// <summary>
    /// Ініціалізує компоненти сторінки допомоги (HelpPage).
    /// </summary>
    public HelpPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Обробник натискання кнопки для переходу на офіційний сайт гри.
    /// Відкриває в браузері сторінку за вказаним URL.
    /// </summary>
    /// <param name="sender">Джерело події — кнопка</param>
    /// <param name="e">Аргументи події натискання</param>
    private async void VisitWebsiteButton_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://pizzaman333.github.io/MazeRiddleRun/");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }

    /// <summary>
    /// Обробник натискання кнопки "Як грати".
    /// Викликає подію NavigateToHowToPlayRequested для переходу на відповідну сторінку.
    /// </summary>
    /// <param name="sender">Джерело події — кнопка</param>
    /// <param name="e">Аргументи події натискання</param>
    private void GpToHowToPlayButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateToHowToPlayRequested?.Invoke(this, null);
    }
}
