using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class SoftProDollar : CollidableSprite
    {
        private const float BOUNCE_HEIGHT = 0.18f;
        private float BOUNCE_RATE = 3.0f;
        private float BOUNCE_SYNC = -0.75f;

        // Physics state
        private Vector2 _basePosition; // The dollar is animated from a base position along the Y axis.

        // SoftPro Dollar Info
        public readonly Color Color = Color.Yellow;
        private SoundEffect _collectedSound;
        private bool _isActive;

        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public SoftProDollar(IServiceProvider site, Vector2 position)
            : base(site, position, CollidableType.Passable)
        {
            this._basePosition = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftProDollar"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public SoftProDollar(SpriteInfo info)
            : base(info, CollidableType.Passable)
        {
            this._basePosition = info.Position;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content mananger.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            this.Texture = content.Load<Texture2D>(Constants.SOFTPRODOLLAR_TEXTURE);
            this._collectedSound = content.Load<SoundEffect>(Constants.SOFTPRODOLLAR_SOUND);
            this._isActive = true;
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="input">The input.</param>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame.
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                // Bounce along a sine curve over time.
                // Include the X coordinate so that neighboring dollars bounce in a nice wave pattern.
                var t = gameTime.TotalGameTime.TotalSeconds * BOUNCE_RATE + this.Position.X * BOUNCE_SYNC;
                var bounce = (float)Math.Sin(t) * BOUNCE_HEIGHT * this.Texture.Height;
                this.Position = this._basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.IsActive)
            {
                spriteBatch.Draw(this.Texture, this.Position, null, this.Color);

#if DEBUG
                // Draw debug outline
                if (ScreenManager.DEBUG_ENABLED)
                {
                    var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                    boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                    DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.Green);
                }
#endif
            }
        }

        #region ICollidable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public override bool IsActive
        {
            get { return this._isActive; }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="info">The collision information.</param>
        public override void HandleCollisions(CollisionInfo info)
        {
            var player = info.Obstacle as Player;
            if ((player != null) && this._isActive)
            {
                SoundEffectManager manager = this.GetService<SoundEffectManager>();
                manager.PlayDollarSound();
                this._isActive = false;
                CollisionManager.Collection.QueuePendingRemoval(this);
            }
        }

        #endregion
    }
}