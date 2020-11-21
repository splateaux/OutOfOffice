using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace SoftProGameWindows
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        // For debugging
        public static bool Debugging;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan _warningTime = TimeSpan.FromSeconds(30);

        // Global content.
        private ContentManager _content;

        // Meta-level game state.
        private LevelConfiguration _levelConfig;
        private SideScrollingLevelScreen _level;
        private int _levelIndex;
        private string _characterName;
        private int _maxTimeAllowed;

        // Player info
        private Player _player;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(string characterName = "Player1")
        {
            this._characterName = characterName;
            this.TransitionOnTime = TimeSpan.FromSeconds(1.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (this._content == null)
            {
                this._content = new ContentManager(this.ScreenManager.Game.Services, "Content");
            }

            // Load the level configuration
            using (var fs = TitleContainer.OpenStream(@"Content/Data/LevelConfiguration.xml"))
            {
                var serializer = new XmlSerializer(typeof(LevelConfiguration));
                this._levelConfig = (LevelConfiguration)serializer.Deserialize(fs);
            }

            // Load the player
            this._player = new Player(this, Vector2.Zero, this._characterName);
            this._player.LoadContent(this._content);

            // Set the level index
            this._levelIndex = 0;
            this._maxTimeAllowed = this._levelConfig.Levels[this._levelIndex].AllowableTimeInSeconds;
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            if (this._content != null)
            {
                this._content.Unload();
            }
        }

        /// <summary>
        /// Called when update occurs.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            if (this.IsActive && (this._player != null))
            {
                if (this._level == null)
                {
                    // Reload the current level
                    this.ReloadCurrentLevel();

                    // Re-spawn the player on the level
                    this._level.SpawnPlayer(this._player);
                }
                else if (this._player.ReachedExit)
                {
                    // Go on to the next level
                    this.LoadNextLevel();

                    if (this._level != null)
                    {
                        // Spawn the player on the level
                        this._level.SpawnPlayer(this._player);
                    }
                }
                else if (!this._player.IsAlive)
                {
                    // Load the loser screen!
                    LoadingScreen.Load(this.ScreenManager, false, null, new LoserScreen(this._characterName, this._player.Score));
                }
            }
        }

        /// <summary>
        /// Loads the next level.
        /// </summary>
        private void LoadNextLevel()
        {
            // Increment the level index
            this._levelIndex++;
            this._level = null;

            if (this._levelIndex < this._levelConfig.Levels.Count)
            {
                this._maxTimeAllowed += this._levelConfig.Levels[this._levelIndex].AllowableTimeInSeconds;

                // Load the next level
                this._level = new SideScrollingLevelScreen(this._levelConfig.Levels[this._levelIndex]);
                this.ScreenManager.AddScreen(this._level, null);
            }
            else
            {
                // Load the winner screen!
                LoadingScreen.Load(this.ScreenManager, false, null, new WinnerScreen(this._characterName, this._player.Score, this._player.TimeToLive, this._maxTimeAllowed));
            }
        }

        /// <summary>
        /// Reloads the current level.
        /// </summary>
        private void ReloadCurrentLevel()
        {
            --this._levelIndex;
            this.LoadNextLevel();
        }
    }
}