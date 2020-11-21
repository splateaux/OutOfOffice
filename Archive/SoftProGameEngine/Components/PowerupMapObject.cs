using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameEngine.Components
{
    public class PowerupMapObject : PhysicsMapObject
    {
        public PowerupMapObject(string name, int height, int width, Texture2D texture, Vector2 velocity)
            : base(name, height, width, texture, velocity)
        {
            HandlesCollisions = true;
        }

        #region ICollidable Members

        public override void OnBottomCollision(object collider)
        {
            OnCollision(collider);
        }

        public override void OnTopCollision(object collider)
        {
            OnCollision(collider);
        }

        public override void OnLeftCollision(object collider)
        {
            if (collider is Tile)
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            else
                OnCollision(collider);
        }

        public override void OnRightCollision(object collider)
        {
            if (collider is Tile)
            {
                Tile tile = collider as Tile;
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
                Position = new Vector2(tile.Position.X - this.Width - 1, Position.Y);
            }
            else
                OnCollision(collider);
        }

        public virtual void OnCollision(object collider)
        {
        }

        #endregion ICollidable Members
    }
}