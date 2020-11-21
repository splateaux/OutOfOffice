using InputController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class
    /// tracks both the current and previous state of the input devices, and implements
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public const int MaxInputs = 4;
        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        public readonly bool[] GamePadWasConnected;

        private NintendoController _nintendoController;
        public NintendoControllerState CurrentNintendoControllerState;
        public NintendoControllerState LastNintendoControllerState;
        public bool NintendoControllerWasConnected;

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        /// <param name="game">The game.</param>
        public InputState(Game game)
        {
            this.CurrentKeyboardStates = new KeyboardState[MaxInputs];
            this.CurrentGamePadStates = new GamePadState[MaxInputs];

            this.LastKeyboardStates = new KeyboardState[MaxInputs];
            this.LastGamePadStates = new GamePadState[MaxInputs];

            this.GamePadWasConnected = new bool[MaxInputs];

            this._nintendoController = new NintendoController(game);
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            // Read all input state of the keyboard and support game pad controllers
            for (int i = 0; i < MaxInputs; i++)
            {
                this.LastKeyboardStates[i] = this.CurrentKeyboardStates[i];
                this.LastGamePadStates[i] = this.CurrentGamePadStates[i];

                this.CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                this.CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (this.CurrentGamePadStates[i].IsConnected)
                {
                    this.GamePadWasConnected[i] = true;
                }
            }

            // Add the capability to read data from the Nintendo Controller
            this.LastNintendoControllerState = this.CurrentNintendoControllerState;
            this.CurrentNintendoControllerState = this._nintendoController.GetState();
            if (this.CurrentNintendoControllerState.IsConnected)
            {
                this.NintendoControllerWasConnected = true;
            }
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. This method
        /// will only return true if the key was not pressed during the last update.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which pressed the key.</param>
        /// <returns>
        ///   <c>true</c> if the key has been pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (this.CurrentKeyboardStates[i].IsKeyDown(key) &&
                        this.LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (this.IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        this.IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        this.IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        this.IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update. This method
        /// will only return true if the button was not pressed during the last update.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which pressed the button.</param>
        /// <returns>
        ///   <c>true</c> if the button has been pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (
                    (this.CurrentGamePadStates[i].IsButtonDown(button) && this.LastGamePadStates[i].IsButtonUp(button)) ||
                    (this.CurrentNintendoControllerState.IsButtonDown(button) && this.LastNintendoControllerState.IsButtonUp(button))
                );
            }
            else
            {
                // Accept input from any player.
                return (this.IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        this.IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Helper for checking if a key is currently being pressed during this update.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the key is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return this.CurrentKeyboardStates[i].IsKeyDown(key);
            }
            else
            {
                // Accept input from any player.
                return (this.IsKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        this.IsKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        this.IsKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        this.IsKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Helper for checking if a button is currently being pressed during this update.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the button.</param>
        /// <returns>
        ///   <c>true</c> if the button is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return this.CurrentGamePadStates[i].IsButtonDown(button) ||
                       this.CurrentNintendoControllerState.IsButtonDown(button);
            }
            else
            {
                // Accept input from any player.
                return (this.IsButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        this.IsButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        this.IsButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        this.IsButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "menu select" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   this.IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "menu cancel" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "menu up" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsMenuUp(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "menu down" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsMenuDown(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "pause the game" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsPauseGame(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "primary" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "primary" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsPrimaryPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "secondary" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "secondary" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsSecondaryPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "up" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "up" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsUpPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "down" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "down" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsDownPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "left" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "left" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsLeftPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a newly intiated "right" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the newly intiated "right" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsRightPressed(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex) ||
                   this.IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "primary" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "primary" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsPrimary(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.A, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "secondary" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "secondary" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsSecondary(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.B, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "up" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "up" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsUp(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "down" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "down" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsDown(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "left" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "left" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsLeft(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "right" input action.
        /// </summary>
        /// <param name="controllingPlayer">Specifies which player to read input from (null = any player).</param>
        /// <param name="playerIndex">Index of the player which is pressing the key.</param>
        /// <returns>
        ///   <c>true</c> if the "right" input is being pressed; <c>false</c> otherwise
        /// </returns>
        public bool IsRight(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return this.IsKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex) ||
                   this.IsButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex);
        }
    }
}