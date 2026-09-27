using System.Globalization;
using System.Resources;
using log4net;
using Microsoft.Xna.Framework;

namespace Bastion.Client;

public sealed class BastionGame : Game
{
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const string ContentDirectory = "Content";
    private const string StringsBaseName = "Bastion.Client.Localization.Strings";
    private const string GameTitleKey = "Game.Title";

    private static readonly ILog _logger = LogManager.GetLogger(typeof(BastionGame));
    private static readonly ResourceManager _strings = new ResourceManager(
        StringsBaseName,
        typeof(BastionGame).Assembly);

    private readonly GraphicsDeviceManager _graphics;

    public BastionGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight,
        };
        Content.RootDirectory = ContentDirectory;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.Title = _strings.GetString(GameTitleKey, CultureInfo.CurrentUICulture) ?? string.Empty;
        _logger.Info($"Client started. Culture={CultureInfo.CurrentUICulture.Name}");
        base.Initialize();
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
    }
}
