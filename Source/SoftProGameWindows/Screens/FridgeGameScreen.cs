using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FridgeGame;
using InputController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SoftProGameWindows
{
    /// <summary>
    /// The Stock the Fridge game!
    /// </summary>
    class FridgeGameScreen : MiniGameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private Breakroom _breakroom;
        private NintendoController _nintendoController;
        private NintendoControllerState _controllerState;
        private KeyboardState _keyboardState;
        private QuizTimer _miniGameTimer;
        private bool _contentFinishedLoading;


        /// <summary>
        /// Give me an intro screen message!
        /// </summary>
        /// <value>
        /// The intro message.
        /// </value>
        protected override string IntroMessage
        {
            get
            {
                StringBuilder introMessage = new StringBuilder();
                introMessage.AppendLine("Oh no, the fridge is empty!");
                introMessage.AppendLine("You've got one minute to stock as many cans as possible.");
                introMessage.AppendLine("Make sure to sort them correctly.");
                introMessage.AppendLine();
                introMessage.AppendLine("Too scared?  You can cancel to try and make it to the shave event.");
                return introMessage.ToString();
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        { 
            this._content = new ContentManager(this.ScreenManager.Game.Services, "Content");
            this._spriteBatch = this.ScreenManager.SpriteBatch;
            this._nintendoController = new NintendoController(this.ScreenManager.Game);
            
            _miniGameTimer = new QuizTimer(new TimeSpan(0, 1, 0));
            _miniGameTimer.LoadContent(ScreenManager.Font, Content, ScreenManager.GraphicsDevice);
            _miniGameTimer.TimeLocation = new Vector2(30, -5);

            base.LoadContent();
        }

        /// <summary>
        /// An "update" just for minigames.
        /// Only gets called if your mini game SHOULD be updating,
        /// because it's not paused, the intro is gone, etc..
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void MiniGameUpdate(Microsoft.Xna.Framework.GameTime gametime)
        {
            //
            // Several things we can't load until PLayer is available...
            if (!_contentFinishedLoading)
            {
                FinishLoadingObjects();
            }

            UpdateTimer(gametime);

            if (_miniGameTimer.TimeIsUp)
            {
                ExitScreen();
            }

            HandleInput();
            this.Player.AddToScore(_breakroom.Score);
            _breakroom.Score = 0;
            _breakroom.Update(gametime, _controllerState, _keyboardState);

            this.Hud.Message = "Stock the Fridge!";
        }

        /// <summary>
        /// Finishes the loading objects we couldn't load until Player was loaded
        /// </summary>
        private void FinishLoadingObjects()
        {
            _breakroom = new Breakroom(this.ScreenManager.Game.Services, this.ScreenManager.GraphicsDevice, this.Player.CharacterName); 

            _contentFinishedLoading = true;
        }

        /// <summary>
        /// Called when a random sound should be played
        /// </summary>
        /// <param name="manager"></param>
        protected override void OnPlayRandomSound(SoundEffectManager manager)
        {
            manager.PlayFridge();
        }


        /// <summary>
        /// Updates the timer.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        private void UpdateTimer(GameTime gametime)
        {
            if (_miniGameTimer.QuizTime > Player.TimeToLive)
            {
                _miniGameTimer.QuizTime = Player.TimeToLive - gametime.ElapsedGameTime;
            }

            _miniGameTimer.Update(gametime);
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_contentFinishedLoading)
            {
                this.ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

                _breakroom.Draw(gameTime, _spriteBatch);

                _miniGameTimer.Draw(ScreenManager.SpriteBatch);
            }

            base.Draw(gameTime);
        }

        private void HandleInput()
        {
            _controllerState = _nintendoController.GetState();
            _keyboardState = Keyboard.GetState();
        }

    }
}
