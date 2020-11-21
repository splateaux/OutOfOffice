using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// A basic representation of a collidable game object.
    /// </summary>
    public class CollidableSprite : Sprite, ICollidable
    {
        private CollidableType _collidableType = CollidableType.Impassable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collidableType">The collidable type of the sprite.</param>
        /// <param name="texture">The texture (default = null).</param>
        /// <param name="scale">The scale (default = 1).</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CollidableSprite(IServiceProvider site, Vector2 position, CollidableType? collidableType = CollidableType.Passable, Texture2D texture = null, float scale = 1.0f)
            : base(site, position, texture, null, Color.White, 0.0f, Vector2.Zero, scale)
        {
            this._collidableType = collidableType.HasValue ? collidableType.Value : CollidableType.Passable;
            if (CollisionManager.Collection != null) CollisionManager.Collection.Insert(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollidableSprite"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public CollidableSprite(SpriteInfo info)
            : base(info)
        {
            var setting = info.Settings.FirstOrDefault(s => s.Key.Equals("CollidableType", StringComparison.OrdinalIgnoreCase));
            if ((setting == null) || !Enum.TryParse(setting.Value, out this._collidableType))
            {
                this._collidableType = CollidableType.Passable;
            }

            if (CollisionManager.Collection != null) CollisionManager.Collection.Insert(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollidableSprite"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public CollidableSprite(SpriteInfo info, CollidableType collidableType)
            : base(info)
        {
            this._collidableType = collidableType;
            if (CollisionManager.Collection != null) CollisionManager.Collection.Insert(this);
        }

        /// <summary>
        /// Unloads the content of the sprite.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();
            if (CollisionManager.Collection != null) CollisionManager.Collection.QueuePendingRemoval(this);
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.Texture != null)
            {
                spriteBatch.Draw(this.Texture, this.Position, Color.White);

#if DEBUG
                // Draw debug outline
                if (ScreenManager.DEBUG_ENABLED)
                {
                    var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                    boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                    DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.Yellow);
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
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool IsActive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the bounding box of the sprite.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        /// <remarks>
        /// Used for collision detection. This rectangle is aligned on the upper-left hand corner.
        /// </remarks>
        public virtual Rectangle BoundingBox
        {
            get
            {
                var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;

                var screenPosition = (this.Layer != null) ? CollisionManager.ConvertWorldPosition(this.Position, this.Layer) : this.Position;
                var x = (int)Math.Floor(screenPosition.X + this.Origin.X);
                var y = (int)Math.Floor(screenPosition.Y + this.Origin.Y);

                return new Rectangle(x, y, boundingBox.Width, boundingBox.Height);
            }
        }

        /// <summary>
        /// Gets the type of the collidable.
        /// </summary>
        /// <value>
        /// The type of the collidable.
        /// </value>
        public CollidableType CollidableType
        {
            get { return this._collidableType; }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="info">The collision information.</param>
        public virtual void HandleCollisions(CollisionInfo info)
        {
        }

        #endregion
    }
}
