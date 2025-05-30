using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using RiddleMazeRun.Helpers;
using RiddleMazeRun.Services;
using System;

namespace RiddleMazeRun.Pages;

public sealed partial class MainMenuPage : Page
{
    /// <summary>
    /// Представляє головне меню гри.
    /// Ініціалізує компоненти сторінки.
    /// </summary>
    public MainMenuPage()
    {
        this.InitializeComponent();
        //var brush = (RadialGradientBrush)Application.Current.Resources["BgCircularGradientBrush"];
        //MainGrid.Background = brush;
    }

    /// <summary>
    /// Подія, яка використовується для навігації до інших сторінок.
    /// </summary>
    public event Action<Page> NavigationRequested;

    /// <summary>
    /// Обробник натискання кнопки "Вихід".
    /// Відображає контроль підтвердження виходу.
    /// </summary>
    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        ConfirmExitControl.Show();
    }

    /// <summary>
    /// Обробник натискання кнопки "Грати".
    /// Ініціює навігацію до головної ігрової сторінки.
    /// </summary>
    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new MainGamePage());
    }

    /// <summary>
    /// Обробник натискання кнопки "Про гру".
    /// Ініціює навігацію до сторінки з описом гри.
    /// </summary>
    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new AboutPage());
    }

    /// <summary>
    /// Обробник натискання кнопки "Налаштування".
    /// Ініціює навігацію до сторінки налаштувань.
    /// </summary>
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new SettingsPage());
    }

    /// <summary>
    /// Обробник натискання кнопки "Як грати".
    /// Ініціює навігацію до сторінки з інструкцією.
    /// </summary>
    private void HowToPlayButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new HowToPlayPage());
    }

    /// <summary>
    /// Обробник натискання кнопки "Допомога".
    /// Ініціює навігацію до сторінки допомоги.
    /// </summary>
    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationRequested?.Invoke(new HelpPage());
    }

    /// <summary>
    /// Обробник події завантаження сторінки.
    /// Встановлює відповідний фон залежно від теми та оновлює кнопки.
    /// </summary>
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppSettings.Current.CurrentTheme == ElementTheme.Light)
        {
            MainGrid.Background = (Brush)Application.Current.Resources["LightBgCircularGradientBrush"];
        }
        else
        {
            MainGrid.Background = (Brush)Application.Current.Resources["DarkBgCircularGradientBrush"];
        }
        UIHelper.RefreshAllButtons(MainGrid);
    }

    /// <summary>
    /// Обробник події завантаження кнопки з ефектом наведення.
    /// Встановлює градієнтний фон для основної сітки.
    /// </summary>
    private void ButtonWithHover_Loaded(object sender, RoutedEventArgs e)
    {
        var brush = (RadialGradientBrush)Application.Current.Resources["BgCircularGradientBrush"];
        MainGrid.Background = brush;
    }
}
