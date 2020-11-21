using InputController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGame2014
{
    public class Player : ICollidable
    {
        #region Fields and Events

        // The image atlas and it's directional mappings.
        private AtlasInfo _atlas;

        // Frame tracking for current animation.
        private int _currentFrame;
        private int _firstFrame;
        private int _lastFrame;
        private float animationGovernor = 0.1f;
        private float AnimationTime;
        private bool isOnGround;

        // Sprite coordinates
        private Vector2 position;

        private Vector2 velocity;

        // Flag to hold the current sprite movement direction.
        private ControllerKey previousDirection;
        private bool directionChanged;

        private float movement;

        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;
        private float _YFloor;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;

        // Location changed event.
        public delegate void PlayerLocationChangedEvent(object sender, LocationEventArgs e);

        private ICollidable _thingIhit;

        #endregion Fields and Events

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="atlas">The atlas.</param>
        public Player(AtlasInfo atlas)
        {
            _atlas = atlas;

            _firstFrame = _currentFrame = _atlas.RightMovingAnimationStartFrame;
            _lastFrame = _atlas.RightMovingAnimationEndFrame;
            position = new Vector2(350, 350);
            _YFloor = position.Y;
            isOnGround = true;
        }

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="isBackgroundMoving">if set to <c>true</c> [is background moving].</param>
        /// <param name="speed">The speed.</param>
        public void Update(GameTime gameTime, ControllerKey direction, bool isBackgroundMoving, int speed)
        {
            GetInput(direction, isBackgroundMoving);

            ApplyPhysics(gameTime);

            // If nothing is really moving, don't bother reanimating
            if (Math.Abs(Velocity.X) - 0.01f > 0 || Math.Abs(Velocity.Y) - 0.01f > 0)
            {
                DoAnimation(gameTime, direction);
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        private void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();
        }

        /// <summary>
        /// Does the animation.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        /// <param name="direction">The direction.</param>
        private void DoAnimation(GameTime gametime, ControllerKey direction)
        {
            if (directionChanged) ChangeDirection(direction);

            if (!isOnGround)
            {
                if (previousDirection.HasFlag(ControllerKey.Left))
                {
                    _currentFrame = _atlas.JumpingLeftFrame;
                }
                else
                {
                    _currentFrame = _atlas.JumpingRightFrame;
                }
            }
            else
            {

                if (direction == ControllerKey.Stationary)
                {
                    _currentFrame = _firstFrame;
                }
                else if (direction.HasFlag(ControllerKey.Right) || direction.HasFlag(ControllerKey.Left))
                {
                    AnimationTime += (float)gametime.ElapsedGameTime.TotalSeconds;

                    if (AnimationTime >= animationGovernor)
                    {
                        _currentFrame++;
                        AnimationTime = 0.0f;
                    }

                    // If the current frame is equal to last frame wrap it back to the beginning.
                    if (_currentFrame == _lastFrame) _currentFrame = _firstFrame;
                }
            }
        }

        /// <summary>
        /// Handles jump logic
        /// </summary>
        /// <param name="velocityY">The velocity y.</param>
        /// <param name="gameTime">The game time.</param>
        /// <returns></returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(ControllerKey direction, bool isBackgroundMoving)
        {
            // If any digital horizontal movement input is found, override the analog movement.
            if (direction.HasFlag(ControllerKey.Left))
            {
                directionChanged = previousDirection != ControllerKey.Left;
                previousDirection = ControllerKey.Left;

                movement = -1.0f;
            }
            else if (direction.HasFlag(ControllerKey.Right))
            {
                directionChanged = previousDirection != ControllerKey.Right;
                previousDirection = ControllerKey.Right;

                if (!isBackgroundMoving)
                {
                    // We're only moving right when the background isn't moving
                    movement = 1.0f;
                }
            }

            // Check if the player wants to jump.
            isJumping = direction.HasFlag(ControllerKey.Up);
        }

        /// <summary>
        /// Handles anything we might have collided with
        /// </summary>
        private void HandleCollisions()
        {
            isOnGround = false;

            if (_thingIhit != null)
            {
                Vector2 depth = this.BoundingBox.GetIntersectionDepth(_thingIhit.BoundingBox);

                if (depth != Vector2.Zero)
                {
                    float absDepthX = Math.Abs(depth.X);
                    float absDepthY = Math.Abs(depth.Y);

                    if (_thingIhit.CollidableType == ICollidableType.ImmoveableObject || _thingIhit.CollidableType == ICollidableType.SquishableMob)
                    {
                        if (absDepthY < absDepthX)
                        {
                            // Resolve the collision along the Y axis.
                            position = new Vector2(position.X, position.Y + depth.Y);
                        }
                        else
                        {
                            // Resolve the collision along the Y axis.
                            position = new Vector2(position.X + depth.X, position.Y);
                        }

                        // We just stopped our movement, reset our velocity and say we're on the "ground"
                        velocity.Y = 0;
                        velocity.X = 0;
                        isOnGround = true;
                    }
                }

                // Reset our collision flag
                _thingIhit = null;
            }

            // Since there's no floor, this make sure the character doesn't just keep falling down
            if (position.Y >= _YFloor)
            {
                position.Y = _YFloor;
                isOnGround = true;
            }
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            int width = _atlas.Texture.Width / _atlas.Columns;
            int height = _atlas.Texture.Height / _atlas.Rows;
            int columns = ((_lastFrame + 1) - _firstFrame);
            int row = (int)((float)_currentFrame / columns);
            int column = _currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            // This will be wrapped in a Begin/End batch in the game Draw method.
            spriteBatch.Draw(_atlas.Texture, destinationRectangle, sourceRectangle, Color.White);
        }

        /// <summary>
        /// Changing direction switched the atlas image range between left facing and right facing.
        /// </summary>
        /// <param name="direction"></param>
        ///
        private void ChangeDirection(ControllerKey direction)
        {
            if (direction.HasFlag(ControllerKey.Left))
            {
                _firstFrame = _atlas.LeftMovingAnimationStartFrame;
                _lastFrame = _atlas.LeftMovingAnimationEndFrame;
            }
            else
            {
                _firstFrame = _atlas.RightMovingAnimationStartFrame;
                _lastFrame = _atlas.RightMovingAnimationEndFrame;
            }

            // Reset first frame to start of new image range and update direction.
            _currentFrame = _firstFrame;
        }

        /// <summary>
        /// Gets or sets the animation governor.
        /// </summary>
        /// <value>
        /// The animation governor.
        /// </value>
        public float AnimationGovernor
        {
            get { return animationGovernor; }
            set { animationGovernor = value; }
        }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    _atlas.Texture.Width / _atlas.Columns,
                    _atlas.Texture.Height / _atlas.Rows);
            }
        }

        /// <summary>
        /// Gets the type of the collidable.
        /// </summary>
        /// <value>
        /// The type of the collidable.
        /// </value>
        public ICollidableType CollidableType
        {
            get { return ICollidableType.Player; }
        }

        /// <summary>
        /// Called when a collision happens
        /// </summary>
        /// <param name="targetOfCollision">The target of collision.</param>
        public void OnCollision(ICollidable targetOfCollision)
        {
            _thingIhit = targetOfCollision;
        }
    }
}