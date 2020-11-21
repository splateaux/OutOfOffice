using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents one frame of animation.
    /// </summary>
    public class AnimationFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationFrame"/> class.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <param name="sourceRectangle">The source rectangle.</param>
        /// <param name="boundingBox">The bounding box.</param>
        public AnimationFrame(int frameIndex, Rectangle sourceRectangle, Rectangle boundingBox)
        {
            this.FrameIndex = frameIndex;
            this.SourceRectangle = sourceRectangle;
            this.BoundingBox = boundingBox;
            this.BoundingBoxOffset = new Vector2(boundingBox.X, boundingBox.Y);
        }

        /// <summary>
        /// Gets the index of the frame.
        /// </summary>
        /// <value>
        /// The index of the frame.
        /// </value>
        public int FrameIndex { get; private set; }

        /// <summary>
        /// Gets or sets the source rectangle.
        /// </summary>
        /// <value>
        /// The source rectangle.
        /// </value>
        public Rectangle SourceRectangle { get; private set; }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public Rectangle BoundingBox { get; private set; }

        /// <summary>
        /// Gets or sets the bounding box offset.
        /// </summary>
        /// <value>
        /// The bounding box offset.
        /// </value>
        public Vector2 BoundingBoxOffset { get; private set; }
    }
}
