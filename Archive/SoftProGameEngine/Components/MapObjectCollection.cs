using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SoftProGameEngine.Components
{
    public class MapObjectCollection : List<MapObject>
    {
        public MapObjectCollection()
            : base()
        {
        }

        #region IComponent Members

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (MapObject mapObject in this)
            {
                mapObject.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (MapObject mapObject in this)
            {
                mapObject.Update(gameTime);
            }
        }

        #endregion IComponent Members
    }
}