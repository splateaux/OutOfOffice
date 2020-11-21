using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen : IServiceProvider
    {
        private bool _isPopup = false;
        private TimeSpan _transitionOnTime = TimeSpan.Zero;
        private TimeSpan _transitionOffTime = TimeSpan.Zero;
        private float _transitionPosition = 1;
        private ScreenState _screenState = ScreenState.TransitionOn;
        private bool _isExiting = false;
        private bool _otherScreenHasFocus;
        private ScreenManager _screenManager;
        private PlayerIndex? _controllingPlayer;
        private GestureType _enabledGestures = GestureType.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameScreen"/> class.
        /// </summary>
        public GameScreen()
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return this._isPopup; }
            protected set { this._isPopup = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return this._transitionOnTime; }
            protected set { this._transitionOnTime = value; }
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return this._transitionOffTime; }
            protected set { this._transitionOffTime = value; }
        }

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return this._transitionPosition; }
            protected set { this._transitionPosition = value; }
        }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return this._screenState; }
            protected set { this._screenState = value; }
        }

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return this._isExiting; }
            protected internal set { this._isExiting = value; }
        }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !this._otherScreenHasFocus &&
                       (this._screenState == ScreenState.TransitionOn ||
                        this._screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return this._screenManager; }
            internal set { this._screenManager = value; }
        }

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent screens, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return this._controllingPlayer; }
            internal set { this._controllingPlayer = value; }
        }

        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return this._enabledGestures; }
            protected set
            {
                this._enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourself.
                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        /// <param name="otherScreenHasFocus">if set to <c>true</c> [other screen has focus].</param>
        /// <param name="coveredByOtherScreen">if set to <c>true</c> [covered by other screen].</param>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this._otherScreenHasFocus = otherScreenHasFocus;

            if (this._isExiting)
            {
                // If the screen is going away to die, it should transition off.
                this._screenState = ScreenState.TransitionOff;

                if (!this.UpdateTransition(gameTime, this._transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    this.ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (this.UpdateTransition(gameTime, this._transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    this._screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    this._screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (this.UpdateTransition(gameTime, this._transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    this._screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    this._screenState = ScreenState.Active;
                }
            }

            if (this.IsActive) this.OnUpdate(gameTime);
        }

        protected virtual void OnUpdate(GameTime gametime)
        {
        
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to UpdateTransition.</param>
        /// <param name="time">The time.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            this._transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (this._transitionPosition <= 0)) ||
                ((direction > 0) && (this._transitionPosition >= 1)))
            {
                this._transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        /// <param name="input">The input.</param>
        public virtual void HandleInput(InputState input)
        {
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public virtual void Draw(GameTime gameTime)
        {
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                this.ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                this._isExiting = true;
            }
        }

        #region IServiceProvider Implementation

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            if (this.ScreenManager != null)
            {
                return this.ScreenManager.GetService(serviceType);
            }

            return null;
        }

        #endregion
    }
}