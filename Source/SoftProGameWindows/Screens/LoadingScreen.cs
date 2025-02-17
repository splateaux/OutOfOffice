using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    ///
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    public class LoadingScreen : GameScreen
    {
        private bool _loadingIsSlow;
        private bool _otherScreensAreGone;
        private GameScreen[] _screensToLoad;

        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        /// <param name="screenManager">The screen manager.</param>
        /// <param name="loadingIsSlow">if set to <c>true</c> [loading is slow].</param>
        /// <param name="screensToLoad">The screens to load.</param>
        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            this._loadingIsSlow = loadingIsSlow;
            this._screensToLoad = screensToLoad;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        /// <param name="screenManager">The screen manager.</param>
        /// <param name="loadingIsSlow">if set to <c>true</c> [loading is slow].</param>
        /// <param name="controllingPlayer">The controlling player.</param>
        /// <param name="screensToLoad">The screens to load.</param>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }


        /// <summary>
        /// Called when [update].
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (this._otherScreensAreGone)
            {
                this.ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in this._screensToLoad)
                {
                    if (screen != null)
                    {
                        this.ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                this.ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((this.ScreenState == ScreenState.Active) &&
                (this.ScreenManager.GetScreens().Length == 1))
            {
                this._otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (this._loadingIsSlow)
            {
                SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
                SpriteFont font = this.ScreenManager.Font;

                const string message = "Loading...";

                // Center the text in the viewport.
                Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * this.TransitionAlpha;

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }
    }
}