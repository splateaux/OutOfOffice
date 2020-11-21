using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameEngine.Components
{
    public class Tile : MapObject
    {
        public Map Map { get; set; }

        public const int DefaultWidth = 40;
        public const int DefaultHeight = 40;

        public const string TileFormat = "Tile{0}";

        public Tile(string name, Texture2D texture)
            : base(name, DefaultHeight, DefaultWidth, texture)
        {
        }

        public Tile(string name, Texture2D texture, int height, int width)
            : base(name, height, width, texture)
        {
        }

        public static bool TryCast<T>(Tile tileIn, out T cast)
        {
            if (tileIn is T)
            {
                cast = (T)(object)tileIn;
                return true;
            }
            else
            {
                cast = default(T);
                return false;
            }
        }

        public virtual void Reset()
        {
        }
    }
}