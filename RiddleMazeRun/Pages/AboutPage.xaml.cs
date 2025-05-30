using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace RiddleMazeRun.Pages;

public sealed partial class AboutPage : Page
{
    /// <summary>
    /// Сторінка про гру
    /// </summary>
    public AboutPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Обробник натискання кнопки для переходу на офіційний сайт гри.
    /// Відкриває в браузері сторінку за заданим URL-адресом.
    /// </summary>
    /// <param name="sender">Джерело події — кнопка</param>
    /// <param name="e">Аргументи події натискання</param>
    private async void VisitWebsiteButton_Click(object sender, RoutedEventArgs e)
    {
        var uri = new Uri("https://pizzaman333.github.io/MazeRiddleRun/");
        await Windows.System.Launcher.LaunchUriAsync(uri);
    }
}
