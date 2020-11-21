using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Components;
using SoftProGameEngine.Framework;
using System.Collections.Generic;

namespace SoftProGameEngine.Particles
{
    public class ParticleEffectCollection : List<ParticleEffect>, IComponent
    {
        public static Map Map;

        public ParticleEffectCollection(Map map)
            : base()
        {
            Map = map;
        }

        #region IComponent Members

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (ParticleEffect effect in this)
            {
                effect.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            int i = 0;
            while (i < this.Count)
            {
                ParticleEffect particleEffect = this[i];

                particleEffect.Update(gameTime);

                if (!particleEffect.IsAlive)
                    this.Remove(particleEffect);
                else
                    i++;
            }
        }

        #endregion IComponent Members
    }
}