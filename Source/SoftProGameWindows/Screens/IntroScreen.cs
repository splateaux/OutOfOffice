using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// The intro screen shows the video story at the beginning of the game sequence.
    /// </summary>
    public class IntroScreen : MenuScreen
    {
        private ContentManager _content;
        private string _selectedCharacterName;
        private Video _intro;
        private VideoPlayer _player;
        private bool _playingIntroMovie;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public IntroScreen(string selectedCharacterName)
            : base("")
        {
            this._selectedCharacterName = selectedCharacterName;
            this._player = new VideoPlayer();
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            if ((this._content == null) && (this.ScreenManager != null))
            {
                this._content = new ContentManager(this.ScreenManager, "Content");
            }

            // Load and add the email messages
            this._intro = this._content.Load<Video>("Videos/OutOfOfficeIntro");
            this._playingIntroMovie = true;
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            if (this._content != null)
            {
                this._content.Unload();
            }
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            if (this._player.State != MediaState.Stopped) this._player.Stop();

            this.ScreenManager.RemoveScreen(this);
            LoadingScreen.Load(this.ScreenManager, true, PlayerIndex.One, new GameplayScreen(this._selectedCharacterName));
        }

        /// <summary>
        /// Called when update occurs.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            if (this._playingIntroMovie == true)
            {
                this._player.Play(this._intro);
                this._playingIntroMovie = false;
            }

            if (this._player.State == MediaState.Stopped)
            {
                this._player.Stop();

                this.ScreenManager.RemoveScreen(this);
                LoadingScreen.Load(this.ScreenManager, true, PlayerIndex.One, new GameplayScreen(this._selectedCharacterName));
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            if (this._player.State != MediaState.Stopped)
            {
                this.ScreenManager.SpriteBatch.Begin();
                this.ScreenManager.SpriteBatch.Draw(this._player.GetTexture(), this.ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);
                this.ScreenManager.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}