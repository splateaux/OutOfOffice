using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Pause screen for mini games
    /// </summary>
    class MiniGamePauseScreen : MenuScreen
    {
        private string _controllerTexture;
        private ContentManager _content;
        private Texture2D _controls;

        /// <summary>
        /// Occurs when [quit selected].
        /// </summary>
        public event EventHandler QuitSelected;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MiniGamePauseScreen(MiniGameScreen game)
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Mini Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Mini Game");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += quitGameMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            // Set the name of the controller texture based on the type of game
            var format = @"Textures/Objects/{0}/Controls";
            if (game is FridgeGameScreen)
            {
                this._controllerTexture = string.Format(format, "FridgeGame");
            }
            else if (game is MazeGameScreen)
            {
                this._controllerTexture = string.Format(format, "MazeGame");
            }
            else if (game is QuizGameScreen)
            {
                this._controllerTexture = string.Format(format, "QuizGame");
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            if (this._content == null)
            {
                this._content = new ContentManager(this.ScreenManager.Game.Services, "Content");
            }

            this._controls = this._content.Load<Texture2D>(this._controllerTexture);
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public override void UnloadContent()
        {
            if (this._content != null) this._content.Unload();
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 520f);

            // update each menu entry's location in turn
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                MenuEntry menuEntry = this._menuEntries[i];

                // each entry is to be centered horizontally
                position.X = this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.TransitionPosition == 0.0f)
            {
                this.ScreenManager.SpriteBatch.Begin();
                this.ScreenManager.SpriteBatch.Draw(this._controls, this.ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);
                this.ScreenManager.SpriteBatch.End();
            }
        }

        /// <summary>
        /// Handles the Selected event of the quitGameMenuEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        private void quitGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (QuitSelected != null)
            {
                QuitSelected(this, new EventArgs());
            }

            this.ExitScreen();
        }

    }
}
