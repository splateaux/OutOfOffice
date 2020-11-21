using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MazeGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace SoftProGameWindows
{
    /// <summary>
    /// The aMAZEing game!
    /// </summary>
    class MazeGameScreen : MiniGameScreen
    {
        private ContentManager _content;
        private Maze _maze;
        private MazeRunner _runner;
        private MazeBoss _boss;

        private Texture2D _bossTexture;
        private Texture2D _conferenceRoomTexture;
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
                introMessage.AppendLine("Get to the end of maze before Joyce!.");
                introMessage.AppendLine("That might sound easy, but you've only got one minute!");
                introMessage.AppendLine("If you think you're good enough, try to get some SoftPro dollars on the way.");
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

            MazeOptions options = new MazeOptions(
                new Vector2(2, 3),
                28,
                13,
                Content.Load<Texture2D>(Constants.SOFTPRODOLLAR_TEXTURE));

            _maze = new Maze(this.ScreenManager.GraphicsDevice, options);

            _bossTexture = Content.Load<Texture2D>("Textures/Objects/MazeGame/Boss");
            _conferenceRoomTexture = Content.Load<Texture2D>("Textures/Objects/MazeGame/ConferenceRoom");

            base.LoadContent();
        }

        /// <summary>
        /// An "update" just for minigames.
        /// Only gets called if your mini game SHOULD be updating,
        /// because it's not paused, the intro is gone, etc..
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void MiniGameUpdate(GameTime gametime)
        {
            // Several things we can't load until PLayer is available...
            if (!_contentFinishedLoading)
            {
                FinishLoadingObjects();
            }

            if (_runner.EndReached)
            {
                OnReachedEnd();
            }

            if (_boss.IsAtDestination)
            {
                OnBossGotThereFirst();
            }

            _runner.Update(gametime);
            this.Player.AddToScore(_runner.Points);
            this._runner.Points = 0;
            this._boss.Update(gametime);
            this.Hud.Message = "Get to the meeting before Joyce!";
            
        }

        /// <summary>
        /// Called when a random sound should be played
        /// </summary>
        /// <param name="manager"></param>
        protected override void OnPlayRandomSound(SoundEffectManager manager)
        {
            manager.PlayMaze();
        }

        /// <summary>
        /// Finishes the loading objects we couldn't load until Player was loaded
        /// </summary>
        private void FinishLoadingObjects()
        {
            _boss = new MazeBoss(
                _bossTexture,
                new Vector2(0, 0),
                new Vector2(1100, 0),
                Player.TimeToLive < new TimeSpan(0, 1, 0) ? Player.TimeToLive : new TimeSpan(0, 1, 0),
                _conferenceRoomTexture);

            MazeRunnerOptions runnerOptions = new MazeRunnerOptions(
                _maze,
                Content.Load<Texture2D>(String.Format("Textures/Players/{0}/CloseupSmall", Player.CharacterName)),
                this._content.Load<SoundEffect>(Constants.SOFTPRODOLLAR_SOUND),
                this.ScreenManager.PointManager.GetValue(Constants.SOFTPRO_DOLLAR_VALUE));

            _runner = new MazeRunner(this.ScreenManager.Game, runnerOptions);

            _contentFinishedLoading = true;
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

                this.ScreenManager.SpriteBatch.Begin();
                _maze.Draw(this.ScreenManager.SpriteBatch);
                _runner.Draw(this.ScreenManager.SpriteBatch);
                _boss.Draw(this.ScreenManager.SpriteBatch);
                this.ScreenManager.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Called when [reached end].
        /// </summary>
        private void OnReachedEnd()
        {
            this.Player.AddToScore(this.ScreenManager.PointManager.GetValue(Constants.MAZE_GAME_COMPLETE));
            this.ScreenManager.RemoveScreen(this);
        }

        /// <summary>
        /// Called when [boss got there first].
        /// </summary>
        private void OnBossGotThereFirst()
        {
            this.ScreenManager.RemoveScreen(this);
        }
    }
}
