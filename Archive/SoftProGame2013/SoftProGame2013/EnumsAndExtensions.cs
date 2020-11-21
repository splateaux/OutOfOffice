using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{

    /// <summary>
    /// Just a basic start to the general "types" of collidable types
    /// </summary>
    public enum ICollidableType
    {
        Player,
        SquishableMob,
        NonSquishableMob,
        ImmoveableObject,
        LeftWall,
        RightWall,
        Ceiling,
        Floor,
        Coin,
    }

    /// <summary>
    /// How things can collide
    /// </summary>
    public enum CollisionDirection
    {
        None,
        Top,
        Bottom,
        Side,
        Tie
    }


    public static class Extensions
    {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            /* Totally stole this from that microsoft sample game */

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
        /// Gets the collision direction... i.e. Am I on bottom, top, side, etc...
        /// </summary>
        /// <param name="thing1">The thing1.</param>
        /// <param name="thing2">The thing2.</param>
        /// <returns></returns>
        public static CollisionDirection GetCollisionDirection(this ICollidable thing1, ICollidable thing2)
        {
            Vector2 depth = thing1.BoundingBox.GetIntersectionDepth(thing2.BoundingBox);

            if (depth == Vector2.Zero) return CollisionDirection.None;

            float absDepthX = Math.Abs(depth.X);
            float absDepthY = Math.Abs(depth.Y);

            // Top / bottom
            if (absDepthY < absDepthX)
            {
                if (depth.Y > 0) return CollisionDirection.Bottom;
                return CollisionDirection.Top;
            }
            else if (absDepthX < absDepthY) // Side
            {
                return CollisionDirection.Side;
            }
            else
            {
                return CollisionDirection.Tie;
            }

        }
    }
}
