using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace RiddleMazeRun.CustomElements;

class ButtonWithHover : Button
{
    public ButtonWithHover()
    {
        Loaded += ButtonWithHover_SetCursor;
    }

    private void ButtonWithHover_SetCursor(object sender, RoutedEventArgs e)
    {
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }
}