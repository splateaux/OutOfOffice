using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace SoftProGameWindows
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    /// <remarks>
    /// This public class is similar to one in the GameStateManagement sample.
    /// </remarks>
    public class ScreenManager : DrawableGameComponent, IServiceProvider
    {
        public const bool DEBUG_ENABLED = false;

        private List<GameScreen> _screens = new List<GameScreen>();
        private List<GameScreen> _screensToUpdate = new List<GameScreen>();
        private List<GameScreen> _screensToDraw = new List<GameScreen>();

        private InputState _input;

        private IGraphicsDeviceService _graphicsDeviceService;
        private IServiceProvider _site;

        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _blankTexture;
        private Rectangle _titleSafeArea;

        private bool _traceEnabled;

        private ObjectValueManager _pointManager;
        private SettingsManager _settingsManager;
        private SoundEffectManager _soundManager;
        private GraphicsDeviceManager _graphicsManager;

        /// <summary>
        /// Gets the point manager... for looking up the point value for something
        /// </summary>
        /// <value>
        /// The point manager.
        /// </value>
        internal ObjectValueManager PointManager
        {
            get { return _pointManager; }
        }

        internal SettingsManager SettingsManager
        {
            get { return _settingsManager; }
        }

        /// <summary>
        /// Expose access to our Game instance (this is protected in the
        /// default GameComponent, but we want to make it public).
        /// </summary>
        new public Game Game
        {
            get { return base.Game; }
        }

        /// <summary>
        /// Expose access to our graphics device (this is protected in the
        /// default DrawableGameComponent, but we want to make it public).
        /// </summary>
        new public GraphicsDevice GraphicsDevice
        {
            get { return base.GraphicsDevice; }
        }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return _font; }
        }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return _traceEnabled; }
            set { _traceEnabled = value; }
        }

        /// <summary>
        /// Gets the transparent texture.
        /// </summary>
        /// <value>
        /// The transparent texture.
        /// </value>
        public Texture2D TransparentTexture
        {
            get { return this._blankTexture; }
        }

        /// <summary>
        /// The title-safe area for the menus.
        /// </summary>
        public Rectangle TitleSafeArea
        {
            get { return _titleSafeArea; }
        }

        /// <summary>
        /// Gets or sets the keyboard dispatcher.
        /// </summary>
        /// <value>
        /// The keyboard dispatcher.
        /// </value>
        public KeyboardDispatcher KeyboardDispatcher { get; set; }

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(SoftProGame game)
            : base(game)
        {
            this._pointManager = new ObjectValueManager();
            this._settingsManager = new SettingsManager();
            this._site = game.Services;
            this._content = new ContentManager(this._site, "Content");
            this._graphicsDeviceService = this._site.GetService<IGraphicsDeviceService>();
            this._input = new InputState(game);
            this._soundManager = new SoundEffectManager();
            this._graphicsManager = game.Graphics;

            if (this._graphicsDeviceService == null)
            {
                throw new InvalidOperationException("No graphics device service.");
            }

            this.KeyboardDispatcher = new KeyboardDispatcher(game.Window);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            this._spriteBatch = new SpriteBatch(GraphicsDevice);
            this._font = this._content.Load<SpriteFont>("Fonts/MenuFont");
            this._blankTexture = this._content.Load<Texture2D>("Textures/Blank");

            // Load up them sounds
            this._soundManager.LoadContent(this._content);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in this._screens)
            {
                screen.LoadContent();
            }

            // update the title-safe area
            this._titleSafeArea = new Rectangle(
                (int)Math.Floor(this.GraphicsDevice.Viewport.X +
                   this.GraphicsDevice.Viewport.Width * 0.05f),
                (int)Math.Floor(this.GraphicsDevice.Viewport.Y +
                   this.GraphicsDevice.Viewport.Height * 0.05f),
                (int)Math.Floor(this.GraphicsDevice.Viewport.Width * 0.9f),
                (int)Math.Floor(this.GraphicsDevice.Viewport.Height * 0.9f));
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload content belonging to the screen manager.
            this._content.Unload();

            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in this._screens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            this._input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others
            // (or it happens on another thread)
            this._screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
                this._screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !this.Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (this._screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = this._screensToUpdate[this._screensToUpdate.Count - 1];

                this._screensToUpdate.RemoveAt(this._screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input and update presence.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(this._input);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (this._traceEnabled) this.TraceScreens();
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        private void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in this._screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of drawing one screen adds or removes others
            // (or it happens on another thread
            this._screensToDraw.Clear();

            foreach (GameScreen screen in this._screens)
                this._screensToDraw.Add(screen);

            foreach (GameScreen screen in this._screensToDraw)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;

            // If we have a graphics device, tell the screen to load content.
            if ((this._graphicsDeviceService != null) &&
                (this._graphicsDeviceService.GraphicsDevice != null))
            {
                screen.LoadContent();
            }

            this._screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if ((this._graphicsDeviceService != null) &&
                (this._graphicsDeviceService.GraphicsDevice != null))
            {
                screen.UnloadContent();
            }

            this._screens.Remove(screen);
            this._screensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return this._screens.ToArray();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            this._spriteBatch.Begin();

            this._spriteBatch.Draw(this._blankTexture,
                                   new Rectangle(0, 0, viewport.Width, viewport.Height),
                                   Color.Black * alpha);

            this._spriteBatch.End();
        }

        #region IServiceProvider Implementation

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ScreenManager))
            {
                return this;
            }
            else if (serviceType == typeof(ObjectValueManager))
            {
                return this.PointManager;
            }
            else if (serviceType == typeof(SoundEffectManager))
            {
                return this._soundManager;
            }
            else if (serviceType == typeof(GraphicsDeviceManager))
            {
                return this._graphicsManager;
            }
            else if (serviceType == typeof(SettingsManager))
            {
                return this._settingsManager;
            }
            else if (serviceType == typeof(KeyboardDispatcher))
            {
                return this.KeyboardDispatcher;
            }
            else if (this._site != null)
            {
                return this._site.GetService(serviceType);
            }


            return null;
        }

        #endregion
    }
}