using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputController
{
    /// <summary>
    /// Allows retrieval of user interaction with a Nintendo Controller.
    /// </summary>
    public class NintendoController
    {
        private const int DEBOUNCE_PERIOD = 3;
        private UsbSingleNintendoController _controller;
        private NintendoControllerState _previousState;
        private int _debounce = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="NintendoController"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        public NintendoController(Game game)
        {
            this._controller = new UsbSingleNintendoController(game.Window.Handle);
            this._previousState = NintendoControllerState.Disconnected;
        }

        /// <summary>
        /// Returns the current Nintendo Controller state.
        /// </summary>
        /// <returns></returns>
        public NintendoControllerState GetState()
        {
            var state = this._previousState;

            if (--this._debounce <= 0)
            {
                if (this._controller.QueryDevice())
                {
                    state = new NintendoControllerState(this._controller.ControllerInput);
                }
                else
                {
                    state = NintendoControllerState.Disconnected;
                }

                this._debounce = DEBOUNCE_PERIOD;
                this._previousState = state;
            }

            return state;
        }
    }
}
