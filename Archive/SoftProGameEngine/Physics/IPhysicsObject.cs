using Microsoft.Xna.Framework;

namespace SoftProGameEngine.Physics
{
    public interface IPhysicsObject
    {
        Vector2 Position { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        bool GravityEnabled { get; }

        bool PhysicsEnabled { get; }

        //bool HasLanded { get; set; }
    }
}