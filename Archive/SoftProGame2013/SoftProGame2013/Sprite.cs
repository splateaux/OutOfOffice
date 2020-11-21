using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{
    /// <summary>
    /// A basic visual item
    /// </summary>
    class Sprite
    {
        private Texture2D _texture;
        public Vector2 Position;
        public Vector2 Velocity;
        private float _scale;
        private bool _isDestroyed;

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Convert.ToInt32(_texture.Width * _scale),
                    Convert.ToInt32(_texture.Height * _scale));
            }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public float Scale
        {
            get { return _scale; }
            set 
            {
                if (_scale != value)
                {
                    _scale = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is destroyed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is destroyed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDestroyed
        {
            get { return _isDestroyed; }
            set { _isDestroyed = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public Sprite(Texture2D texture, Vector2 position, float scale = 1)
        {
            this._texture = texture;
            this.Position = position;
            this.Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="velocity">The velocity.</param>
        /// <param name="scale">The scale.</param>
        public Sprite(Texture2D texture, Vector2 position, Vector2 velocity, float scale = 1)
        {
            this._texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Scale = scale;
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _texture.Bounds, Color.White, 0f, new Vector2(), Scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Updates the specified background moving.
        /// </summary>
        /// <param name="backgroundMoving">if set to <c>true</c> [background moving].</param>
        /// <param name="scrollSpeed">The scroll speed.</param>
        public virtual void Update(bool backgroundMoving, int scrollSpeed)
        {
            if (backgroundMoving)
            {
                Position.X -= scrollSpeed;
            }
        }
    }
}
