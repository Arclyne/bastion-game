using System;
using System.Globalization;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Rendering;
using Bastion.Client.Screens;

namespace Bastion.Client;

public sealed class BastionGame : Game
{
    private const string ContentDirectory = "Content";
    private const string RegularFontName = "Regular";
    private const string BoldFontName = "Bold";
    private const string TitleFontName = "Title";

    private static readonly ILog _logger = LogManager.GetLogger(typeof(BastionGame));

    private readonly ClientServices _services;
    private readonly ScreenId _startScreen;
    private readonly GraphicsDeviceManager _graphics;
    private readonly InputState _input = new InputState();
    private readonly Navigator _navigator;

    private SpriteBatch? _batch;
    private ShapeRenderer? _shapes;
    private Canvas? _canvas;
    private BoardRenderer? _board;

    public BastionGame(ClientServices services, ScreenId startScreen)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
        _startScreen = startScreen;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Theme.WindowWidth,
            PreferredBackBufferHeight = Theme.WindowHeight,
            SynchronizeWithVerticalRetrace = true,
        };

        // Applied before the first catalog lookup, or the interface would start in the operating system language,
        // which may be neither of the two supported ones.
        Language.Apply(Language.Default);

        _navigator = new Navigator(services);
        Content.RootDirectory = ContentDirectory;
        IsMouseVisible = true;
        Window.Title = TextCatalog.WindowTitle;
        Window.AllowUserResizing = false;
    }

    protected override void Initialize()
    {
        Window.TextInput += OnTextInput;
        _logger.Info($"Client started. Culture={CultureInfo.CurrentUICulture.Name}");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _batch = new SpriteBatch(_graphics.GraphicsDevice);
        _shapes = new ShapeRenderer(_graphics.GraphicsDevice, _batch);
        _board = CreateBoardRenderer();
        _services.AttachBoard(_board);
        _canvas = new Canvas
        {
            Shapes = _shapes,
            Text = new TextRenderer(_batch),
            Fonts = new FontSet
            {
                Regular = Content.Load<SpriteFont>(RegularFontName),
                Bold = Content.Load<SpriteFont>(BoldFontName),
                Title = Content.Load<SpriteFont>(TitleFontName),
            },
        };
        _navigator.Start(_startScreen);
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update(gameTime);
        _navigator.Update(_input);
        _input.ClearText();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear wipes colour and depth: without it the depth buffer of the
        // previous frame carves pieces out of the board.
        _graphics.GraphicsDevice.Clear(Theme.Background);

        if (_board is not null && _navigator.Current is IWorldScreen worldScreen)
        {
            worldScreen.DrawWorld(_board);
        }

        if (_batch is not null && _canvas is not null)
        {
            _batch.Begin(samplerState: SamplerState.LinearClamp);
            _navigator.Draw(_canvas);
            _batch.End();
        }

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        Window.TextInput -= OnTextInput;
        _shapes?.Dispose();
        _batch?.Dispose();
        base.UnloadContent();
    }

    // A missing model must not stop the client: every other screen works without
    // it, and the match then shows its interface over an empty background.
    private BoardRenderer? CreateBoardRenderer()
    {
        try
        {
            var assets = BoardAssetSet.Load(Content);
            var camera = new OrbitCamera(assets.Frame, BoardAssetSet.ClassicBoardSize);

            return new BoardRenderer(_graphics.GraphicsDevice, assets, camera);
        }
        catch (ContentLoadException ex)
        {
            _logger.Error("The match models could not be loaded; the board will not be drawn.", ex);
            return null;
        }
    }

    // DesktopGL delivers characters already resolved by the system, with accents and keyboard layout applied.
    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        _input.AddCharacter(e.Character);
    }
}
