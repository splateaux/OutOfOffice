using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputController
{
    /// <summary>
    /// Identifies which directions on the directional pad of an Nintendo Controller are being pressed.
    /// </summary>
    public struct NintendoControllerDPad
    {
        private ControllerKey _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="NintendoControllerButtons"/> struct.
        /// </summary>
        /// <param name="buttons">
        /// The state of the keys on the <see cref="NintendoController"/>.
        /// </param>
        public NintendoControllerDPad(ControllerKey state)
        {
            this._state = state;
        }

        /// <summary>
        /// Identifies if the A button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState Down { get { return this._state.HasFlag(ControllerKey.Down) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the B button on the Xbox 360 Controller is pressed.
        /// </summary>
        public ButtonState Left { get { return this._state.HasFlag(ControllerKey.Left) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the SELECT button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState Right { get { return this._state.HasFlag(ControllerKey.Right) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the START button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState Up { get { return this._state.HasFlag(ControllerKey.Up) ? ButtonState.Pressed : ButtonState.Released; } }
    }
}
