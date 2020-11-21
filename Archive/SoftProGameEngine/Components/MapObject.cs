using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Framework;

namespace SoftProGameEngine.Components
{
    public abstract class MapObject : IComponent, ICollidable
    {
        //Backing fields
        private string _name;

        private int _height;
        private int _width;
        private bool _isAlive;
        private Texture2D _texture;

        public bool HitEnabled { get; set; }

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
            protected set { _texture = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Height
        {
            get { return _height; }
            protected set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            protected set { _width = value; }
        }

        public Vector2 Position { get; set; }

        #region Side Properties

        public int Top
        {
            get { return (int)Position.Y; }
        }

        public int Left
        {
            get { return (int)Position.X; }
        }

        public int Bottom
        {
            get { return (int)Position.Y + _height; }
        }

        public int Right
        {
            get { return (int)Position.X + _width; }
        }

        #endregion Side Properties

        public bool HandlesCollisions { get; set; }

        public EnemyStatus EnemyStatus
        {
            get { return _enemyStatus; }
            set { _enemyStatus = value; }
        }

        private EnemyStatus _enemyStatus;

        public bool CollisionsEnabled { get; set; }

        protected MapObject(string name, int height, int width, Texture2D texture)
        {
            //Params
            _name = name;
            Texture = texture;
            _height = height;
            _width = width;

            //Defaults
            _isAlive = false;
            HitEnabled = true;

            HandlesCollisions = false;
            CollisionsEnabled = true;

            _enemyStatus = new EnemyStatus();
        }

        public BoundingBox GetBoundingBox()
        {
            Vector3 min = new Vector3(Position, 0);
            Vector3 max = new Vector3(Position.X + Width, Position.Y + Height, 0);
            BoundingBox playerBox = new BoundingBox(min, max);
            return playerBox;
        }

        public Vector2 GetCenter()
        {
            Vector2 center = new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
            return center;
        }

        #region IComponent Members

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!_isAlive)
                return;

            Rectangle destination = new Rectangle((int)Position.X - Map.ScrollOffset, (int)Position.Y, Width, Height);
            spriteBatch.Draw(Texture, destination, Color.White);
        }

        #endregion IComponent Members

        #region ICollidable Members

        public virtual void OnBottomCollision(object collider)
        {
        }

        public virtual void OnTopCollision(object collider)
        {
        }

        public virtual void OnLeftCollision(object collider)
        {
        }

        public virtual void OnRightCollision(object collider)
        {
        }

        #endregion ICollidable Members
    }
}