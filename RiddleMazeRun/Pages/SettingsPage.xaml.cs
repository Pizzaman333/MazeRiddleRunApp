using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RiddleMazeRun.Controls;
using RiddleMazeRun.Services;
using System;
using System.Threading.Tasks;

namespace RiddleMazeRun.Pages;

public sealed partial class SettingsPage : Page
{
    /// <summary>
    /// Подія, яка виникає при запиті на зміну теми з компонента ThemeChangerControl.
    /// </summary>
    public event EventHandler<ThemeChangeRequestedEventArgs>? ThemeChangeRequested;

    /// <summary>
    /// Ініціалізує сторінку налаштувань та підписується на подію зміни теми.
    /// </summary>
    public SettingsPage()
    {
        this.InitializeComponent();
        ThemeChangerControl.ThemeChangeRequested += (s, e) =>
        {
            // Передаємо подію далі
            ThemeChangeRequested?.Invoke(this, e);
            VisualStateManager.GoToState(ApplyBtn, "Disabled", true);
        };
    }

    /// <summary>
    /// Обробник події завантаження сторінки.
    /// Встановлює стан перемикача теми відповідно до поточної теми програми
    /// та переводить кнопку застосування в стан "Disabled".S
    /// </summary>
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeChangerControl.SetToggleState(AppSettings.Current.CurrentTheme == ElementTheme.Light);
        await Task.Delay(100);
        VisualStateManager.GoToState(ApplyBtn, "Disabled", true);
    }

    /// <summary>
    /// Обробник вибору радіокнопки. На даний момент порожній.
    /// </summary>
    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
    }
}
