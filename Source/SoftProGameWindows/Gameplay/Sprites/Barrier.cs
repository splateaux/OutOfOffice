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
    /// A representation of a barrier object
    /// </summary>
    public class Barrier : CollidableSprite
    {
        private Color _color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Barrier"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collidableType">Type of the collidable.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="color">The color.</param>
        public Barrier(IServiceProvider site, Vector2 position, CollidableType collidableType, Rectangle shape, Color color)
            : base(site, position, collidableType)
        {
            this.SourceRectangle = shape;
            this._color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Barrier"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Barrier(SpriteInfo info)
            : base(info)
        {
            var setting = info.Settings.FirstOrDefault(s => s.Key.Equals("Shape", StringComparison.OrdinalIgnoreCase));
            this.SourceRectangle = Utils.RectangleUtils.Parse(setting.Value);

            setting = info.Settings.FirstOrDefault(s => s.Key.Equals("Color", StringComparison.OrdinalIgnoreCase));
            this._color = Utils.ColorUtils.Parse(setting.Value);
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { this._color });
            
            spriteBatch.Draw(pixel, this.BoundingBox, Color.White);

#if DEBUG
            // Draw debug outline
            if (ScreenManager.DEBUG_ENABLED)
            {
                var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.Purple);
            }
#endif
        }
    }
}
