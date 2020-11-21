using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    internal class TransitionHelper
    {
        /// <summary>
        /// The direction the transition position should move (0 to 1 = Up; 1 to 0 = Down).
        /// </summary>
        public enum TransitionDirection
        {
            Up,
            Down
        }

        /// <summary>
        /// The states of the transition.
        /// </summary>
        public enum TransitionState
        {
            /// <summary>
            /// The transition has not been started.
            /// </summary>
            NotStarted,

            /// <summary>
            /// The transition is in progress.
            /// </summary>
            InProgress,

            /// <summary>
            /// The transition is complete.
            /// </summary>
            Complete
        }

        private TransitionDirection _direction;
        private int _numericDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionHelper"/> class.
        /// </summary>
        /// <param name="transitionTime">The duration of the transition.</param>
        /// <param name="direction">The direction of the position (0 to 1 = Up; 1 to 0 = Down).</param>
        public TransitionHelper(TimeSpan transitionTime, TransitionDirection direction)
        {
            this.Time = transitionTime;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the total transition duration.
        /// </summary>
        /// <value>
        /// The transition duration.
        /// </value>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Gets the direction of the transition position (0 to 1 = Up; 1 to 0 = Down).
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public TransitionDirection Direction
        {
            get
            {
                return this._direction;
            }
            private set
            {
                this._direction = value;
                this.Reset();
            }
        }

        /// <summary>
        /// Gets the position within the transition.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        /// <remarks>
        /// This value will always be between 0 and 1;
        /// </remarks>
        public float Position { get; private set; }

        /// <summary>
        /// Gets the state of the transition.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public TransitionState State { get; private set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this._numericDirection = (this._direction == TransitionDirection.Down) ? -1 : 1;
            this.Position = (this._direction == TransitionDirection.Down) ? 1.0f : 0.0f;
            this.State = TransitionState.NotStarted;
        }

        /// <summary>
        /// Sets the specified transition.
        /// </summary>
        /// <param name="transitionTime">The duration of the transition.</param>
        /// <param name="direction">The direction of the position (0 to 1 = Up; 1 to 0 = Down).</param>
        public void Set(TimeSpan transitionTime, TransitionDirection direction)
        {
            this.Time = transitionTime;
            this.Direction = direction;
        }

        /// <summary>
        /// Updates the duration position based upon the game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>The state of the transition.</returns>
        public TransitionState Update(GameTime gameTime)
        {
            // How much should we move by?
            float transitionDelta;

            if (this.Time == TimeSpan.Zero)
            {
                transitionDelta = 1.0f;
            }
            else
            {
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / this.Time.TotalMilliseconds);
            }

            // Update the transition position.
            this.Position += transitionDelta * this._numericDirection;

            // Did we reach the end of the transition?
            if (((this._numericDirection < 0) && (this.Position <= 0)) ||
                ((this._numericDirection > 0) && (this.Position >= 1)))
            {
                this.Position = MathHelper.Clamp(this.Position, 0, 1);
                this.State = TransitionState.Complete;
            }
            else
            {
                this.State = TransitionState.InProgress;
            }

            // Return the state
            return this.State;
        }
    }
}
