using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RiddleMazeRun.MyWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AuthWindow : Window
{
    public AuthWindow()
    {
        this.InitializeComponent();

        var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
        appWindowPresenter.PreferredMinimumWidth = 500;
        appWindowPresenter.PreferredMinimumHeight = 800;
        //MoveAllDown.Begin();
        var brush = (LinearGradientBrush)Application.Current.Resources["BgLinearGradientBrush"];
        AuthGrid.Background = brush;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Closing += AppWindow_Closing;
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (ConfirmExitControl.Opacity != 0) return;
        args.Cancel = true; // prevent it from closing
        ConfirmExitControl.Show();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        ConfirmExitControl.Show();
    }

    private static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Simple but solid regex for basic email structure
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    private static string GetPasswordValidationErrors(string password)
    {
        //var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            //errors.Add("Password cannot be empty.");
            //return errors;
            return "Password cannot be empty.";
        }
        //errors.Add("Password must be 6+ characters long.");
        if (password.Length < 6) return "Password must be 6+ characters long.";

        //errors.Add("Password must contain a lowercase letter.");
        if (!Regex.IsMatch(password, @"[a-z]")) return "Password requires a lowercase letter.";

        //errors.Add("Password must contain an uppercase letter.");
        if (!Regex.IsMatch(password, @"[A-Z]")) return "Password requires an uppercase letter.";

        //errors.Add("Password must contain a digit.");
        if (!Regex.IsMatch(password, @"\d")) return "Password requires a digit.";

        //errors.Add("Password must contain a special character.");
        if (!Regex.IsMatch(password, @"[^A-Za-z\d]")) return "Password requires a special character.";

        return String.Empty;
    }

    private bool LogInInutsAreActive = false;
    private bool SignInInutsAreActive = false;
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (LogInInutsAreActive)
        {
            //string email = LogInEmailTextBox.Text;
            //string password = LogInPasswordTextBox.Text;
            //var errors = GetPasswordValidationErrors(password);

            //if (!IsEmailValid(email))
            //{
            //    LogInEmailError.Begin();
            //    return;
            //}
            //else if (errors != String.Empty)
            //{
            //    LogInEmailNoError.Begin();
            //    LogInPasswordError.Begin();
            //    LogInPasswordErrorText.Text = errors;
            //    //LogInPasswordErrorText.Visibility = Visibility.Visible;
            //    return;
            //}

            var mainWindow = new MainWindow();
            mainWindow.Activate();
            mainWindow.ExtendsContentIntoTitleBar = true;
            this.Close();
            return;
        }
        LogInBtn.TabIndex = 1;
        SignUpBtn.TabIndex = 4;
        PleaseAuthText.Foreground = new SolidColorBrush(Colors.Transparent);
        MoveAllUp.Begin();
        LogInActive.Begin();
        LogInForm.Visibility = Visibility.Visible;
        InputsFadeIn.Begin();

        var brush = AuthGrid.Background as LinearGradientBrush;
        MoveGradient(brush!, 0.65);

        LogInInutsAreActive = true;

        if (!SignInInutsAreActive) return;
        RegInputsFadeOut.Begin();
        await Task.Delay(250);
        SignUpForm.Visibility = Visibility.Collapsed;
        SignInInutsAreActive = false;
    }

    private async void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        if (SignInInutsAreActive)
        {
            var mainWindow = new MainWindow();
            mainWindow.Activate();
            this.Close();
            return;
        }
        LogInBtn.TabIndex = 9;
        SignUpBtn.TabIndex = 1;
        PleaseAuthText.Foreground = new SolidColorBrush(Colors.Transparent);
        MoveAllUp.Begin();
        SignInActive.Begin();
        SignUpForm.Visibility = Visibility.Visible;
        RegInputsFadeIn.Begin();

        LinearGradientBrush brush = AuthGrid.Background as LinearGradientBrush;
        MoveGradient(brush!, 0.8);

        SignInInutsAreActive = true;

        if (!LogInInutsAreActive) return;
        InputsFadeOut.Begin();
        await Task.Delay(250);
        LogInForm.Visibility = Visibility.Collapsed;
        LogInInutsAreActive = false;
    }

    private void MoveGradient(LinearGradientBrush brush, double offset)
    {
        if (brush != null && brush.GradientStops.Count >= 2)
        {
            var gradientStop = brush.GradientStops[1];

            // Create the animation
            var animation = new DoubleAnimation
            {
                From = gradientStop.Offset,
                To = offset,
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                // Required for animating non-UI properties
                EnableDependentAnimation = true
            };

            // Apply the animation
            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, gradientStop);
            Storyboard.SetTargetProperty(animation, "Offset");

            storyboard.Begin();
        }
    }

    private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        AuthGrid.RequestedTheme = ThemeToggle.IsOn ? ElementTheme.Light : ElementTheme.Dark;
        //App.Current.RequestedTheme = ApplicationTheme.Dark;
        // Refresh button visuals
        RefreshButtonState(LogInBtn);
        RefreshButtonState(SignUpBtn);
        RefreshButtonState(GuestBtn);
        RefreshButtonState(ExitBtn);
        var brush = (LinearGradientBrush)Application.Current.Resources["BgGradientBrush"];
        AuthGrid.Background = brush;
    }

    // Refresh btns in visualstatemanager manually
    private void RefreshButtonState(Control control)
    {
        if (control != null)
        {
            VisualStateManager.GoToState(control, "PointerOver", true);
            VisualStateManager.GoToState(control, "Normal", true);
        }
    }
}

public partial class VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string culture)
    {
        return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string culture)
    {
        throw new NotImplementedException();
    }
}