using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using RiddleMazeRun.Controls;
using RiddleMazeRun.Entities;
using RiddleMazeRun.Helpers;
using RiddleMazeRun.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace RiddleMazeRun.MyWindows;

public sealed partial class AuthWindow : Window
{
    /// <summary>
    /// Authorization window
    /// </summary>
    public AuthWindow()
    {
        this.InitializeComponent();

        var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
        appWindowPresenter.PreferredMinimumWidth = 1000;
        appWindowPresenter.PreferredMinimumHeight = 748;

        AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

        //MoveAllDown.Begin();
        var brush = (LinearGradientBrush)Application.Current.Resources["BgLinearGradientBrush"];
        AuthGrid.Background = brush;

        //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        //var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        //var appWindow = AppWindow.GetFromWindowId(windowId);
        //appWindow.Closing += AppWindow_Closing;

        ThemeChangerControl.ThemeChangeRequested += ThemeControl_ThemeChangeRequested;
        ThemeChangerControl.SetToggleState(AppSettings.Current.CurrentTheme == ElementTheme.Light);
    }

    /// <summary>
    /// Theme control
    /// </summary>
    /// <param name="sender">Obj</param>
    /// <param name="e">Theme</param>
    private void ThemeControl_ThemeChangeRequested(object? sender, ThemeChangeRequestedEventArgs e)
    {
        var newTheme = e.RequestedThemeIsLight ? ElementTheme.Light : ElementTheme.Dark;

        AppSettings.Current.CurrentTheme = newTheme; // This saves and notifies

        AuthGrid.RequestedTheme = newTheme;
        UIHelper.RefreshAllButtons(AuthGrid);

        var brush = (LinearGradientBrush)Application.Current.Resources["BgLinearGradientBrush"];
        AuthGrid.Background = brush;
    }

    /// <summary>
    /// Closing logic
    /// </summary>
    /// <param name="sender">obj</param>
    /// <param name="args">Closing event args</param>
    private void AppWindow_Closing(Windows.UI.WindowManagement.AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (ConfirmExitControl.Opacity != 0) return;
        args.Cancel = true; // prevent it from closing
        ConfirmExitControl.Show();
    }

    /// <summary>
    /// Closing logic
    /// </summary>
    /// <param name="sender">obj</param>
    /// <param name="e">Routed event args</param>
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

    private static string GetPasswordValidationError(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return "  Password cannot be empty.";
        }
        if (password.Length < 6) return "  Password must be 6+ characters long.";

        if (!Regex.IsMatch(password, @"[a-z]")) return "  Password requires a lowercase letter.";

        if (!Regex.IsMatch(password, @"[A-Z]")) return "  Password requires an uppercase letter.";

        if (!Regex.IsMatch(password, @"\d")) return "  Password requires a digit.";

        if (!Regex.IsMatch(password, @"[^A-Za-z\d]")) return "  Password requires a special character.";

        return String.Empty;
    }

    private bool LogInInutsAreActive = false;
    private bool SignInInutsAreActive = false;

    /// <summary>
    /// Login logic
    /// </summary>
    /// <param name="sender">Obj</param>
    /// <param name="e">RoutedEventArgs</param>
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (LogInInutsAreActive)
        {
            string email = LogInEmailTextBox.Text;
            string password = LogInPasswordTextBox.Text;
            string hashedPassword = HashPassword(password);

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RiddleMazeRun",
                "users.json"
            );

            try
            {
                if (!File.Exists(filePath))
                {
                    LogInEmailErrorTranslate.Begin();
                    return;
                }

                string json = await File.ReadAllTextAsync(filePath);
                var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

                var user = users.FirstOrDefault(u => u.Email == email);

                if (user == null || user.HashedPassword != hashedPassword)
                {
                    LogInEmailErrorTranslate.Begin();
                    return;
                }

                LogInEmailNoErrorTranslate.Begin();
                SessionManager.CurrentUser = user;

                var mainWindow = new MainWindow();
                if (mainWindow.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = AppSettings.Current.CurrentTheme;
                }
                mainWindow.Activate();
                mainWindow.ExtendsContentIntoTitleBar = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login error: " + ex.Message);
                LogInEmailErrorTranslate.Begin();
            }
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
            string name = RegisterUsernameTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            string password = RegisterPasswordTextBox.Text;
            string repeatPassword = RegisterRepeatPasswordTextBox.Text;

            if (AreInputsValid(name, email, password, repeatPassword))
            {
                string hashedPassword = HashPassword(password);

                User user = new User
                {
                    Username = name,
                    Email = email,
                    HashedPassword = hashedPassword,
                    CompletedLevels = 0,
                    CustomLevels = { },
                };
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RiddleMazeRun", "users.json");
                DataServices dataService = new DataServices();
                await dataService.SaveUserToFileAsync(user, filePath);
                var dataPackage = new DataPackage();
                dataPackage.SetText(filePath);
                Clipboard.SetContent(dataPackage);
                SessionManager.CurrentUser = user;


                var mainWindow = new MainWindow();
                if (mainWindow.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = AppSettings.Current.CurrentTheme;
                }
                mainWindow.Activate();
                mainWindow.ExtendsContentIntoTitleBar = true;
                this.Close();
            }
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

    private bool AreInputsValid(string username, string email, string password, string repeatPassword)
    {
        bool usernameIsValid = false;
        bool emailIsValid = false;
        bool passwordIsValid = false;
        bool passwordRepeatIsValid = false;

        var dataService = new RiddleMazeRun.Services.DataServices();
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RiddleMazeRun", "users.json");

        if (username.Length < 3)
        {
            RegisterUsernameErrorText.Visibility = Visibility.Visible;
            RegisterUsernameErrorTranslate.Begin();
        }
        else
        {
            RegisterUsernameNoErrorTranslate.Begin();
            usernameIsValid = true;
        }

        if (!IsEmailValid(email))
        {
            RegisterEmailErrorTextBlock.Text = "  Invalid email";
            RegisterEmailErrorText.Visibility = Visibility.Visible;
            RegisterEmailErrorTranslate.Begin();
        }
        else if (dataService.IsEmailInUse(email, filePath))
        {
            RegisterEmailErrorTextBlock.Text = "Email is already in use";
            RegisterEmailErrorText.Visibility = Visibility.Visible;
            RegisterEmailErrorTranslate.Begin();
        }
        else
        {
            RegisterEmailNoErrorTranslate.Begin();
            RegisterEmailErrorTextBlock.Text = "";
            emailIsValid = true;
        }

        string error = GetPasswordValidationError(password);
        if (error != String.Empty)
        {
            RegisterPasswordErrorText.Visibility = Visibility.Visible;
            RegisterPasswordErrorTextBlock.Text = error;
            RegisterPasswordErrorTranslate.Begin();
        }
        else
        {
            RegisterPasswordNoErrorTranslate.Begin();
            RegisterPasswordErrorTextBlock.Text = "";
            passwordIsValid = true;
        }

        if (repeatPassword != password)
        {
            RegisterRepeatPasswordErrorText.Visibility = Visibility.Visible;
            RegisterRepeatPasswordErrorTranslate.Begin();
        }
        else
        {
            RegisterRepeatPasswordNoErrorTranslate.Begin();
            passwordRepeatIsValid = true;
        }

        return usernameIsValid && emailIsValid && passwordIsValid && passwordRepeatIsValid;
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
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

    private void GuestBtn_Click(object sender, RoutedEventArgs e)
    {
        SessionManager.CurrentUser = new User
        {
            Username = "Guest",
            Email = "Guest",
            HashedPassword = "Guest",
            CompletedLevels = 0,
            CustomLevels = { },
        };
        var mainWindow = new MainWindow();
        if (mainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = AppSettings.Current.CurrentTheme;
        }
        mainWindow.Activate();
        mainWindow.ExtendsContentIntoTitleBar = true;
        this.Close();
    }

    private void AuthGrid_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            AppWindow.SetPresenter(AppWindowPresenterKind.Default);
        }
        else if (e.Key == Windows.System.VirtualKey.F11)
        {
            AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
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