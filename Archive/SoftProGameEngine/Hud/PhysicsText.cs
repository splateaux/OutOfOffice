using Microsoft.Xna.Framework;
using SoftProGameEngine.Physics;

namespace SoftProGameEngine.Hud
{
    public class PhysicsText : IPhysicsObject
    {
        #region IPhysicsObject Members

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        private bool _gravityEnabled;

        public bool GravityEnabled
        {
            get { return _gravityEnabled; }
        }

        private bool _physicsEnabled;

        public bool PhysicsEnabled
        {
            get { return _physicsEnabled; }
        }

        #endregion IPhysicsObject Members

        private string _text;
        private float _dieTimer;

        public PhysicsText(string text, Vector2 position)
            : this(text, position, 2f)
        {
        }

        public PhysicsText(string text, Vector2 position, float dieTimer)
        {
            _text = text;
            Position = position;
            _dieTimer = dieTimer;
        }
    }
}