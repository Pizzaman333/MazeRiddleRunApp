using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media.Animation;
using System.Numerics;
using System.Threading.Tasks;

namespace RiddleMazeRun.Controls;

public sealed partial class ConfirmExitControl : UserControl
{
    public ConfirmExitControl()
    {
        this.InitializeComponent();
        ShadowHost.Loaded += (s, e) => AddDropShadow();
        this.Opacity = 0;
        this.Visibility = Visibility.Collapsed;
    }

    public void Show()
    {
        this.Visibility = Visibility.Visible;

        if (Resources["FadeInStoryboard"] is Storyboard fadeIn)
        {
            fadeIn.Begin();
        }
    }

    public async Task HideAsync()
    {
        if (Resources["FadeOutStoryboard"] is Storyboard fadeOut)
        {
            fadeOut.Begin();
            await Task.Delay(250);
            this.Visibility = Visibility.Collapsed;
        }
    }


    private void AddDropShadow()
    {
        var compositor = ElementCompositionPreview.GetElementVisual(ShadowHost).Compositor;

        var shadow = compositor.CreateDropShadow();
        shadow.Color = Colors.Black;
        shadow.BlurRadius = 30f;
        shadow.Offset = new Vector3(0, 0, 0);
        shadow.Opacity = 0.4f;

        var shadowVisual = compositor.CreateSpriteVisual();
        shadowVisual.Shadow = shadow;
        shadowVisual.Size = new Vector2(
            475,
            275);

        ElementCompositionPreview.SetElementChildVisual(ShadowHost, shadowVisual);
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Exit();
    }

    private void StayButton_Click(object sender, RoutedEventArgs e)
    {
        this.HideAsync();
    }
}
