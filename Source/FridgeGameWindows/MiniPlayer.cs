using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FridgeGame
{
    public class MiniPlayer
    {
        // Constants for controling horizontal movement
        private const float MoveAcceleration = 18000.0f;
        private const float MaxMoveSpeed = 2500.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 0f;//3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f; 
        
       // private Breakroom _breakroom;
        private NintendoControllerState _controllerState;
        private const float MoveStickScale = 1.0f;
        private float movement;
        private bool isCatching;
        private const Buttons JumpButton = Buttons.A;
        //private bool _hasCan;
        private bool _isCatching;

        Breakroom _breakroom;
        Vector2 _position;
        Texture2D _texture;
        String _name;

        public Vector2 Position
        {
            get { return _position; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
        }

        public Breakroom Room
        {
            get { return _breakroom; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsCatching
        {
            get { return _isCatching; }
        }

        public bool HasCan
        {
            get 
            {
                if (CurrentlyHolding != null)
                    return true;
                else
                    return false;
            }
            //set { _hasCan = value; }
        }

        public SodaCan CurrentlyHolding { get; set; }


        private Rectangle relativeBoundingBox = new Rectangle(0, 0, 0, 0);
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + relativeBoundingBox.X,
                    (int)Position.Y + relativeBoundingBox.Y,
                    (relativeBoundingBox.Width > 0) ? relativeBoundingBox.Width : Texture.Width,
                    (relativeBoundingBox.Height > 0) ? relativeBoundingBox.Height : Texture.Height);
            }
        }


        public MiniPlayer(Breakroom breakroom, Vector2 startPosition, String name)
        {
            
            _breakroom = breakroom;
            _position = startPosition;
            _name = name;

            LoadContent();
        }

        public void LoadContent()
        {
            //switch (_name)
            //{
            //    case "John":
            //        charName = "HandsDown";
            //        break;
            //    default:
            //        charName = "HandsDown";
            //        break;
            //}


            this._texture = _breakroom.Content.Load<Texture2D>(String.Format("Textures/Players/{0}/FridgeGame/HandsDown", _name));
            this._isCatching = false;
        }

        public void Update(GameTime gameTime, NintendoControllerState controllerState,KeyboardState keyboardState)
        {
            GetInput(controllerState, keyboardState);
            ApplyPhysics(gameTime);
            //ClearInputs();
            movement = 0.0f;            
        }

        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            //velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            //if (IsOnGround)
                velocity.X *= GroundDragFactor;
            //else
            //    velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            _position += velocity * elapsed;
            _position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            //HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (HasCan)
            {
                switch (CurrentlyHolding.DrinkType)
                {
                    case SodaCan.Brand.Can_DrDocuments_Small:
                        spriteBatch.Draw(_breakroom.Content.Load<Texture2D>(String.Format("Textures/Players/{0}/FridgeGame/HandsUp_DrDocuments", _name)), new Vector2(_position.X, _position.Y - 165), Color.White);
                        break;
                    case SodaCan.Brand.Can_ImplementationMist_Small:
                        spriteBatch.Draw(_breakroom.Content.Load<Texture2D>(String.Format("Textures/Players/{0}/FridgeGame/HandsUp_ImpMist", _name)), new Vector2(_position.X, _position.Y - 165), Color.White);
                        break;
                    case SodaCan.Brand.Can_SupportSoda_Small:
                        spriteBatch.Draw(_breakroom.Content.Load<Texture2D>(String.Format("Textures/Players/{0}/FridgeGame/HandsUp_SupportSoda", _name)), new Vector2(_position.X, _position.Y - 165), Color.White);
                        break;

                }
                
            }
            else if (_isCatching)
            {
                spriteBatch.Draw(_breakroom.Content.Load<Texture2D>(String.Format("Textures/Players/{0}/FridgeGame/HandsUp_Empty", _name)), new Vector2(_position.X, _position.Y - 5), Color.White);
            }
            else
            {
                spriteBatch.Draw(_texture, _position, Color.White);

            }
        }

        private void ReleaseCan()
        {
            if (HasCan)
            {
                _breakroom.OnCanReleased(this.CurrentlyHolding);
                this.CurrentlyHolding = null;

            }
        }
        
        private void GetInput(NintendoControllerState controllerState, KeyboardState keyboardState)
        {
            //movement = controllerState.DPad.Left * MoveStickScale;
            movement = 0;// MoveStickScale;

            if (controllerState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                movement = -1.0f;
            }
            else if (controllerState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right))
            {
                movement = 1.0f;
            }

            if (controllerState.IsButtonDown(Buttons.A) ||
                keyboardState.IsKeyDown(Keys.Space))
            {
                if (!HasCan)
                {
                    _isCatching = true;
                }
                else
                {
                    _isCatching = false;
                }
            }
            else
            {
                _isCatching = false;
                
                if (HasCan)
                {
                    ReleaseCan();
                }

            }
        }

    }
}
