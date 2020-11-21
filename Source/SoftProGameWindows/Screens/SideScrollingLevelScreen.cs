using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents a side-scrolling level screen.
    /// </summary>
    public class SideScrollingLevelScreen : LevelScreen
    {
        // Level game state.
        private static readonly Vector2 _uninitializedVector = new Vector2(-1, -1);
        private Random _random = new Random(354668); // Arbitrary, but constant seed

        private TimeSpan _randomSoundInterval = new TimeSpan(0, 0, 20);
        private TimeSpan _randomSoundLastPlayed;

        // Level information.
        private LevelInfo _info;
        private List<Layer> _layers;
        private Layer _playerLayer;
        private Vector2 _playerStartPosition;
        private Camera _camera;
        private IDisposable _collisionManagerScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="SideScrollingLevelScreen"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        public SideScrollingLevelScreen(LevelInfo info)
            : base()
        {
            this._info = info;
            this._playerStartPosition = _uninitializedVector;
            this._randomSoundLastPlayed = new TimeSpan(0);
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            // Create a new camera
            this._camera = new Camera(this.ScreenManager.GraphicsDevice.Viewport);
            this._camera.Limits = new Rectangle(0, 0, this._info.Width, this._info.Height);

            // Suppress collision management until the layers are loaded
            using (var scope = CollisionManager.Suppress())
            {
                // Load up the layers
                this.LoadLayers(this._info.Layers);
            }

            // Initialize the player start information
            this._playerLayer = this._layers[this._info.PlayerLevelIndex];
            this._playerStartPosition = this._info.PlayerStartPosition;

            // Start a new collision manager scope
            this._collisionManagerScope = CollisionManager.NewScope(this._camera.Limits.Value, this._playerLayer);

            // Register the collidable sprites with the collision manager
            foreach (var sprite in this.Sprites.Where(s => s is ICollidable).Cast<ICollidable>())
            {
                CollisionManager.Collection.Insert(sprite);
            }

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(this.Content.Load<Song>(this._info.SoundTrack));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Loads the layers.
        /// </summary>
        /// <param name="list">The list.</param>
        private void LoadLayers(List<LayerInfo> list)
        {
            // Build the layers
            this._layers = new List<Layer>(list.Count);
            foreach (var layerInfo in this._info.Layers)
            {
                var layer = new Layer(this._camera) { Parallax = new Vector2(layerInfo.ScrollRate, 1.0f) };
                var position = Vector2.Zero;

                // Load each sprite
                foreach (var info in layerInfo.Sprites)
                {
                    var sprite = SpriteFactory.Create(info.SpriteType, info);
                    sprite.SetSite(this);
                    sprite.LoadContent(this.Content);
                    layer.AddSprite(sprite);
                    this.Sprites.Add(sprite);
                }

                this._layers.Add(layer);
            }
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            MediaPlayer.Stop();
            this._collisionManagerScope.Dispose();
            base.UnloadContent();
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            // Ensure that the current screen is active and not exiting
            if (!this.IsExiting)
            {
                // Check to see if the player has been spawned
                if (this.Player != null)
                {
                    if (!this.Player.IsAlive || this.Player.ReachedExit)
                    {
                        // The player is dead so exit the level
                        this.ExitScreen();
                    }
                    else
                    {
                        // Perform player and sprite updates
                        foreach (var layer in this._layers)
                        {
                            layer.Update(gametime);
                        }

                        // Check for collisions
                        CollisionManager.Update(gametime);

                        PlayRandomSound(gametime);

                        // Update the camera position based on the player
                        this._camera.LookAt(this.Player.Position);

                        ///////////////////////////////////////////////////////////////////////////////////////
                        // REMOVED BECAUSE THESE ARE OVERWRITING ANY REAL MESSAGE YOU WANT TO WRITE TO THE HUD
                        ///////////////////////////////////////////////////////////////////////////////////////
                        //// Send random messages to the HUD
                        //if (this.Player.TimeToLive < new TimeSpan(0, 1, 30))
                        //{
                        //    this.Hud.Message = "Amy is a goober!";
                        //}
                        //else if (this.Player.TimeToLive < new TimeSpan(0, 1, 50))
                        //{
                        //    this.Hud.Message = "Morton Rules!";
                        //}
                        //else
                        //{
                        //    this.Hud.Message = "Go get to the head shaving event!";
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// Plays a random sound.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        private void PlayRandomSound(GameTime gametime)
        {
            _randomSoundLastPlayed += gametime.ElapsedGameTime;

            if (_randomSoundLastPlayed > _randomSoundInterval)
            {
                this.GetService<SoundEffectManager>().PlayMain();
                _randomSoundLastPlayed = new TimeSpan(0);
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

            // Determine if the game should be paused
            PlayerIndex index;
            if (input.IsPauseGame(ControllingPlayer, out index))
            {
                this.ScreenManager.AddScreen(new PauseMenuScreen(), this.ControllingPlayer);
            }
            else if (this.Player != null)
            {
                this.Player.HandleInput(input);
            }
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            // Ensure that the current screen is active and not exiting
            if (!this.IsExiting)
            {
                // Check to see if the player has been spawned
                if (this.Player != null)
                {
                    foreach (var layer in this._layers)
                    {
                        layer.Draw(gameTime, this.ScreenManager.SpriteBatch);
                    }

                    base.Draw(gameTime);
                }
            }
        }

        /// <summary>
        /// Spawns the player.
        /// </summary>
        /// <param name="player">The player.</param>
        public override void SpawnPlayer(Player player)
        {
            if (this.Player != null)
            {
                this._playerLayer.RemoveSprite(this.Player);
                CollisionManager.Collection.Remove(this.Player);
            }

            base.SpawnPlayer(player);

            player.Reset(this._playerStartPosition, TimeSpan.FromSeconds(this._info.AllowableTimeInSeconds));
            player.SetSite(this);
            this._playerLayer.AddSprite(player);
            this._camera.LookAt(this._playerStartPosition);
            CollisionManager.Collection.Insert(player);
        }
    }
}
