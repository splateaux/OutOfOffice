using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.IO;
using SoftProGameWindows.Gameplay;
using Microsoft.Xna.Framework.Media;

namespace SoftProGameWindows
{
    /// <summary>
    /// A gameplay arena with collections of dollars and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    public class Level
    {
        private static readonly Vector2 _uninitializedVector = new Vector2(-1, -1);

        // Physical structure of the level.
        private List<Layer> _layers;
        private Player _player;
        private List<Sprite> _sprites = new List<Sprite>();

        // Key locations in the level.
        private Vector2 _start = _uninitializedVector;
        private Vector2 _exit = _uninitializedVector;
        private float _cameraPosition;

        // Level game state.
        private Random _random = new Random(354668); // Arbitrary, but constant seed
        private TimeSpan _allowableTime;
        private ScreenManager _screenManager;
        private ContentManager _content;

        // Level information.
        private LevelInfo _levelInfo;
        private int _entityLayer;
        private float _viewMargin;
        private int _scale;
        private int _width;
        private int _height;

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="screenManager">The screen manager.</param>
        /// <param name="info">The level information.</param>
        public Level(ScreenManager screenManager, LevelInfo info)
        {
            // Initialize the level information
            this._screenManager = screenManager;
            this._levelInfo = info;
            this._entityLayer = info.EntityLayer;
            this._viewMargin = info.XAxisViewMargin;
            this._scale = info.XAxisScale;
            this._allowableTime = TimeSpan.FromSeconds(info.AllowableTimeInSeconds);
        }
        
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public void LoadContent()
        {
            if (this._content == null)
            {
                // Create a new content manager to load content used just by this level.
                this._content = new ContentManager(this._screenManager, "Content");
            }

            // Load up the background layers
            this._layers = new List<Layer>(this._levelInfo.Layers.Count);
            foreach (var layer in this._levelInfo.Layers)
            {
                this._layers.Add(new Layer(this._content, layer));
            }

            // Load up the level layout
            this.LoadLayouts(this._levelInfo.Layouts);

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(this._content.Load<Song>(this._levelInfo.SoundTrack));
            }
            catch { }
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public void UnloadContent()
        {
            MediaPlayer.Stop();

            this._sprites.ForEach((sprite) => sprite.UnloadContent());

            if (this._content != null)
            {
                this._content.Unload();
            }
        }

        /// <summary>
        /// Iterates over every sprite in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="layouts">The layouts.</param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.NotSupportedException">A level must have a starting point.
        /// or
        /// A level must have an exit.</exception>
        private void LoadLayouts(List<string> layouts)
        {
            // Load the level and ensure all of the lines are the same length.
            this._width = 0;
            this._height = 0;

            // Process each of the layouts
            foreach (var layout in layouts)
            {
                using (var fs = TitleContainer.OpenStream(layout + ".txt"))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line = reader.ReadLine();
                    var width = line.Length;
                    var rowIndex = 0;

                    do
                    {
                        // Check to see if the width of the line is consistent with the other rows
                        if (line.Length != width)
                        {
                            throw new Exception(string.Format("The length of line {0} is different from all preceeding lines.", rowIndex));
                        }

                        // Loop through each character in the line
                        for (int colIndex = 0; colIndex < width; ++colIndex)
                        {
                            // To load each sprite.
                            char spriteType = line[colIndex];
                            var sprite = this.LoadSprite(spriteType, new Vector2((colIndex + this._width) * this._scale, rowIndex * this._scale));

                            // Check to see if a sprite was loaded
                            if (sprite != null)
                            {
                                sprite.LoadContent(this._content);
                                this._sprites.Add(sprite);
                            }
                        }

                        // Read the next line
                        line = reader.ReadLine();
                        rowIndex++;

                    } while (line != null);

                    // Adjust the total height and width of the level
                    this._width += (width * this._scale);
                    this._height = Math.Max(this._height, (rowIndex * this._scale));
                }
            }

            // Verify that the level has a beginning and an end.
            if (this._start == _uninitializedVector)
                throw new NotSupportedException("A level must have a starting point.");

            if (this._exit == _uninitializedVector)
                throw new NotSupportedException("A level must have an exit.");
        }

        /// <summary>
        /// Loads an individual sprite's appearance and behavior.
        /// </summary>
        /// <param name="spriteType">The character loaded from the structure file which indicates what should be loaded.</param>
        /// <param name="position">The position of the sprite on the level.</param>
        /// <returns>
        /// The loaded sprite.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private Sprite LoadSprite(char spriteType, Vector2 position)
        {
            switch (spriteType)
            {
                // Placeholder
                case '.':
                    return null;

                // Player Start Position
                case '1':
                    this._start = position;
                    return null;

                // Exit
                case 'X':
                    return this.LoadExitMarker(position); // Each level may have only one of these

                // SoftPro Dollar
                case '$':
                    return this.LoadSoftProDollar(position);

                // Table
                case '-':
                    return this.LoadCollidableObject(position, ICollidableType.Platform, "table");

                // Printer
                case 'P':
                    return this.LoadCollidableObject(position, ICollidableType.Platform, "laser_printer");

                // Various enemies
                case 'A':
                    return this.LoadEnemy(position, "BugA");

                case 'B':
                    return this.LoadEnemy(position, "BugB");

                case 'C':
                    return this.LoadEnemy(position, "BugC");

                case 'D':
                    return this.LoadEnemy(position, "MonsterD");

                // Platform block
                case '~':
                    return this.LoadCollidableObject(position, ICollidableType.Platform, "BlockB", 2);

                // Passable block
                case ':':
                    return this.LoadCollidableObject(position, ICollidableType.Passable, "BlockB", 2);

                // Impassable block
                case '#':
                    return this.LoadCollidableObject(position, ICollidableType.Impassable, "BlockA", 7);

                // Unknown sprite type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported sprite type character '{0}' at position {1}, {2}.", spriteType, position.X, position.Y));
            }
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The sprite that represents the exit location.
        /// </returns>
        /// <exception cref="System.NotSupportedException">A level may only have one exit.</exception>
        private Sprite LoadExitMarker(Vector2 position)
        {
            if (this._exit != _uninitializedVector)
                throw new NotSupportedException("A level may only have one exit.");

            this._exit = position;
            return new Exit(position, this._content.Load<Texture2D>("Textures/Objects/Exit"));
        }

        /// <summary>
        /// Loads a collidable sprite object.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The sprite that represents the SoftPro dollar.
        /// </returns>
        private Sprite LoadSoftProDollar(Vector2 position)
        {
            return new SoftProDollar(position);
        }

        /// <summary>
        /// Loads a sprite object.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="variations">The number of variations.  If specified, a random variation of the object will be created.</param>
        /// <returns>
        /// The sprite that represents the object.
        /// </returns>
        private Sprite LoadObject(Vector2 position, string objectName, int? variations = null)
        {
            if (variations.HasValue)
            {
                var index = this._random.Next(variations.Value);
                return new Sprite(position, this._content.Load<Texture2D>("Textures/Objects/" + objectName + index));
            }
            else
            {
                return new Sprite(position, this._content.Load<Texture2D>("Textures/Objects/" + objectName));
            }
        }

        /// <summary>
        /// Loads a collidable sprite object.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collidableType">Type of the collidable.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="variations">The number of variations.  If specified, a random variation of the object will be created.</param>
        /// <returns>
        /// The sprite that represents the collidable object.
        /// </returns>
        private Sprite LoadCollidableObject(Vector2 position, ICollidableType collidableType, string objectName, int? variations = null)
        {
            if (variations.HasValue)
            {
                var index = this._random.Next(variations.Value);
                return new CollidableSprite(position, collidableType, this._content.Load<Texture2D>("Textures/Objects/" + objectName + index));
            }
            else
            {
                return new CollidableSprite(position, collidableType, this._content.Load<Texture2D>("Textures/Objects/" + objectName));
            }
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="enemyName">Name of the enemy.</param>
        /// <returns></returns>
        private Sprite LoadEnemy(Vector2 position, string enemyName)
        {
            return new Enemy(position, enemyName);
        }

        /// <summary>
        /// Width of level measured in pixels.
        /// </summary>
        public int Width
        {
            get { return this._width; }
        }

        /// <summary>
        /// Height of the level measured in pixels.
        /// </summary>
        public int Height
        {
            get { return this._height; }
        }

        /// <summary>
        /// Scrolls the camera.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        private void ScrollCamera(Viewport viewport)
        {
            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * this._levelInfo.XAxisViewMargin;
            float marginLeft = this._cameraPosition + marginWidth;
            float marginRight = this._cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (this._player.Position.X < marginLeft)
                cameraMovement = this._player.Position.X - marginLeft;
            else if (this._player.Position.X > marginRight)
                cameraMovement = this._player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = this.Width - viewport.Width;
            this._cameraPosition = MathHelper.Clamp(this._cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime, InputState input)
        {
            if (this._player != null)
            {
                // Pause while the player is dead or time is expired.
                if (!this._player.IsAlive || (this._player.TimeToLive == TimeSpan.Zero))
                {
                    // Remove the player from the level
                    this._player = null;
                }
                else
                {
                    // Check for collisions
                    CollisionManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    // Perform player and sprite updates
                    this._player.Update(gameTime, input);
                    this._sprites.ForEach((sprite) => sprite.Update(gameTime, input));
                }
            }
        }

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this._player != null)
            {
                // Draw all the background layers
                spriteBatch.Begin();

                for (int i = 0; i <= this._levelInfo.EntityLayer; ++i)
                {
                    this._layers[i].Draw(spriteBatch, this._cameraPosition);
                }

                spriteBatch.End();

                // Calculate the current camera view port
                this.ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
                Matrix cameraTransform = Matrix.CreateTranslation(-this._cameraPosition, 0.0f, 0.0f);

                // Draw the player and all of the sprites
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
                
                this._player.Draw(gameTime, spriteBatch);
                if (this._screenManager.DebugEnabled)
                {
                    DrawingUtils.DrawRectangle(spriteBatch, this._player.BoundingBox, this._screenManager.TransparentTexture, Color.White, false, 1);
                }
                
                this._sprites.ForEach((sprite) =>
                {
                    sprite.Draw(gameTime, spriteBatch);
                    if (this._screenManager.DebugEnabled)
                    {
                        DrawingUtils.DrawRectangle(spriteBatch, sprite.BoundingBox, this._screenManager.TransparentTexture, Color.White, false, 1);
                    }
                });

                spriteBatch.End();

                // Draw all the foreground layers
                spriteBatch.Begin();

                for (int i = this._levelInfo.EntityLayer + 1; i < this._layers.Count; ++i)
                {
                    this._layers[i].Draw(spriteBatch, this._cameraPosition);
                }

                spriteBatch.End();
            }
        }

        /// <summary>
        /// Spawns the player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void SpawnPlayer(Player player)
        {
            this._player = player;
            this._player.Reset(this._start, this._allowableTime);
        }
    }
}