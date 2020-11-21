using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameEngine.Components
{
    public class BackgroundTexture
    {
        private Point _point;

        public Texture2D Texture { get; set; }

        private int _height;
        private int _width;

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }

        public Point Point
        {
            get { return _point; }
        }

        public BackgroundTexture(Point point, Texture2D texture, int height, int width)
        {
            _point = point;
            Texture = texture;
            _height = height;
            _width = width;
        }
    }
}