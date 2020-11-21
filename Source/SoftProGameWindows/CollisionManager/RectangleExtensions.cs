using Microsoft.Xna.Framework;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// A set of helpful methods for working with rectangles.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which sides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the top left coordinate of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The top left coordinate.</returns>
        public static Vector2 GetTopLeft(this Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y);
        }

        /// <summary>
        /// Gets the top right coordinate of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The top right coordinate.</returns>
        public static Vector2 GetTopRight(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width, rect.Y);
        }

        /// <summary>
        /// Gets the bottom left coordinate of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The bottom left coordinate.</returns>
        public static Vector2 GetBotomLeft(this Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y + rect.Height);
        }

        /// <summary>
        /// Gets the bottom right coordinate of the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The bottom right coordinate.</returns>
        public static Vector2 GetBottomRight(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
        }

        /// <summary>
        /// Gets the intersecting rectangle between two rectangles.
        /// </summary>
        /// <param name="a">The source rectangle.</param>
        /// <param name="b">The target rectangle.</param>
        /// <returns>The intersecting rectangle.</returns>
        public static Rectangle GetIntersectingRectangle(this Rectangle a, Rectangle b)
        {
            int x1 = Math.Max(a.X, b.X);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Max(a.Y, b.Y);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }
    }
}