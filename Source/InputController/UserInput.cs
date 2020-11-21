using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace InputController
{
    /// <summary>
    /// Methods to query the user input.  This class will be fleshed out as USB
    /// functionality is added.  Call GetState() to get the aggregate of all
    /// user input - be it computer keyboard or Nintendo controller.
    /// </summary>
    public partial class UserInput
    {
        private KeyboardState _keyboardState;
        private static UserInput _instance = null;
        private UsbSingleNintendoController _usb;
        private IntPtr _parentHandle = IntPtr.Zero;
        private int _usbReadDelay = 10;

        /// <summary>
        /// Consumer cannot create a new instance of this class.
        /// Instead must call this method to get the one and only class instance.
        /// </summary>
        /// <returns></returns>
        public static UserInput GetClassInstance(IntPtr parentHandle)
        {
            if (_instance == null)
            {
                // Create object only once.
                _instance = new UserInput(parentHandle);
            }

            return _instance;
        }

        /// <summary>
        /// Initialize state.
        /// </summary>
        private UserInput(IntPtr parentHandle)
        {
            _parentHandle = parentHandle;

            // Instantiate controller and open device.
            _usb = new UsbSingleNintendoController(parentHandle);
            _usb.FindController();
        }

        #region Discrete button press utility properties.

        /// <summary>
        /// Right direction button pressed.
        /// </summary>
        public bool RightPressed
        {
            get
            {
                return IsPressed(ControllerKey.Right);
            }
        }

        /// <summary>
        /// Left direction button pressed.
        /// </summary>
        public bool LeftPressed
        {
            get
            {
                return IsPressed(ControllerKey.Left);
            }
        }

        /// <summary>
        /// Up direction button pressed.
        /// </summary>
        public bool UpPressed
        {
            get
            {
                return IsPressed(ControllerKey.Up);
            }
        }

        /// <summary>
        /// Down direction button pressed.
        /// </summary>
        public bool DownPressed
        {
            get
            {
                return IsPressed(ControllerKey.Down);
            }
        }

        /// <summary>
        /// Select Nintendo button pressed.
        /// </summary>
        public bool SelectPressed
        {
            get
            {
                return IsPressed(ControllerKey.Select);
            }
        }

        /// <summary>
        /// Start Nintendo button pressed.
        /// </summary>
        public bool StartPressed
        {
            get
            {
                return IsPressed(ControllerKey.Start);
            }
        }

        /// <summary>
        /// "A" Nintendo button pressed.
        /// </summary>
        public bool APressed
        {
            get
            {
                return IsPressed(ControllerKey.A);
            }
        }

        /// <summary>
        /// "B" Nintendo button pressed.
        /// </summary>
        public bool BPressed
        {
            get
            {
                return IsPressed(ControllerKey.B);
            }
        }

        #endregion

        /// <summary>
        /// Called once per frame by main timing loop to perform a single poll from
        /// the keyboard and USB as necessary.  This provides way to poll USB
        /// for a mininum number of times per frame instead of multiple times
        /// when GetState() is called.
        /// </summary>
        public void PollUserInput()
        {
            _keyboardState = Keyboard.GetState();

            if (_usbReadDelay > 0)
            {
                _usbReadDelay--;
            }
            else
            {
                _usb.QueryDevice();
            }
        }

        /// <summary>
        /// Return user input direction keys - can return two keys.
        /// </summary>
        /// <returns></returns>
        public ControllerKey GetDirectionKeys()
        {
            ControllerKey state = ControllerKey.Stationary;

            // Get USB controller state if present.
            ControllerKey usbState = _usb.ControllerInput;

            // Exclusive Left or Right
            if (_keyboardState.IsKeyDown(Keys.Left) || usbState == ControllerKey.Left)
            {
                state |= ControllerKey.Left;
            }
            else if (_keyboardState.IsKeyDown(Keys.Right) || usbState == ControllerKey.Right)
            {
                state |= ControllerKey.Right;
            }

            // Exclusive Up or Down
            if (_keyboardState.IsKeyDown(Keys.Up) || usbState == ControllerKey.Up)
            {
                state |= ControllerKey.Up;
            }
            else if (_keyboardState.IsKeyDown(Keys.Down) || usbState == ControllerKey.Down)
            {
                state |= ControllerKey.Down;
            }

            return state;
        }

        /// <summary>
        /// Get user button press information.  Directions are formed by
        /// calling GetDirection().
        /// </summary>
        /// <returns></returns>
        public ControllerKey GetAllKeys()
        {
            ControllerKey key = GetDirectionKeys();

            // For developers - map remaining Nintendo keys to keyboard.
            if (_keyboardState.IsKeyDown(Keys.F1))
            {
                key |= ControllerKey.Select;
            }
            if (_keyboardState.IsKeyDown(Keys.F2))
            {
                key |= ControllerKey.Start;
            }
            if (_keyboardState.IsKeyDown(Keys.F3))
            {
                key |= ControllerKey.B;
            }
            if (_keyboardState.IsKeyDown(Keys.F4))
            {
                key |= ControllerKey.A;
            }

            return key;
        }

        /// <summary>
        /// Return true if all buttons passed in ControllerKey enum are pressed.
        /// This method uses GetDirection() so only certain simultaneous direction
        /// keys are detectable.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsPressed(ControllerKey value)
        {
            ControllerKey state = GetAllKeys() & value;
            return state == value;
        }
    }
}
