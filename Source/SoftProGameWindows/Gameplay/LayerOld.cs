using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents a display layer.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Layer" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="info">The information.</param>
        public Layer(ContentManager content, LayerInfo info)
        {
            // Assumes each layer only has 3 segments.
            this.Textures = new List<Texture2D>(info.Segment.Count);
            foreach (var texture in info.Segment)
            {
                this.Textures.Add(content.Load<Texture2D>(texture));
            }

            this.ScrollRate = info.ScrollRate;
        }

        /// <summary>
        /// Gets the textures.
        /// </summary>
        /// <value>
        /// The textures.
        /// </value>
        public List<Texture2D> Textures { get; private set; }

        /// <summary>
        /// Gets the scroll rate.
        /// </summary>
        /// <value>
        /// The scroll rate.
        /// </value>
        public float ScrollRate { get; private set; }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="cameraPosition">The camera position.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            // Assume each segment is the same width.
            int segmentWidth = this.Textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition.X * this.ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            spriteBatch.Draw(this.Textures[leftSegment % this.Textures.Count], new Vector2(x, -cameraPosition.Y), Color.White);
            spriteBatch.Draw(this.Textures[rightSegment % this.Textures.Count], new Vector2(x + segmentWidth, -cameraPosition.Y), Color.White);
        }
    }
}
