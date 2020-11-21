using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputController
{
    /// <summary>
    /// Represents specific information about the state of a Nintendo Controller,
    /// including the current state of buttons and direction pad.
    /// </summary>
    public struct NintendoControllerState
    {
        private Buttons _state;
        private bool _isConnected;
        private NintendoControllerButtons _nintendoButtons;
        private NintendoControllerDPad _dpad;

        /// <summary>
        /// The disconnected
        /// </summary>
        public static NintendoControllerState Disconnected = new NintendoControllerState()
        {
            _state = (Buttons)0,
            _isConnected = false
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="NintendoControllerState" /> struct.
        /// </summary>
        /// <param name="state">The state of the keys on the <see cref="NintendoController"/>.</param>
        public NintendoControllerState(ControllerKey state)
        {
            this._state = (Buttons)0;
            this._isConnected = true;
            this._nintendoButtons = new NintendoControllerButtons(state);
            this._dpad = new NintendoControllerDPad(state);

            // Calculate the button states
            this._state |= state.HasFlag(ControllerKey.A) ? Microsoft.Xna.Framework.Input.Buttons.A : 0;
            this._state |= state.HasFlag(ControllerKey.B) ? Microsoft.Xna.Framework.Input.Buttons.B : 0;
            this._state |= state.HasFlag(ControllerKey.Select) ? Microsoft.Xna.Framework.Input.Buttons.Back : 0;
            this._state |= state.HasFlag(ControllerKey.Start) ? Microsoft.Xna.Framework.Input.Buttons.Start : 0;

            // Calculate the directional pad states
            this._state |= state.HasFlag(ControllerKey.Up) ? Microsoft.Xna.Framework.Input.Buttons.DPadUp : 0;
            this._state |= state.HasFlag(ControllerKey.Down) ? Microsoft.Xna.Framework.Input.Buttons.DPadDown : 0;
            this._state |= state.HasFlag(ControllerKey.Left) ? Microsoft.Xna.Framework.Input.Buttons.DPadLeft : 0;
            this._state |= state.HasFlag(ControllerKey.Right) ? Microsoft.Xna.Framework.Input.Buttons.DPadRight : 0;
        }

        /// <summary>
        /// Returns a structure that identifies what buttons on the Nintendo controller are pressed.
        /// </summary>
        public NintendoControllerButtons Buttons { get { return this._nintendoButtons; } }

        /// <summary>
        /// Returns a structure that identifies what directions of the directional pad on the 
        /// Nintendo Controller are pressed.
        /// </summary>
        public NintendoControllerDPad DPad { get { return this._dpad; } }

        /// <summary>
        /// Indicates whether the Nintendo Controller is connected.
        /// </summary>
        public bool IsConnected { get { return this._isConnected; } }

        /// <summary>
        /// Determines whether specified input device buttons are pressed in this <see cref="NintendoControllerState"/>.
        /// </summary>
        /// <param name="button">
        /// Buttons to query. Specify a single button, or combine multiple buttons using a 
        /// bitwise OR operation.
        /// </param>
        /// <returns></returns>
        public bool IsButtonDown(Buttons button)
        {
            return (this._state & button) == button;
        }

        /// <summary>
        /// Determines whether specified input device buttons are up (not pressed) in this <see cref="NintendoControllerState"/>.
        /// </summary>
        /// <param name="button">
        /// Buttons to query. Specify a single button, or combine multiple buttons using 
        /// a bitwise OR operation.</param>
        /// <returns></returns>
        public bool IsButtonUp(Buttons button)
        {
            return (this._state & button) == (Buttons)0;
        }
    }
}
