using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using RiddleMazeRun.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;

namespace RiddleMazeRun.Pages;

public sealed partial class StoryLevelsPage : Page
{
    public event EventHandler<object?> NavigateToNextLevelRequested;
    public event EventHandler<object?> NavigateToMainMenuRequested;
    private int lvl;
    private int[,] map;
    private int tileSize = 100;
    private CanvasControl player;
    private int playerX, playerY;
    private TranslateTransform cameraTransform = new TranslateTransform();
    private int score = 0;

    // Animation fields
    private CanvasBitmap _spritesheet;
    private DispatcherTimer _animationTimer;
    private int _currentFrame = 0;
    private readonly int _frameWidth = 72;
    private readonly int _frameHeight = 72;
    private readonly int _framesPerRow = 4;
    private readonly int _totalFramesPerDirection = 4;
    private int _currentDirection = 0; // 0: down, 1: left, 2: right, 3: up
    private bool _isMoving = false;

    public StoryLevelsPage(int level)
    {
        this.InitializeComponent();
        lvl = level;
        Loaded += StoryLevelsPage_Loaded;
        KeyUp += StoryLevelsPage_KeyUp;
        // Initialize animation timer
        _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _animationTimer.Tick += AnimationTimer_Tick;
    }

    private async void StoryLevelsPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GameCanvas.RenderTransform = cameraTransform;
        LoadAndDrawMapAsync();
        this.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        Debug.WriteLine($"Level: {SessionManager.CurrentUser.CompletedLevels}");

        string name;
        switch (lvl)
        {
            case 0:
                name = "The Awakanning";
                break;
            case 1:
                name = "Whispers of the Spiral";
                break;
            case 2:
                name = "Serpent’s Dance";
                break;
            case 3:
                name = "Echoes of the Void";
                break;
            case 4:
                name = "Labyrinth of Eternal Shadows";
                break;
            case 5:
                name = "Tomb of Forgotten Kings";
                break;
            default:
                name = "";
                break;
        }
        LevelInfoText.Text = $"Level {lvl + 1} The {name}";
        var fadeIn = new DoubleAnimation
        {
            From = 0.9,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.3)),
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(fadeIn, LevelInfoOverlay);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeIn);
        storyboard.Begin();
        await Task.Delay(300);

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(3.5)),
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(fadeOut, LevelInfoOverlay);
        Storyboard.SetTargetProperty(fadeOut, "Opacity");

        var storyboard2 = new Storyboard();
        storyboard2.Children.Add(fadeOut);
        storyboard2.Begin();
        await Task.Delay(3500);
        LevelInfoOverlay.Visibility = Visibility.Collapsed;
    }

    private async void PlayerSprite_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        try
        {
            _spritesheet = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Skins/spritesheet.png"));
            Debug.WriteLine($"Spritesheet loaded: Width={_spritesheet.Bounds.Width}, Height={_spritesheet.Bounds.Height}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load spritesheet: {ex.Message}");
            _spritesheet = null; // Set to null to handle gracefully in Draw
        }
    }

    private void PlayerSprite_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (_spritesheet == null)
        {
            Debug.WriteLine("Spritesheet is null, cannot draw.");
            return; // Prevent drawing if spritesheet failed to load
        }

        // Ensure frame and direction are within bounds
        int col = _isMoving ? Math.Clamp(_currentFrame, 0, _totalFramesPerDirection - 1) : 0;
        int row = Math.Clamp(_currentDirection, 0, 3); // Max 3 for 4 rows (down, left, right, up)

        // Calculate source rectangle
        double x = col * _frameWidth;
        double y = row * _frameHeight;

        // Ensure sourceRect stays within spritesheet bounds
        double maxX = _spritesheet.Bounds.Width - _frameWidth;
        double maxY = _spritesheet.Bounds.Height - _frameHeight;
        x = Math.Clamp(x, 0, maxX);
        y = Math.Clamp(y, 0, maxY);

        var sourceRect = new Rect(x, y, _frameWidth, _frameHeight);

        // Draw the frame
        args.DrawingSession.DrawImage(_spritesheet, new Vector2(0, 0), sourceRect);
    }

    private void AnimationTimer_Tick(object sender, object e)
    {
        if (_isMoving)
        {
            _currentFrame = (_currentFrame + 1) % _totalFramesPerDirection;
            PlayerSprite.Invalidate(); // Redraw the sprite
        }
    }

    private async void LoadAndDrawMapAsync()
    {
        try
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Maps/map-lvl{lvl}.txt"));
            string[] lines = (await FileIO.ReadTextAsync(file)).Split('\n').Select(l => l.Trim()).ToArray();

            map = new int[lines.Length, lines[0].Split(' ').Length];

            for (int y = 0; y < lines.Length; y++)
            {
                var numbers = lines[y].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                for (int x = 0; x < numbers.Length; x++)
                {
                    map[y, x] = numbers[x];
                }
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 3)
                    {
                        playerX = x;
                        playerY = y;
                        break;
                    }
                }
                if (playerX != 0) break;
            }

            DrawMap();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading map: {ex.Message}");
        }
    }

    private void DrawMap()
    {
        GameCanvas.Children.Clear();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                int mask = GetPathTileMask(map, y, x);

                // Always draw base tile (path or wall or spawn or finish)
                Image baseTileImage = new Image
                {
                    Width = tileSize,
                    Height = tileSize,
                    Stretch = Stretch.UniformToFill
                };

                string baseImagePath;
                switch (map[y, x])
                {
                    case 0:
                        baseImagePath = $"ms-appx:///Assets/Parts/tile_{mask}.png";
                        break;
                    case 1:
                        baseImagePath = "ms-appx:///Assets/Parts/wall.jpg";
                        break;
                    case 2:
                        baseImagePath = $"ms-appx:///Assets/Parts/tile_{mask}.png";
                        break;
                    case 3:
                        baseImagePath = $"ms-appx:///Assets/Parts/tile_{mask}.png";
                        break;
                    case 4:
                        baseImagePath = $"ms-appx:///Assets/Parts/finish.png";
                        break;
                    case 5:
                        baseImagePath = $"ms-appx:///Assets/Parts/tile_{mask}.png";
                        break;
                    default:
                        baseImagePath = "ms-appx:///Assets/Parts/unknown.png";
                        break;
                }

                baseTileImage.Source = new BitmapImage(new Uri(baseImagePath));
                Canvas.SetLeft(baseTileImage, x * tileSize);
                Canvas.SetTop(baseTileImage, y * tileSize);
                GameCanvas.Children.Add(baseTileImage);

                if (map[y, x] == 2)
                {
                    Image collectibleImage = new Image
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Stretch = Stretch.UniformToFill,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/Parts/collectible.png"))
                    };
                    Canvas.SetLeft(collectibleImage, x * tileSize);
                    Canvas.SetTop(collectibleImage, y * tileSize);
                    Canvas.SetZIndex(collectibleImage, 1);
                    GameCanvas.Children.Add(collectibleImage);
                }
                else if (map[y, x] == 3)
                {
                    Image collectibleImage = new Image
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Stretch = Stretch.UniformToFill,
                        Source = new BitmapImage(new Uri("ms-appx:///Assets/Parts/player_spawn.png"))
                    };
                    Canvas.SetLeft(collectibleImage, x * tileSize);
                    Canvas.SetTop(collectibleImage, y * tileSize);
                    Canvas.SetZIndex(collectibleImage, 1);
                    GameCanvas.Children.Add(collectibleImage);
                }
                else if (map[y, x] == 5)
                {
                    Random random = new Random();
                    int num = random.Next(1, 6);
                    Image collectibleImage = new Image
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Stretch = Stretch.UniformToFill,
                        Source = new BitmapImage(new Uri($"ms-appx:///Assets/Parts/Objects/object{num}.png"))
                    };
                    Canvas.SetLeft(collectibleImage, x * tileSize);
                    Canvas.SetTop(collectibleImage, y * tileSize);
                    Canvas.SetZIndex(collectibleImage, 1);
                    GameCanvas.Children.Add(collectibleImage);
                }
            }
        }


        // Add player (CanvasControl already in XAML)
        player = PlayerSprite; // Reference the CanvasControl
        GameCanvas.Children.Add(player);
        UpdatePlayerPosition();
        Canvas.SetZIndex(player, 1000);
        _animationTimer.Start(); // Start animation timer
    }

    bool IsWalkable(int[,] map, int y, int x)
    {
        if (y >= 0 && y < map.GetLength(0) && x >= 0 && x < map.GetLength(1))
            return map[y, x] != 1;
        return false;
    }

    int GetPathTileMask(int[,] map, int y, int x)
    {
        int mask = 0;
        if (y > 0 && IsWalkable(map, y - 1, x)) mask |= 1; // Up
        if (x < map.GetLength(1) - 1 && IsWalkable(map, y, x + 1)) mask |= 2; // Right
        if (y < map.GetLength(0) - 1 && IsWalkable(map, y + 1, x)) mask |= 4; // Down
        if (x > 0 && IsWalkable(map, y, x - 1)) mask |= 8; // Left
        return mask;
    }

    private void UpdatePlayerPosition()
    {
        Canvas.SetLeft(player, playerX * tileSize + (tileSize - _frameWidth) / 2);
        Canvas.SetTop(player, playerY * tileSize + (tileSize - _frameHeight) / 2);
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        double viewWidth = this.ActualWidth;
        double viewHeight = this.ActualHeight;

        double centerX = playerX * tileSize + tileSize / 2;
        double centerY = playerY * tileSize + tileSize / 2;

        double offsetX = centerX - viewWidth / 2;
        double offsetY = centerY - viewHeight / 2;

        offsetX = Math.Max(0, offsetX);
        offsetY = Math.Max(0, offsetY);

        double mapWidth = map.GetLength(1) * tileSize;
        double mapHeight = map.GetLength(0) * tileSize;

        offsetX = Math.Min(offsetX, mapWidth - viewWidth);
        offsetY = Math.Min(offsetY, mapHeight - viewHeight);

        cameraTransform.X = -offsetX;
        cameraTransform.Y = -offsetY;
    }

    private async void StoryLevelsPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {

        int newX = playerX;
        int newY = playerY;
        int newDirection = _currentDirection;
        bool moved = false;

        switch (e.Key)
        {
            case VirtualKey.Up:
                newY--;
                newDirection = 3; // Up
                moved = true;
                break;
            case VirtualKey.Down:
                newY++;
                newDirection = 0; // Down
                moved = true;
                break;
            case VirtualKey.Left:
                newX--;
                newDirection = 1; // Left
                moved = true;
                break;
            case VirtualKey.Right:
                newX++;
                newDirection = 2; // Right
                moved = true;
                break;
            case VirtualKey.Escape:
                newY++; // Note: Consider removing or handling Escape differently
                newDirection = 0; // Down
                moved = true;
                break;
            default:
                return;
        }
        PlayerSprite.Invalidate();

        if (newX >= 0 && newX < map.GetLength(1) && newY >= 0 && newY < map.GetLength(0) && IsWalkable(map, newY, newX))
        {
            playerX = newX;
            playerY = newY;
            _currentDirection = newDirection;
            _isMoving = true;
            UpdatePlayerPosition();
            // Check for collectibles
            if (map[newY, newX] == 2)
            {
                score++;
                ScoreText.Text = $"Score: {score}";
                map[newY, newX] = 0; // Remove collectible
                DrawMap(); // Redraw map to update collectible
            }
            else if (map[newY, newX] == 4)
            {
                if (lvl == 4)
                {
                    ShowCredits();
                }
                else
                {
                    await ShowLevelPassedOverlayAsync();
                }
            }
        }
        else
        {
            _isMoving = false; // Stop animation if movement is blocked
            _currentFrame = 3; // Reset to idle frame
            PlayerSprite.Invalidate();
        }

        if (moved)
        {
            UpdateCamera();
        }

        e.Handled = false;
    }

    private void StoryLevelsPage_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        // Якщо користувач відпустив клавішу стрілки, припиняємо анімацію
        if (e.Key == VirtualKey.Up || e.Key == VirtualKey.Down || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right)
        {
            _isMoving = false;
        }
    }

    private async Task ShowLevelPassedOverlayAsync()
    {
        LevelPassedOverlay.Visibility = Visibility.Visible;

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(1)),
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(fadeIn, LevelPassedOverlay);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        var storyboard = new Storyboard();
        storyboard.Children.Add(fadeIn);
        storyboard.Begin();

        await Task.Delay(2000);

        if (SessionManager.CurrentUser.CompletedLevels == lvl)
        {
            SessionManager.CurrentUser.CompletedLevels += 1;
        }
        NavigateToNextLevelRequested?.Invoke(this, lvl + 1);
    }

    private void ButtonWithHover_Click(object sender, RoutedEventArgs e)
    {
        NavigateToMainMenuRequested?.Invoke(this, null);
    }

    private async void ShowCredits()
    {
        CreditsOverlay.Visibility = Visibility.Visible;

        // Fade in
        var fadeIn = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
        {
            To = 1,
            Duration = TimeSpan.FromSeconds(1)
        };
        var fadeStoryboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        fadeStoryboard.Children.Add(fadeIn);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fadeIn, CreditsOverlay);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fadeIn, "Opacity");
        fadeStoryboard.Begin();

        // Start scrolling credits
        var scrollTransform = new TranslateTransform();
        CreditsPanel.RenderTransform = scrollTransform;

        double startY = this.ActualHeight;
        double endY = -CreditsPanel.ActualHeight;

        // Wait for layout to ensure ActualHeight is valid
        await Task.Delay(100);

        var animation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
        {
            From = startY,
            To = endY,
            Duration = TimeSpan.FromSeconds(15),
            EnableDependentAnimation = true
        };

        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        storyboard.Children.Add(animation);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(animation, scrollTransform);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(animation, "Y");
        storyboard.Begin();
    }
}