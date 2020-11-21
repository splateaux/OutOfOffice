using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameEngine.Components
{
    public class PlayerState
    {
        private string _name;
        private Texture2D _texture;
        private int _height;
        private int _width;

        public PlayerState(string name, Texture2D texture, int height, int width)
        {
            _name = name;
            _texture = texture;
            _height = height;
            _width = width;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return _texture;
            }
        }

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }
    }
}