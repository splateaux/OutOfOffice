using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Physics;

namespace SoftProGameEngine.Components
{
    public class PhysicsMapObject : MapObject, IPhysicsObject
    {
        #region Physics Properties

        //Enabled
        public bool PhysicsEnabled { get; set; }

        public bool GravityEnabled { get; set; }

        //State vars
        //private Vector2 _position;
        private Vector2 _velocity;

        private Vector2 _acceleration;
        private Vector2 _spawnPosition;

        //Increment vars
        private Vector2 _movementVelocity;

        #endregion Physics Properties

        public static PhysicsEngine PhysicsEngine;

        public PhysicsMapObject(string name, int height, int width, Texture2D texture, Vector2 movementVelocity)
            : this(name, height, width, texture, movementVelocity, Vector2.Zero)
        {
        }

        public PhysicsMapObject(string name, int height, int width, Texture2D texture, Vector2 movementVelocity, Vector2 position)
            : base(name, height, width, texture)
        {
            //Enabled
            PhysicsEnabled = true;
            GravityEnabled = true;

            //State vars
            Position = position;
            _velocity = new Vector2(0, 0);
            _acceleration = new Vector2(0, 0);

            //Increment vars
            _movementVelocity = movementVelocity;
        }

        public virtual void Spawn(Vector2 position)
        {
            _spawnPosition = position;
            Position = position;
            IsAlive = true;
        }

        public virtual void Reset()
        {
            Position = _spawnPosition;
            IsAlive = true;
            PhysicsEnabled = true;
            HitEnabled = true;
        }

        #region IPhysicsObject Members

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }

        public Vector2 Acceleration
        {
            get
            {
                return _acceleration;
            }
            set
            {
                _acceleration = value;
            }
        }

        public Vector2 MovementVelocity
        {
            get { return _movementVelocity; }
            set { _movementVelocity = value; }
        }

        #endregion IPhysicsObject Members

        #region ICollidable Members

        public override void OnBottomCollision(object collider)
        {
        }

        public override void OnTopCollision(object collider)
        {
        }

        public override void OnLeftCollision(object collider)
        {
        }

        public override void OnRightCollision(object collider)
        {
        }

        #endregion ICollidable Members

        #region IComponent Members

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            if (PhysicsEnabled)
            {
                PhysicsEngine.Update(gameTime, this);
            }
        }

        #endregion IComponent Members
    }
}