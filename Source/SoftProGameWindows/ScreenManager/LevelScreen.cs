using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// This screen implements the base level functionality.  You need to derive from this
    /// screen to create levels, sub-levels, and mini-games.
    /// </summary>
    public abstract class LevelScreen : GameScreen
    {
        private ContentManager _content;
        private Player _player;
        private List<Sprite> _sprites;

        private Texture2D _statusTexture;
        private Vector2 _statusVector;

        private Hud _hud;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelScreen"/> class.
        /// </summary>
        public LevelScreen()
        {
            this._sprites = new List<Sprite>();
        }

        /// <summary>
        /// Gets the content manager.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        protected ContentManager Content
        {
            get
            {
                if ((this._content == null) && (this.ScreenManager != null))
                {
                    this._content = new ContentManager(this.ScreenManager, "Content");
                }

                return this._content;
            }
        }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <remarks>
        /// Player must be spawned using the <see cref="SpawnPlayer"/> method to set this value.
        /// </remarks>
        /// <value>
        /// The player.
        /// </value>
        protected Player Player
        {
            get
            {
                return this._player;
            }
        }

        /// <summary>
        /// Gets the hud.
        /// </summary>
        /// <value>
        /// The hud.
        /// </value>
        public Hud Hud
        {
            get { return _hud; }
        }

        /// <summary>
        /// Gets the sprites on the level.
        /// </summary>
        /// <value>
        /// The sprites.
        /// </value>
        protected List<Sprite> Sprites
        {
            get
            {
                return this._sprites;
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">LevelScreen</exception>
        public override void LoadContent()
        {
            base.LoadContent();
            this._hud = new Hud(this.ScreenManager);
            this._hud.LoadContent(this.Content);
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var sprite in this.Sprites)
            {
                var collidable = sprite as ICollidable;
                if ((collidable != null) && (CollisionManager.Collection != null))
                {
                    CollisionManager.Collection.Remove(collidable);
                }
            }

            if (this.Content != null) this.Content.Unload();
        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        /// <param name="otherScreenHasFocus">if set to <c>true</c> [other screen has focus].</param>
        /// <param name="coveredByOtherScreen">if set to <c>true</c> [covered by other screen].</param>
        public override sealed void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            this.Hud.Update(this.Player);
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        /// <param name="input">The input.</param>
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
            this.DrawStatus();
            this.Hud.Draw(this.ScreenManager.SpriteBatch);
        }

        /// <summary>
        /// Draws the status texture, if one has been set
        /// </summary>
        protected virtual void DrawStatus()
        {
            this.ScreenManager.SpriteBatch.Begin();

            if (_statusTexture != null)
            {
                // Draw status message.
                this.ScreenManager.SpriteBatch.Draw(_statusTexture, _statusVector, Color.White);
            }

            this.ScreenManager.SpriteBatch.End();
        }

        /// <summary>
        /// Spawns the player.
        /// </summary>
        /// <param name="player">The player.</param>
        public virtual void SpawnPlayer(Player player)
        {
            this._player = player;

            // Make sure the sound manager knows about this player
            SoundEffectManager manager = this.ScreenManager.GetService<SoundEffectManager>();
            manager.SetPlayer(player);
        }

        #region IServiceProvider Implementation

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType == typeof(Hud))
            {
                return this.Hud;
            }

            return base.GetService(serviceType);
        }

        #endregion
    }
}