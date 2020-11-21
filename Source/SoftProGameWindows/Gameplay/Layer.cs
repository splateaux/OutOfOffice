using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents a layer
    /// </summary>
    public class Layer
    {
        private readonly Camera _camera;
        private HashSet<Sprite> _sprites;

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer" /> class.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public Layer(Camera camera)
        {
            this._camera = camera;
            this._sprites = new HashSet<Sprite>();
            this.Parallax = Vector2.One;
        }

        /// <summary>
        /// Gets or sets the parallax.
        /// </summary>
        /// <value>
        /// The parallax.
        /// </value>
        public Vector2 Parallax { get; set; }

        /// <summary>
        /// Allows the screen to run logic.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (var sprite in this._sprites)
            {
                sprite.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, this._camera.GetViewMatrix(this.Parallax));

            foreach (var sprite in this._sprites)
            {
                sprite.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Adds the sprite to the layer.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void AddSprite(Sprite sprite)
        {
            if (this._sprites.Add(sprite))
            {
                sprite.Layer = this;
            }
        }

        /// <summary>
        /// Removes the sprite from the layer.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void RemoveSprite(Sprite sprite)
        {
            if (this._sprites.Remove(sprite))
            {
                sprite.Layer = null;
            }
        }

        /// <summary>
        /// Gets a screen position from a world position.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <returns></returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, this._camera.GetViewMatrix(this.Parallax));
        }

        /// <summary>
        /// Gets the world position from a screen position.
        /// </summary>
        /// <param name="screenPosition">The screen position.</param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(this._camera.GetViewMatrix(this.Parallax)));
        }
    }
}