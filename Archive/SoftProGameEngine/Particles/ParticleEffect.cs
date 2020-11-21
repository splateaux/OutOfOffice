using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Components;
using SoftProGameEngine.Framework;
using SoftProGameEngine.Physics;
using System.Collections.Generic;

namespace SoftProGameEngine.Particles
{
    public class ParticleEffect : IComponent
    {
        public static PhysicsEngine PhysicsEngine;

        private List<PhysicsMapObject> _particles;
        private Vector2 _center;

        public ParticleEffect(Vector2 center)
        {
            _particles = new List<PhysicsMapObject>();
            _center = center;
        }

        public List<PhysicsMapObject> Particles
        {
            get
            {
                return _particles;
            }
        }

        #region IComponent Members

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (PhysicsMapObject particle in _particles)
            {
                particle.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                PhysicsMapObject particle = _particles[i];

                particle.Update(gameTime);

                if (particle.Position.Y > Map.ScreenBounds.Height)
                {
                    particle.IsAlive = false;
                }
            }
        }

        #endregion IComponent Members

        public bool IsAlive
        {
            get
            {
                foreach (PhysicsMapObject particle in _particles)
                {
                    if (particle.IsAlive)
                        return true;
                }
                return false;
            }
        }
    }
}