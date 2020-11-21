using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// A baseclass for minigames
    /// It does a lot of the little things for you like
    /// Providing a pause screen, intro screen, updating the player, and letting you exit the game
    /// </summary>
    abstract class MiniGameScreen : LevelScreen
    {
        private MessageBoxScreen _intro;
        private BackgroundScreen _back;
        private bool _pauseSelected;

        private TimeSpan _randomSoundInterval = new TimeSpan(0, 0, 20);
        private TimeSpan _randomSoundLastPlayed;

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            _back = new BackgroundScreen("Textures/Backgrounds/GradientBackground");

            _intro = new MessageBoxScreen(this.IntroMessage);
            _intro.Accepted += screen_Accepted;
            _intro.Cancelled += screen_Cancelled;

            base.LoadContent();
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected sealed override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            if (_intro != null)
            {
                ScreenManager.AddScreen(_back, null);
                ScreenManager.AddScreen(_intro, null);
            }
            else
            {
                if (_pauseSelected)
                {
                    MiniGamePauseScreen pause = new MiniGamePauseScreen(this);
                    pause.QuitSelected += pause_QuitSelected;
                    this.ScreenManager.AddScreen(pause, this.ControllingPlayer);
                }
                else
                {
                    base.Player.UpdateTimeToLive(gametime);
                    this.PlayRandomSound(gametime);
                    MiniGameUpdate(gametime);
                }
            }
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        /// <param name="input">The input.</param>
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            PlayerIndex index;
            _pauseSelected = input.IsPauseGame(ControllingPlayer, out index);

        }

        /// <summary>
        /// An "update" just for minigames.  
        /// Only gets called if your mini game SHOULD be updating,
        /// because it's not paused, the intro is gone, etc..
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected abstract void MiniGameUpdate(GameTime gametime);

        /// <summary>
        /// Called when a random sound should be played
        /// </summary>
        protected abstract void OnPlayRandomSound(SoundEffectManager manager);

        /// <summary>
        /// Give me an intro screen message!
        /// </summary>
        /// <value>
        /// The intro message.
        /// </value>
        protected abstract string IntroMessage { get; }

        /// <summary>
        /// Plays a random sound.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        private void PlayRandomSound(GameTime gametime)
        {
            _randomSoundLastPlayed += gametime.ElapsedGameTime;

            if (_randomSoundLastPlayed > _randomSoundInterval)
            {
                this.OnPlayRandomSound(this.GetService<SoundEffectManager>());
                _randomSoundLastPlayed = new TimeSpan(0);
            }
        }

        /// <summary>
        /// Handles the QuitSelected event of the pause control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void pause_QuitSelected(object sender, EventArgs e)
        {
            this.ExitScreen();
        }

        /// <summary>
        /// Handles the Cancelled event of the screen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        void screen_Cancelled(object sender, PlayerIndexEventArgs e)
        {
            _back.ExitScreen();
            _intro.ExitScreen();
            ExitScreen();
        }

        /// <summary>
        /// Handles the Accepted event of the screen control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        void screen_Accepted(object sender, PlayerIndexEventArgs e)
        {
            _back.ExitScreen();
            _intro.ExitScreen();
            _intro = null;
        }

    }
}
