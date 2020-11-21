using Microsoft.Xna.Framework;
using SoftProGameEngine.Helpers;
using System.Collections.Generic;

namespace SoftProGameEngine.Physics
{
    public class PhysicsEngine
    {
        public Vector2 Gravity;

        public PhysicsEngine()
        {
        }

        public void Update(GameTime gameTime, List<IPhysicsObject> _physicsObjects)
        {
            foreach (IPhysicsObject physicsObject in _physicsObjects)
            {
                Update(gameTime, physicsObject);
            }
        }

        public void Update(GameTime gameTime, IPhysicsObject physicsObject)
        {
            float elapsed = gameTime.GetElapsedSeconds();

            if (!physicsObject.PhysicsEnabled)
                return;

            Vector2 totalAcceleration;
            if (physicsObject.GravityEnabled)
                totalAcceleration = new Vector2(Gravity.X + physicsObject.Acceleration.X, Gravity.Y + physicsObject.Acceleration.Y);
            else
                totalAcceleration = physicsObject.Acceleration;

            float newVelocityX = physicsObject.Velocity.X + totalAcceleration.X * elapsed;
            float newVelocityY = physicsObject.Velocity.Y + totalAcceleration.Y * elapsed;

            physicsObject.Velocity = new Vector2(newVelocityX, newVelocityY);

            physicsObject.Position = new Vector2(physicsObject.Position.X + physicsObject.Velocity.X * elapsed,
                physicsObject.Position.Y + physicsObject.Velocity.Y * elapsed);
        }
    }
}