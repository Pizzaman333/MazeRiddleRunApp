using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using RiddleMazeRun.Controls;
using RiddleMazeRun.Helpers;
using RiddleMazeRun.Pages;
using RiddleMazeRun.Services;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RiddleMazeRun;

public sealed partial class MainWindow : Window
{
    private MainMenuPage menu = new MainMenuPage();
    public MainWindow()
    {
        this.InitializeComponent();

        var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
        appWindowPresenter.PreferredMinimumWidth = 1000;
        appWindowPresenter.PreferredMinimumHeight = 600;

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Closing += AppWindow_Closing;

        //menu.NavigationRequested += page => NavigateWithExitEffect(page);
        menu.NavigationRequested += page =>
        {
            NavigateTo(page);
            UIHelper.RefreshAllButtons(MainGrid);
        };
        NavigateTo(menu);
    }

    public async void NavigateTo(Page page)
    {
        page.Opacity = 0;

        MainFrame.Content = page;

        await Task.Delay(10);

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        Storyboard.SetTarget(fadeIn, page);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");
        var sb = new Storyboard();
        sb.Children.Add(fadeIn);
        sb.Begin();

        if (page is not MainMenuPage)
        {
            PauseBtnFadeIn.Begin();
        }

        if (page is SettingsPage settingsPage)
        {
            settingsPage.ThemeChangeRequested += ThemeControl_ThemeChangeRequested;
        }

        UIHelper.RefreshAllButtons(MainGrid);
    }

    //private void ThemeControl_ThemeChangeRequested(object? sender, ThemeChangeRequestedEventArgs e)
    //{
    //    MainGrid.RequestedTheme = e.RequestedThemeIsLight ? ElementTheme.Light : ElementTheme.Dark;
    //    RefreshButtonState(PauseMenuBtn);
    //}

    private void ThemeControl_ThemeChangeRequested(object? sender, ThemeChangeRequestedEventArgs e)
    {
        var newTheme = e.RequestedThemeIsLight ? ElementTheme.Light : ElementTheme.Dark;

        AppSettings.Current.CurrentTheme = newTheme;

        MainGrid.RequestedTheme = newTheme;
        UIHelper.RefreshAllButtons(MainGrid);
        //if (MainFrame.Content is FrameworkElement currentPage)
        //{
        //    UIHelper.RefreshBackgrounds(currentPage, "BgLinearGradientBrush");
        //}
    }

    public async void NavigateToWithStyle(Page page)
    {
        // Start with transform: translate and rotate
        var transformGroup = new TransformGroup();
        var translate = new TranslateTransform { X = -300, Y = -300 }; // Fly in from top-left
        var rotate = new RotateTransform { Angle = -90 }; // Start rotated
        transformGroup.Children.Add(rotate);
        transformGroup.Children.Add(translate);
        page.RenderTransform = transformGroup;
        page.RenderTransformOrigin = new Point(0.5, 0.5); // Rotate around center

        // Load page
        MainFrame.Content = page;

        await Task.Delay(10); // Allow rendering before animating

        // Rotation animation
        var rotateAnim = new DoubleAnimation
        {
            From = -90,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(500),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(rotateAnim, page);
        Storyboard.SetTargetProperty(rotateAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)");

        // X position animation
        var xAnim = new DoubleAnimation
        {
            From = -300,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(500),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(xAnim, page);
        Storyboard.SetTargetProperty(xAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)");

        // Y position animation
        var yAnim = new DoubleAnimation
        {
            From = -300,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(500),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(yAnim, page);
        Storyboard.SetTargetProperty(yAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)");

        // Combine animations
        var sb = new Storyboard();
        sb.Children.Add(rotateAnim);
        sb.Children.Add(xAnim);
        sb.Children.Add(yAnim);
        sb.Begin();

        if (page is MainMenuPage) return;
        PauseBtnFadeIn.Begin();
    }

    public async void NavigateWithExitEffect(Page newPage)
    {
        if (MainFrame.Content is not Page currentPage) return;

        // Set up the transform on the current page
        var transformGroup = new TransformGroup();
        var rotate = new RotateTransform();
        var translate = new TranslateTransform();
        transformGroup.Children.Add(rotate);
        transformGroup.Children.Add(translate);
        currentPage.RenderTransform = transformGroup;
        currentPage.RenderTransformOrigin = new Point(0.5, 0.5);

        // Rotation out
        var rotateOut = new DoubleAnimation
        {
            From = 0,
            To = 720, // 2 full spins
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(rotateOut, currentPage);
        Storyboard.SetTargetProperty(rotateOut, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)");

        // Fly out to bottom-right
        var xOut = new DoubleAnimation
        {
            From = 0,
            To = 500,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(xOut, currentPage);
        Storyboard.SetTargetProperty(xOut, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)");

        var yOut = new DoubleAnimation
        {
            From = 0,
            To = 500,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(yOut, currentPage);
        Storyboard.SetTargetProperty(yOut, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)");

        // Combine and run
        var sb = new Storyboard();
        sb.Children.Add(rotateOut);
        sb.Children.Add(xOut);
        sb.Children.Add(yOut);
        sb.Begin();

        // Wait for the animation to finish
        await Task.Delay(400);

        // Now bring in the new page with your flying spin
        NavigateToWithStyle(newPage);
    }

    public async void NavigateWithVortexEffect(Page newPage)
    {
        if (MainFrame.Content is not Page currentPage) return;

        // Setup transform
        var transformGroup = new TransformGroup();
        var scale = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
        var translate = new TranslateTransform { X = 0, Y = 0 };
        transformGroup.Children.Add(scale);
        transformGroup.Children.Add(translate);
        currentPage.RenderTransform = transformGroup;
        currentPage.RenderTransformOrigin = new Point(1, 0); // Top-right corner

        // Scale down
        var scaleAnim = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(350),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(scaleAnim, currentPage);
        Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");

        var scaleAnimY = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(350),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(scaleAnimY, currentPage);
        Storyboard.SetTargetProperty(scaleAnimY, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");

        // Move slightly toward top-right
        var moveAnim = new DoubleAnimation
        {
            From = 0,
            To = -100,
            Duration = TimeSpan.FromMilliseconds(350),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(moveAnim, currentPage);
        Storyboard.SetTargetProperty(moveAnim, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)");

        var moveAnimY = new DoubleAnimation
        {
            From = 0,
            To = -100,
            Duration = TimeSpan.FromMilliseconds(350),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(moveAnimY, currentPage);
        Storyboard.SetTargetProperty(moveAnimY, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)");

        // Create and start storyboard
        var sb = new Storyboard();
        sb.Children.Add(scaleAnim);
        sb.Children.Add(scaleAnimY);
        sb.Children.Add(moveAnim);
        sb.Children.Add(moveAnimY);
        sb.Begin();

        await Task.Delay(350);

        // Now do the reverse on the new page
        PlayVortexIn(newPage);
    }

    public async void PlayVortexIn(Page page)
    {
        // Setup starting transform
        var transformGroup = new TransformGroup();
        var scale = new ScaleTransform { ScaleX = 0, ScaleY = 0 };
        var translate = new TranslateTransform { X = -100, Y = -100 };
        transformGroup.Children.Add(scale);
        transformGroup.Children.Add(translate);
        page.RenderTransform = transformGroup;
        page.RenderTransformOrigin = new Point(1, 0); // Same top-right corner

        MainFrame.Content = page;
        await Task.Delay(10);

        // Animate scale in
        var scaleIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleIn, page);
        Storyboard.SetTargetProperty(scaleIn, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");

        var scaleInY = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleInY, page);
        Storyboard.SetTargetProperty(scaleInY, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");

        // Animate back to original position
        var moveX = new DoubleAnimation
        {
            From = -100,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(moveX, page);
        Storyboard.SetTargetProperty(moveX, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)");

        var moveY = new DoubleAnimation
        {
            From = -100,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(400),
            EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(moveY, page);
        Storyboard.SetTargetProperty(moveY, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)");

        // Go!
        var sb = new Storyboard();
        sb.Children.Add(scaleIn);
        sb.Children.Add(scaleInY);
        sb.Children.Add(moveX);
        sb.Children.Add(moveY);
        sb.Begin();

        if (page is MainMenuPage) return;
        PauseBtnFadeIn.Begin();
    }

    private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (ConfirmExitControl.Opacity != 0) return;
        args.Cancel = true; // prevent from closing
        ConfirmExitControl.Show();
    }

    private async void PauseMenuBtn_Click(object sender, RoutedEventArgs e)
    {
        //NavigateWithVortexEffect(menu);
        //NavigateWithExitEffect(menu);
        NavigateTo(menu);
        PauseBtnFadeOut.Begin();
        UIHelper.RefreshAllButtons(MainGrid);
    }

    //private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
    //{
    //    if (ThemeToggle.IsOn)
    //    {
    //        // Set the theme to Dark for the root container
    //        DaddyGrid.RequestedTheme = ElementTheme.Light;
    //    }
    //    else
    //    {
    //        // Set the theme to Light for the root container
    //        DaddyGrid.RequestedTheme = ElementTheme.Dark;
    //    }
    //}
}
