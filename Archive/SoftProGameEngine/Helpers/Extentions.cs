using Microsoft.Xna.Framework;

namespace SoftProGameEngine.Helpers
{
    public static class Extensions
    {
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            return gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }

        public static BoundingBox MakeBoundingBox(this Rectangle rect)
        {
            BoundingBox boundingBox = new BoundingBox(new Vector3(rect.Left, rect.Top, 0), new Vector3(rect.Right, rect.Bottom, 0));
            return boundingBox;
        }

        public static T Cast<T>(this object o)
        {
            return (T)o;
        }

        public static bool TryCast<T>(this object o, out T cast)
        {
            if (o is T)
            {
                cast = (T)o;
                return true;
            }
            else
            {
                cast = default(T);
                return false;
            }
        }
    }
}