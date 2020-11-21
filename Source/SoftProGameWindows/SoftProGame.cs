using Microsoft.Xna.Framework;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace SoftProGameWindows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SoftProGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private ScreenManager _screenManager;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        private static readonly string[] _preloadAssets =
        {
            "Textures/gradient",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftProGame"/> class.
        /// </summary>
        public SoftProGame()
        {
            Content.RootDirectory = "Content";

            // Setup the graphics device manager
            this._graphics = new GraphicsDeviceManager(this);
            this._graphics.PreferredBackBufferWidth = 1280;
            this._graphics.PreferredBackBufferHeight = 720;

            // Determine whether or not to load in full screen mode
            NameValueCollection collection = ConfigurationManager.GetSection("Settings") as NameValueCollection;
            var useFullScreen = Boolean.Parse(collection["FullScreen"]);
            this._graphics.IsFullScreen = useFullScreen;

            // Create the screen manager component
            this._screenManager = new ScreenManager(this);
            this.Components.Add(this._screenManager);

            // Activate the first screens
            this._screenManager.AddScreen(new BackgroundScreen(Constants.MAINGAME_BACKGROUND_TEXTURE), null);
            this._screenManager.AddScreen(new MainMenuScreen(), null);
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>
        /// The graphics.
        /// </value>
        public GraphicsDeviceManager Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the preload assets
            foreach (string asset in SoftProGame._preloadAssets)
            {
                this.Content.Load<object>(asset);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear to a black background initially
            this._graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
    }

    #region Entry Point

#if WINDOWS || XBOX

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            using (SoftProGame game = new SoftProGame())
            {
                game.Run();
            }
        }
    }

#endif

    #endregion Entry Point
}