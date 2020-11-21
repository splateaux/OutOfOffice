using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public struct AnimationPlayer
    {
        private float _time;
        private int _frameIndex;

        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation { get; private set; }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public AnimationFrame CurrentFrame { get; private set; }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (this.Animation == animation)
                return;

            // Start the new animation.
            this.Animation = animation;
            this.CurrentFrame = animation.Frames[0];
            this._frameIndex = 0;
            this._time = 0.0f;
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (this.Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            this._time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time > this.Animation.FrameTime)
            {
                this._time -= this.Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (this.Animation.IsLooping)
                {
                    this._frameIndex = (this._frameIndex + 1) % this.Animation.FrameCount;
                }
                else
                {
                    this._frameIndex = Math.Min(this._frameIndex + 1, this.Animation.FrameCount - 1);
                }
            }

            // Calculate the source rectangle of the current frame.
            this.CurrentFrame = this.Animation.Frames[this._frameIndex];

            // Draw the current frame.
            spriteBatch.Draw(this.Animation.Texture, position, this.CurrentFrame.SourceRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffects, 0.0f);
        }
    }
}