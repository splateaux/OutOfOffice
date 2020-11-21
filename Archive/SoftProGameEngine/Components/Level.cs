using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoftProGameEngine.Framework;
using System.Collections.Generic;

namespace SoftProGameEngine.Components
{
    public class Level : IComponent
    {
        private string _name;
        private Dictionary<string, Map> _maps;
        private string _currentMap;

        public Level(string name)
        {
            _name = name;
            _maps = new Dictionary<string, Map>();
        }

        public Level(string name, Dictionary<string, Map> maps)
        {
            _name = name;
            _maps = maps;
        }

        public void Add(Map map)
        {
            _maps.Add(map.Name, map);
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Dictionary<string, Map> Maps
        {
            get
            {
                return _maps;
            }
        }

        public void ChangeMap(string name)
        {
            _currentMap = name;
            _maps[_currentMap].Reset();
        }

        #region IComponent Members

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _maps[_currentMap].Draw(gameTime, spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            _maps[_currentMap].Update(gameTime);
        }

        public Map CurrentMap
        {
            get
            {
                return _maps[_currentMap];
            }
        }

        #endregion IComponent Members
    }
}