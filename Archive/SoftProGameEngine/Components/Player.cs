using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Framework;
using System;
using System.Collections.Generic;

namespace SoftProGameEngine.Components
{
    public class Player : PhysicsMapObject
    {
        private List<PlayerState> _playerStates;
        private int _lives;
        private int _health;
        private bool _isSpawning;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lives">The lives.</param>
        /// <param name="health">The health.</param>
        /// <param name="movementSpeed">The movement speed.</param>
        public Player(string name, int lives, int health, Vector2 movementSpeed)
            : base(name, 0, 0, null, movementSpeed)
        {
            //Basics
            this._playerStates = new List<PlayerState>();
            this.PlayerStateIndex = 0;
            this._lives = lives;
            this._health = health;

            this.Spawn();
            this.IsJumping = false;
            this.Direction = Direction.Null;

            this.Score = 0;
        }
        
        public int PlayerStateIndex { get; set; }

        public bool Gameover { get; set; }

        public bool IsJumping { get; set; }

        public Direction Direction { get; set; }

        public Texture2D DieTexture { get; set; }

        public int Score { get; set; }

        public bool HasLanded { get; set; }

        public void SetFields()
        {
            //Current state crap
            this.Height = this.CurrentState.Height;
            this.Width = this.CurrentState.Width;
            this.Texture = this.CurrentState.Texture;
        }

        public List<PlayerState> States
        {
            get { return this._playerStates; }
        }

        public int Lives
        {
            get { return this._lives; }
            set { this._lives = value; }
        }

        public bool IsSpawning
        {
            get
            {
                return this._isSpawning;
            }
            set
            {
                this._isSpawning = value;
            }
        }

        #region IComponent Members

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!this.IsAlive)
            {
                spriteBatch.Draw(this.DieTexture, new Rectangle((int)(this.Position.X - Map.ScrollOffset), (int)this.Position.Y,
                    this.CurrentState.Width, this.CurrentState.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(CurrentState.Texture, new Rectangle((int)(this.Position.X - Map.ScrollOffset), (int)this.Position.Y,
                    this.CurrentState.Width, this.CurrentState.Height), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Direction == Direction.Right)
            {
                this.Velocity = new Vector2(Math.Abs(this.MovementVelocity.X), this.Velocity.Y);
            }
            else if (this.Direction == Direction.Left)
            {
                this.Velocity = new Vector2(Math.Abs(this.MovementVelocity.X) * -1, this.Velocity.Y);
            }
            else if (this.Direction == Direction.Null)
            {
                this.Velocity = new Vector2(0, this.Velocity.Y);
            }

            base.Update(gameTime);
        }

        #endregion IComponent Members

        public PlayerState CurrentState
        {
            get
            {
                return this._playerStates[this.PlayerStateIndex];
            }
        }

        public virtual void Spawn()
        {
            this.IsAlive = true;
            this._isSpawning = true;
            this.IsJumping = true;
            this.Position = new Vector2(0, 0);
            this._health = 1;
            //Velocity = new Vector2(Velocity.X, 640);
            this.PlayerStateIndex = 0;

            this.CollisionsEnabled = true;
        }

        public void Jump(float amount)
        {
            this.IsJumping = true;
            this.HasLanded = false;
            this.Velocity = new Vector2(this.Velocity.X, (-750) * amount);
        }

        public void Jump()
        {
            this.Jump(1.0f);
        }

        public virtual void TakeDamage(int n)
        {
            this._health -= n;
            if (this._health <= 0)
            {
                this._lives--;
                if (this._lives < 0)
                {
                    this.Gameover = true;
                }
            }
        }

        public void NPlayerState(int n)
        {
            if (this.PlayerStateIndex < n)
            {
                this.PlayerStateIndex = n;
                this.Height = this.CurrentState.Height;
            }
        }

        public virtual void Die()
        {
            this.IsAlive = false;
        }

        #region ICollidable Members

        public override void OnBottomCollision(object collider)
        {
            MapObject mapObject = collider as MapObject;

            if (mapObject.EnemyStatus.IsTop)
                this.TakeDamage(1);
        }

        public override void OnTopCollision(object collider)
        {
            MapObject mapObject = collider as MapObject;

            if (mapObject.EnemyStatus.IsBottom)
                this.TakeDamage(1);
        }

        public override void OnLeftCollision(object collider)
        {
            MapObject mapObject = collider as MapObject;

            if (mapObject.EnemyStatus.IsRight)
                this.TakeDamage(1);
        }

        public override void OnRightCollision(object collider)
        {
            MapObject mapObject = collider as MapObject;

            if (mapObject.EnemyStatus.IsLeft)
                this.TakeDamage(1);
        }

        #endregion ICollidable Members
    }
}