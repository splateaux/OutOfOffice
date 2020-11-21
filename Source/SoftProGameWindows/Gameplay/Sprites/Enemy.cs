using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace SoftProGameWindows
{
    /// <summary>
    /// Facing direction along the X axis.
    /// </summary>
    public enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// A monster who is impeding the progress of our fearless adventurer.
    /// </summary>
    public class Enemy : CollidableSprite, IMovingCollidable
    {
        private const float MAX_WAIT_TIME = 2.0f;
        private const float MOVE_SPEED = 64.0f;
        private const int DEFAULT_SECONDS_TO_RESPAWN = 10;
        private const int SECONDS_TO_SPAWN = 2;

        // Animations
        private Animation _runAnimation;
        private Animation _idleAnimation;
        private AnimationPlayer _animationPlayer;
        private TransitionHelper _transitionHelper;

        // Enemy info
        private Vector2 _basePosition;
        private FaceDirection _direction = FaceDirection.Left;
        private float _waitTime;
        private string _enemyName;
        private Vector2 _velocity;
        private SoundEffect _monsterKilledSound;
        private bool _isAlive;
        private Vector2 _nextPosition;
        private TimeSpan _timeToRespawn;
        private int _secondsToRespawn;
        private bool _spawning;
        private int _lastPercentDrawn;
        private bool _drawEnemy;

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="position">The position.</param>
        /// <param name="enemyName">The sprite set.</param>
        /// <param name="canRespawn">if set to <c>true</c> the enemy can respawn.</param>
        /// <param name="secondsToRespawn">The seconds to respawn after dying.</param>
        public Enemy(IServiceProvider site, Vector2 position, string enemyName, bool canRespawn = false, int secondsToRespawn = DEFAULT_SECONDS_TO_RESPAWN)
            : base(site, position, CollidableType.Impassable)
        {
            this._basePosition = position;
            this._nextPosition = position;
            this._enemyName = enemyName;
            this._isAlive = true;
            this.CanRespawn = canRespawn;
            this._secondsToRespawn = secondsToRespawn;
            this._transitionHelper = new TransitionHelper(TimeSpan.FromSeconds(SECONDS_TO_SPAWN), TransitionHelper.TransitionDirection.Up);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Enemy(SpriteInfo info)
            : base(info, CollidableType.Impassable)
        {
            var setting = info.Settings.FirstOrDefault(s => s.Key.Equals("EnemyName", StringComparison.OrdinalIgnoreCase));
            this._enemyName = setting.Value;

            setting = info.Settings.FirstOrDefault(s => s.Key.Equals("OnPatrol", StringComparison.OrdinalIgnoreCase));
            if ((setting != null) && !string.IsNullOrWhiteSpace(setting.Value))
            {
                this.IsOnPatrol = setting.Value.Equals("1") ||
                                  setting.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            setting = info.Settings.FirstOrDefault(s => s.Key.Equals("CanRespawn", StringComparison.OrdinalIgnoreCase));
            if ((setting != null) && !string.IsNullOrWhiteSpace(setting.Value))
            {
                this.CanRespawn = setting.Value.Equals("1") ||
                                  setting.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            setting = info.Settings.FirstOrDefault(s => s.Key.Equals("SecondsToRespawn", StringComparison.OrdinalIgnoreCase));
            if ((setting != null) && !string.IsNullOrWhiteSpace(setting.Value))
            {
                this._secondsToRespawn = int.Parse(setting.Value);
            }

            this._isAlive = true;
            this._basePosition = info.Position;
            this._nextPosition = info.Position;
            this._transitionHelper = new TransitionHelper(TimeSpan.FromSeconds(SECONDS_TO_SPAWN), TransitionHelper.TransitionDirection.Up);
        }

        /// <summary>
        /// Gets the rectangle that specifies (in texels) the source texels from a texture.
        /// </summary>
        /// <value>
        /// The source rectangle.
        /// </value>
        /// <remarks>
        /// Use null to draw the entire texture. This rectangle is aligned on the upper-left hand corner.
        /// </remarks>
        public override Rectangle? SourceRectangle
        {
            get
            {
                if (this._animationPlayer.CurrentFrame != null)
                {
                    return this._animationPlayer.CurrentFrame.SourceRectangle;
                }

                return null;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return this._isAlive; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is on patrol.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is on patrol; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnPatrol { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can respawn.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can respawn; otherwise, <c>false</c>.
        /// </value>
        public bool CanRespawn { get; set; }

        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <value>
        /// The next position.
        /// </value>
        public Vector2 NextPosition
        {
            get { return this._nextPosition; }
            private set { this._nextPosition = value; }
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content mananger.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            // Load animations.
            this._enemyName = "Textures/Enemies/" + this._enemyName + "/";
            this._runAnimation = new Animation(content.Load<Texture2D>(this._enemyName + "Run"), 0.1f, true);
            this._idleAnimation = new Animation(content.Load<Texture2D>(this._enemyName + "Idle"), 0.15f, true);
            this._animationPlayer.PlayAnimation(this._idleAnimation);

            // Load sounds.
            this._monsterKilledSound = content.Load<SoundEffect>("Audio/Effects/MonsterKilled");

            // Make the enemy alive!~
            this._isAlive = true;
        }

        /// <summary>
        /// Updates the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="input">The input.</param>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame.
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                // Set the enemy's current position and previous bounding box
                this.Position = this.NextPosition;

                // Get the current elapsed game time
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check to see if the we're waiting
                if (this._waitTime > 0)
                {
                    // Animate the player
                    this._animationPlayer.PlayAnimation(this._idleAnimation);

                    // Update the wait time based on the elapsed game time
                    this._waitTime = Math.Max(0.0f, this._waitTime - elapsed);

                    // Check to see if we've waited long enough
                    if (this._waitTime <= 0.0f)
                    {
                        // Update the direction of the enemy
                        this._direction = (FaceDirection)(-(int)this._direction);

                        // Clamp the wait time at zero
                        this._waitTime = 0.0f;
                    }
                }
                else if (this.Position.X < 0)
                {
                    // The enemy walked off the level so kill it off
                    this._isAlive = false;
                    CollisionManager.Collection.QueuePendingRemoval(this);

                    if (this.CanRespawn)
                    {
                        this._timeToRespawn = TimeSpan.FromSeconds(this._secondsToRespawn);
                    }
                }
                else
                {
                    // Animate the player
                    this._animationPlayer.PlayAnimation(this._runAnimation);

                    // Move in the current direction.
                    this._velocity = new Vector2((int)this._direction * MOVE_SPEED * elapsed, 0.0f);
                    this.NextPosition = this.NextPosition + this._velocity;
                }
            }
            else if (this._spawning)
            {
                if (this._transitionHelper.Update(gameTime) == TransitionHelper.TransitionState.Complete)
                {
                    this._spawning = false;
                    this._isAlive = true;
                    CollisionManager.Collection.Insert(this);
                }
                else
                {
                    var percentComplete = (float)Math.Pow(this._transitionHelper.Position, 2);
                    var percentTransitioned = (int)(percentComplete * 100) / 5;
                    if (percentTransitioned != this._lastPercentDrawn)
                    {
                        this._lastPercentDrawn = percentTransitioned;
                        this._drawEnemy = true;
                    }
                    else
                    {
                        this._drawEnemy = false;
                    }
                }
            }
            else if (this.CanRespawn)
            {
                // Calculate remaining time disable player input
                this._timeToRespawn -= gameTime.ElapsedGameTime;

                // Clamp the time to live at zero.
                if (this._timeToRespawn <= TimeSpan.Zero)
                {
                    this._timeToRespawn = TimeSpan.Zero;
                    this.Respawn();
                }
            }
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Save off the previous bounding box before the draw method advances the current frame
            this.PreviousBoundingBox = this.BoundingBox;

            if (this.IsActive || (this._spawning && this._drawEnemy))
            {
                var flip = this._direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                this._animationPlayer.Draw(gameTime, spriteBatch, this.Position, flip);

#if DEBUG
                // Draw debug outline
                if (ScreenManager.DEBUG_ENABLED)
                {
                    var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                    boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                    DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.Red);
                }
#endif
            }
        }

        /// <summary>
        /// Respawns the the enemy.
        /// </summary>
        public void Respawn()
        {
            if (!this.IsAlive && this.CanRespawn)
            {
                this.Position = this._basePosition;
                this._nextPosition = this._basePosition;
                this._spawning = true;
                this._lastPercentDrawn = 0;
                this._drawEnemy = false;
                this._transitionHelper.Reset();
            }
        }

        #region IMovingCollidable Implementation

        /// <summary>
        /// Gets the previous bounding box.
        /// </summary>
        /// <value>
        /// The previous bounding box.
        /// </value>
        public Rectangle PreviousBoundingBox { get; private set; }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        public Vector2 Velocity
        {
            get { return this._velocity; }
            private set { this._velocity = value; }
        }

        #endregion

        #region ICollidable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsActive
        {
            get { return this.IsAlive; }
        }

        /// <summary>
        /// Gets the bounding box of the sprite.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        /// <remarks>
        /// Used for collision detection. This rectangle is aligned on the upper-left hand corner.
        /// </remarks>
        public override Rectangle BoundingBox
        {
            get
            {
                var boundingBox = this._animationPlayer.CurrentFrame.BoundingBox;
                var screenPosition = (this.Layer != null) ? CollisionManager.ConvertWorldPosition(this.NextPosition, this.Layer) : this.NextPosition;
                var x = (int)Math.Round(boundingBox.X + screenPosition.X + this.Origin.X);
                var y = (int)Math.Round(boundingBox.Y + screenPosition.Y + this.Origin.Y);

                return new Rectangle(x, y, boundingBox.Width, boundingBox.Height);
            }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="info">The collision information.</param>
        public override void HandleCollisions(CollisionInfo info)
        {
            var player = info.Obstacle as Player;
            if ((player != null) && this._isAlive)
            {
                if (info.Direction.HasFlag(Direction.Top))
                {
                    this.GetService<SoundEffectManager>().PlayBugSquish();
                    this.GetService<SoundEffectManager>().PlayCrushBug();
                    this._isAlive = false;
                    CollisionManager.Collection.QueuePendingRemoval(this);

                    if (this.CanRespawn)
                    {
                        this._timeToRespawn = TimeSpan.FromSeconds(this._secondsToRespawn);
                    }
                }
            }
            else if (this.IsOnPatrol && (info.Obstacle is PatrolMarker))
            {
                this._waitTime = MAX_WAIT_TIME;
            }
        }

        #endregion
    }
}