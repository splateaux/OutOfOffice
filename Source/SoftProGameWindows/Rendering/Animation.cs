using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    public class Animation
    {
        /// <summary>
        /// Constructors a new animation.
        /// </summary>
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.Texture = texture;
            this.FrameTime = frameTime;
            this.IsLooping = isLooping;
            this.FrameCount = texture.Width / texture.Height;
            this.FrameWidth = texture.Height; // Assume square frames
            this.FrameHeight = texture.Height;

            this.Frames = new List<AnimationFrame>(this.FrameCount);
            for (int frameIndex = 0; frameIndex < this.FrameCount; frameIndex++)
            {
                var sourceRectangle = new Rectangle(frameIndex * this.FrameWidth, 0, this.FrameWidth, this.FrameHeight);
                var boundingBox = CollisionManager.GetSmallestRectangleFromTexture(this.Texture, sourceRectangle);
                this.Frames.Add(new AnimationFrame(frameIndex, sourceRectangle, boundingBox));
            }
        }

        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime { get; private set; }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping { get; private set; }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Gets the frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public List<AnimationFrame> Frames { get; private set; }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        /// <remarks>
        /// Assume square frames.
        /// </remarks>
        public int FrameWidth { get; private set; }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight { get; private set; }
    }
}