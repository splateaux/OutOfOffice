using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftProGameWindows
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    public class Player : CollidableSprite, IMovingCollidable
    {
        // Constants for controling horizontal movement
        private const float MOVE_ACCELERATION = 9000.0f;
        private const float MAX_MOVE_SPEED = 512.0f;
        private const float MIN_MOVE_SPEED = 0.01f;
        private const float GROUND_DRAG_FACTOR = 0.70f;
        private const float AIR_DRAG_FACTOR = 0.70f;

        // Constants for controlling vertical movement
        private const float MAX_JUMP_TIME = 0.35f;
        private const float JUMP_LAUNCH_VELOCITY = -4000.0f;
        private const float GRAVITY_ACCELERATION = 3400.0f;
        private const float MAX_FALL_SPEED = 550.0f;
        private const float JUMP_CONTROL_POWER = 0.20f;

        // Input configuration
        private const float MOVE_STICK_SCALE = 1.0f;

        // Point scale
        private const int POINTS_PER_SECOND = 5;

        // Animations
        private Animation _idleAnimation;
        private Animation _idleBackAnimation;
        private Animation _runAnimation;
        private Animation _jumpAnimation;
        private Animation _celebrateAnimation;
        private Animation _dieAnimation;
        private SpriteEffects _flip = SpriteEffects.None;
        private AnimationPlayer _animationPlayer;

        // Sounds
        private SoundEffect _killedSound;
        private SoundEffect _jumpSound;
        private SoundEffect _fallSound;
        private SoundEffect _hurtSound;

        // Physics state
        private Vector2 _velocity;
        private bool _isOnGround;
        private float _movement;

        // Jumping state
        private bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;

        // Player info
        private PlayerIndex _playerIndex;
        private bool _isAlive;
        private bool _isActive;
        private bool _reachedExit;
        private string _characterName;
        private TimeSpan _timeToLive;
        private TimeSpan _timeOfInvincibility;
        private int _score;
        private Vector2 _nextPosition;
        private Door _door;
        private TimeSpan _playerControlDisabled;
        private bool _wasCollidingWithDoor;

        private HashSet<Door> _doorsGreetedJoyceAt;

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        /// <param name="site">The service provider.</param>
        /// <param name="position">The position.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="playerIndex">Index of the player.</param>
        public Player(IServiceProvider site, Vector2 position, string characterName = "Player1", PlayerIndex playerIndex = PlayerIndex.One)
            : base(site, position, CollidableType.Impassable)
        {
            this._characterName = characterName;
            this._playerIndex = playerIndex;
            this.Velocity = Vector2.Zero;
            this._nextPosition = position;
            this._playerControlDisabled = TimeSpan.Zero;
            this._doorsGreetedJoyceAt = new HashSet<Door>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Player(SpriteInfo info)
            : base(info, CollidableType.Impassable)
        {
            var setting = info.Settings.FirstOrDefault(s => s.Key.Equals("CharacterName", StringComparison.OrdinalIgnoreCase));
            this._characterName = string.IsNullOrWhiteSpace(setting.Value) ? "Player1" : setting.Value;

            setting = info.Settings.FirstOrDefault(s => s.Key.Equals("PlayerIndex", StringComparison.OrdinalIgnoreCase));
            var index = int.Parse(setting.Value);
            switch (index)
            {
                case 1: this._playerIndex = PlayerIndex.One; break;
                case 2: this._playerIndex = PlayerIndex.One; break;
                case 3: this._playerIndex = PlayerIndex.One; break;
                case 4: this._playerIndex = PlayerIndex.One; break;
            }

            this.Velocity = Vector2.Zero;
            this._nextPosition = info.Position;
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
        /// Gets the name of the character.
        /// </summary>
        /// <value>
        /// The name of the character.
        /// </value>
        public string CharacterName
        {
            get { return _characterName; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlive
        {
            get { return _isAlive; }
            private set { this._isAlive = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is invincible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is invincible; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvincible
        {
            get
            {
                return (this._timeOfInvincibility != TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Gets the time to live.
        /// </summary>
        /// <value>
        /// The time to live.
        /// </value>
        public TimeSpan TimeToLive
        {
            get { return this._timeToLive; }
            private set { this._timeToLive = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the player reached an exit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reached an exit; otherwise, <c>false</c>.
        /// </value>
        public bool ReachedExit
        {
            get { return this._reachedExit; }
            private set { this._reachedExit = value; }
        }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get { return this._score; }
            private set { this._score = value; }
        }

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

            // Load animated textures.
            this._idleAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/Idle"), 0.1f, true);
            this._idleBackAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/IdleBack"), 0.1f, true);
            this._runAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/Run"), 0.1f, true);
            this._jumpAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/Jump"), 0.1f, false);
            this._celebrateAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/Celebrate"), 0.1f, false);
            this._dieAnimation = new Animation(content.Load<Texture2D>("Textures/Players/" + this._characterName + "/Die"), 0.1f, false);
            this._animationPlayer.PlayAnimation(this._idleAnimation);

            // Load sounds.
            this._killedSound = content.Load<SoundEffect>("Audio/Effects/PlayerKilled");
            this._jumpSound = content.Load<SoundEffect>("Audio/Effects/MarioJump");
            this._fallSound = content.Load<SoundEffect>("Audio/Effects/PlayerFall");
            this._hurtSound = content.Load<SoundEffect>("Audio/Effects/Doh");

            // Make the player alive!~
            this.IsAlive = true;
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position, TimeSpan timeToLive)
        {
            this.Position = position;
            this.NextPosition = position;
            this.Velocity = Vector2.Zero;
            this.IsAlive = true;
            this._isActive = true;
            this.ReachedExit = false;
            this.TimeToLive = timeToLive;
            this._animationPlayer.PlayAnimation(this._idleAnimation);
            this.PreviousBoundingBox = this.BoundingBox;
        }

        /// <summary>
        /// Adds to score.
        /// </summary>
        /// <param name="newPoints">The new points.</param>
        public void AddToScore(int newPoints)
        {
            this.Score += newPoints;
        }

        /// <summary>
        /// Handles input, performs physics, and animates the sprite.
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

                // Check to see if the player has expired
                if (this.TimeToLive == TimeSpan.Zero)
                {
                    this.OnTimeExpired();
                }
                else
                {
                    // Set the player's current position and previous bounding box
                    this.Position = this.NextPosition;

                    // Get whether or not the player is colliding with a door
                    bool collidingWithDoor = this.IsCollidingWithDoor();

                    // Check whether or not the player is in front of a door
                    if (collidingWithDoor)
                    {
                        if (!this._wasCollidingWithDoor)
                        {
                            var hud = this.GetService<Hud>();
                            hud.Message = "Press UP to enter door";
                        }

                        // Be polite
                        GreetJoyce(this._door);

                        // Save off the fact that the player had been colliding with the door
                        this._wasCollidingWithDoor = true;
                    }
                    else if (this._wasCollidingWithDoor)
                    {
                        var hud = this.GetService<Hud>();
                        hud.Message = string.Empty;
                        this._wasCollidingWithDoor = false;
                    }

                    // Animate the player
                    if (this.IsAlive && this._isOnGround && !this._isJumping)
                    {
                        if (Math.Abs(this.Velocity.X) - 0.02f > 0)
                        {
                            this._animationPlayer.PlayAnimation(this._runAnimation);
                        }
                        else if (collidingWithDoor)
                        {
                            this._animationPlayer.PlayAnimation(this._idleBackAnimation);
                        }
                        else
                        {
                            this._animationPlayer.PlayAnimation(this._idleAnimation);
                        }
                    }

                    // Calculate the player's next position
                    this.ApplyPhysics(gameTime);

                    // Update the player's time to live
                    this.UpdateTimeToLive(gameTime);

                    // Update a player's invincibility
                    this._timeOfInvincibility -= gameTime.ElapsedGameTime;
                    if (this._timeOfInvincibility < TimeSpan.Zero) this._timeOfInvincibility = TimeSpan.Zero;

                    // Calculate remaining time disable player input
                    this._playerControlDisabled -= gameTime.ElapsedGameTime;

                    // Clamp the time to live at zero.
                    if (this._playerControlDisabled < TimeSpan.Zero) this._playerControlDisabled = TimeSpan.Zero;
                }
            }
        }

        /// <summary>
        /// Updates the time to live.
        /// </summary>
        /// <param name="GameTime">The game time.</param>
        public void UpdateTimeToLive(GameTime gameTime)
        {
            // Moved to a seperate method so that other things (like mini games) can update the player's time
            // without having all the other crazy player update stuff run 

            // Calculate remaining time to live
            this.TimeToLive -= gameTime.ElapsedGameTime;

            // Clamp the time to live at zero.
            if (this.TimeToLive < TimeSpan.Zero) this.TimeToLive = TimeSpan.Zero;
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        private void ApplyPhysics(GameTime gameTime)
        {
            // Get the elapsed time since the last update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the current velocity
            var velocity = this.Velocity;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += this._movement * MOVE_ACCELERATION * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GRAVITY_ACCELERATION * elapsed, -MAX_FALL_SPEED, MAX_FALL_SPEED);

            // Perform a player jump
            velocity.Y = this.DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (this._isOnGround)
                velocity.X *= GROUND_DRAG_FACTOR;
            else
                velocity.X *= AIR_DRAG_FACTOR;

            // Prevent the player from running faster than his top speed or slower than his slowest speed.
            velocity.X = MathHelper.Clamp(velocity.X, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);
            if (Math.Abs(velocity.X) < MIN_MOVE_SPEED) velocity.X = 0;

            // Apply velocity.
            this.NextPosition += velocity * elapsed;
            this.NextPosition = new Vector2((float)Math.Round(this.NextPosition.X), (float)Math.Round(this.NextPosition.Y));

            // Save the new velocity
            this.Velocity = velocity;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <param name="velocityY">The player's current velocity along the Y axis.</param>
        /// <param name="gameTime">The game time.</param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (this._isJumping)
            {
                // Begin or continue a jump
                if ((!this._wasJumping && this._isOnGround) || (this._jumpTime > 0.0f))
                {
                    if (this._jumpTime == 0.0f) this._jumpSound.Play(.3f, 0f, 0f);
                    this._jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    this._animationPlayer.PlayAnimation(this._jumpAnimation);
                    this._isOnGround = false;
                }

                // If we are in the ascent of the jump
                if ((0.0f < this._jumpTime) && (this._jumpTime <= MAX_JUMP_TIME))
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JUMP_LAUNCH_VELOCITY * (1.0f - (float)Math.Pow(this._jumpTime / MAX_JUMP_TIME, JUMP_CONTROL_POWER));
                }
                else
                {
                    // Reached the apex of the jump
                    this._jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                this._jumpTime = 0.0f;
            }

            // Store off the current state of jumping
            this._wasJumping = this._isJumping;

            // Return the new velocity along the y-axis
            return velocityY;
        }

        /// <summary>
        /// Allows the sprite to handle user input.
        /// </summary>
        /// <param name="input">The input.</param>
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (this._playerControlDisabled == TimeSpan.Zero)
            {
                // Clear input.
                this._movement = 0.0f;
                this._isJumping = false;

                // Check to see that we have input
                if (input != null)
                {
                    // Initialize the player index
                    PlayerIndex playerIndex;

                    // If any digital horizontal movement input is found, override the analog movement.
                    if (input.IsLeft(this._playerIndex, out playerIndex) && (playerIndex == this._playerIndex))
                    {
                        this._movement = -1.0f;
                    }
                    else if (input.IsRight(this._playerIndex, out playerIndex) && (playerIndex == this._playerIndex))
                    {
                        this._movement = 1.0f;
                    }
                    else if (input.IsUpPressed(this._playerIndex, out playerIndex) && (playerIndex == this._playerIndex))
                    {
                        if (this.IsCollidingWithDoor())
                        {
                            this._door.OnEnter(this);
                            this._door = null;
                        }
                    }

                    // Check if the player wants to jump.
                    this._isJumping = input.IsPrimary(this._playerIndex, out playerIndex) && (playerIndex == this._playerIndex);
                }
            }
        }

        /// <summary>
        /// Draws the animated sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Save off the previous bounding box before the draw method advances the current frame
            this.PreviousBoundingBox = this.BoundingBox;

            if (this.IsAlive && this._isActive)
            {
                // Flip the sprite to face the way we are moving.
                if (this.Velocity.X > 0)
                    this._flip = SpriteEffects.FlipHorizontally;
                else if (Velocity.X < 0)
                    this._flip = SpriteEffects.None;

                if (!this.IsInvincible || ((int)(this._timeOfInvincibility.TotalMilliseconds / 250) % 2 == 0))
                {
                    this._animationPlayer.Draw(gameTime, spriteBatch, this.Position, this._flip);
                }

#if DEBUG
                // Draw debug outline
                if (ScreenManager.DEBUG_ENABLED)
                {
                    var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                    boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                    DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.Blue);
                }
#endif
            }
        }

        /// <summary>
        /// Greets the joyce.
        /// </summary>
        /// <param name="door">The door.</param>
        private void GreetJoyce(Door door)
        {
            if (!_doorsGreetedJoyceAt.Contains(door))
            {
                this.GetService<SoundEffectManager>().PlayJoyce();
                _doorsGreetedJoyceAt.Add(door);
            }
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).</param>
        protected void OnKilled(Enemy killedBy)
        {
            this.IsAlive = false;

            if (killedBy != null)
                this._killedSound.Play();
            else
                this._fallSound.Play();

            this._animationPlayer.PlayAnimation(this._dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        protected void OnReachedExit()
        {
            this.ReachedExit = true;

            // Convert the remaining seconds into points
            int seconds = (int)Math.Ceiling(this.TimeToLive.TotalSeconds);
            this.Score += seconds * POINTS_PER_SECOND; // TODO: Fix this with formula from Phil
            this.TimeToLive = TimeSpan.Zero;

            this._animationPlayer.PlayAnimation(this._celebrateAnimation);
        }

        /// <summary>
        /// Called when time has expired for the level.
        /// </summary>
        protected void OnTimeExpired()
        {
            this.OnKilled(null);
        }

        /// <summary>
        /// Determines whether the door in colliding with the player.
        /// </summary>
        /// <returns><c>true</c> if colliding; <c>false</c> otherwise.</returns>
        private bool IsCollidingWithDoor()
        {
            return ((this._door != null) && this._door.BoundingBox.Intersects(this.BoundingBox));
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
        /// <remarks>
        /// Detects and resolves all collisions between the player and the other sprites.
        /// When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </remarks>
        public override void HandleCollisions(CollisionInfo info)
        {
            var velocityHandled = false;

            // Reset the ground just in case we need to fall
            this._isOnGround = false;

            // Get a reference to the point manager
            var pointManager = this.GetService<ObjectValueManager>();

            // Evaluate how to handle specific obstacles
            if (info.Obstacle is Enemy)
            {
                if (info.Direction.HasFlag(Direction.Bottom))
                {
                    this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
                    velocityHandled = true;

                    this.Score += pointManager.GetValue(Constants.ENEMY_KILL_VALUE);
                }
                else if (!this.IsInvincible)
                {
                    this.GetService<SoundEffectManager>().PlayHitByBug();
                    this.TimeToLive -= TimeSpan.FromSeconds(pointManager.GetValue(Constants.ENEMY_DAMAGE_VALUE_SECONDS));

                    if (this.TimeToLive <= TimeSpan.Zero)
                    {
                        this.TimeToLive = TimeSpan.Zero;
                        this.OnKilled((Enemy)info.Obstacle);
                    }

                    var enemy = (Enemy)info.Obstacle;
                    var x = (float)Math.Max(Math.Max(Math.Abs(this.Velocity.X), Math.Abs(enemy.Velocity.X)), 100000);
                    var y = 0.0f;

                    if (info.Direction.HasFlag(Direction.Right)) { x *= -1000000f; this._movement = -1.0f; }
                    if (info.Direction.HasFlag(Direction.Left)) { x *= 1000000f; this._movement = 1.0f; }

                    if (!info.Direction.HasFlag(Direction.Top))
                    {
                        this._isJumping = true;
                        this._jumpTime = MAX_JUMP_TIME * 0.5f;
                        y = JUMP_LAUNCH_VELOCITY * (1.0f - (float)Math.Pow(this._jumpTime / MAX_JUMP_TIME, JUMP_CONTROL_POWER));
                    }

                    this.Velocity = new Vector2(x, y);
                    velocityHandled = true;

                    this._playerControlDisabled = TimeSpan.FromMilliseconds(750);
                    this._timeOfInvincibility = TimeSpan.FromSeconds(pointManager.GetValue(Constants.PLAYER_INVINCIBILITY_VALUE_SECONDS));
                }
            }
            else if (info.Obstacle is Exit)
            {
                this.OnReachedExit();
            }
            else if (info.Obstacle is SoftProDollar)
            {
                this.Score += pointManager.GetValue(Constants.SOFTPRO_DOLLAR_VALUE);
            }
            else if (info.Obstacle is Door)
            {
                this._door = (Door)info.Obstacle;
            }

            // Evaluate how to handle objects in general
            if (info.Obstacle.CollidableType == CollidableType.Platform)
            {
                if (info.Direction.HasFlag(Direction.Bottom))
                {
                    this._isOnGround = true;
                    this._isJumping = false;
                    if (!velocityHandled) this.Velocity = new Vector2(this.Velocity.X, 0);
                    this.NextPosition = new Vector2(this.NextPosition.X, this.NextPosition.Y + info.CollisionOffset.Y);
                }
            }
            else if (info.Obstacle.CollidableType == CollidableType.Impassable)
            {
                if (info.Direction.HasFlag(Direction.Bottom) || info.Direction.HasFlag(Direction.Top))
                {
                    if (info.Direction.HasFlag(Direction.Bottom))
                    {
                        this._isOnGround = true;
                        this._isJumping = false;
                    }
                    if (!velocityHandled) this.Velocity = new Vector2(this.Velocity.X, 0);
                    this.NextPosition = new Vector2(this.NextPosition.X, this.NextPosition.Y + info.CollisionOffset.Y);
                }

                if (info.Direction.HasFlag(Direction.Left) || info.Direction.HasFlag(Direction.Right))
                {
                    if (!velocityHandled) this.Velocity = new Vector2(0, this.Velocity.Y);
                    this.NextPosition = new Vector2(this.NextPosition.X + info.CollisionOffset.X, this.NextPosition.Y);
                }
            }
        }

        #endregion
    }
}