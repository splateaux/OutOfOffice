using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputController
{
    /// <summary>
    /// Identifies whether buttons on an Nintendo Controller are pressed or released.
    /// </summary>
    public struct NintendoControllerButtons
    {
        private ControllerKey _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="NintendoControllerButtons"/> struct.
        /// </summary>
        /// <param name="buttons">
        /// The state of the keys on the <see cref="NintendoController"/>.
        /// </param>
        public NintendoControllerButtons(ControllerKey state)
        {
            this._state = state;
        }

        /// <summary>
        /// Identifies if the A button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState A { get { return this._state.HasFlag(ControllerKey.A) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the B button on the Xbox 360 Controller is pressed.
        /// </summary>
        public ButtonState B { get { return this._state.HasFlag(ControllerKey.B) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the SELECT button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState Select { get { return this._state.HasFlag(ControllerKey.Select) ? ButtonState.Pressed : ButtonState.Released; } }

        /// <summary>
        /// Identifies if the START button on the Nintendo Controller is pressed.
        /// </summary>
        public ButtonState Start { get { return this._state.HasFlag(ControllerKey.Start) ? ButtonState.Pressed : ButtonState.Released; } }
    }
}
