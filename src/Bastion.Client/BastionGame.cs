using System;
using System.Globalization;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Screens;

namespace Bastion.Client;

public sealed class BastionGame : Game
{
    private const string ContentDirectory = "Content";
    private const string RegularFontName = "Regular";
    private const string BoldFontName = "Bold";
    private const string TitleFontName = "Title";

    private static readonly ILog _logger = LogManager.GetLogger(typeof(BastionGame));

    private readonly GraphicsDeviceManager _graphics;
    private readonly InputState _input = new InputState();
    private readonly Navigator _navigator;

    private SpriteBatch? _batch;
    private ShapeRenderer? _shapes;
    private Canvas? _canvas;

    public BastionGame(ClientServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Theme.WindowWidth,
            PreferredBackBufferHeight = Theme.WindowHeight,
            SynchronizeWithVerticalRetrace = true,
        };

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
        _navigator.Start(ScreenId.MainScreen);
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
        _graphics.GraphicsDevice.Clear(Theme.Background);
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

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        _input.AddCharacter(e.Character);
    }
}
