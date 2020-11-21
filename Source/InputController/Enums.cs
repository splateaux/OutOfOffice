using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputController
{
    /// <summary>
    /// All user game play input can be reduced to one or more of the following
    /// states.  Methods to retrieve the user input status may return a value
    /// that is the logical OR of one or more enum states.
    /// </summary>
    [Flags]
    public enum ControllerKey
    {
        // Player movement - Nintendo DPad or Keyboard arrow key.
        Stationary  = 0x00,
        Right       = 0x01,
        Left        = 0x02,
        Up          = 0x04,
        Down        = 0x08,

        DirectionKeyMask = 0x0f,    // AND mask for direction keys.

        // Remaining Nintendo buttons (from left to right on console).
        Select      = 0x10,
        Start       = 0x20,
        B           = 0x40,
        A           = 0x80

        // Other Keyboard keys.
    }
}
