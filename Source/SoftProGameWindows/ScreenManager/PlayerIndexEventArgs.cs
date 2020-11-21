using Microsoft.Xna.Framework;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerIndexEventArgs"/> class.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this._playerIndex = playerIndex;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return this._playerIndex; }
        }

        private PlayerIndex _playerIndex;
    }
}