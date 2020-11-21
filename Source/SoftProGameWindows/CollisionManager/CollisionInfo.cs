using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Indicates the direction of collision
    /// </summary>
    [Flags]
    public enum Direction
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        Horizontal = Left | Right,
        Vertical = Top | Bottom
    }

    /// <summary>
    /// Contains collision information.
    /// </summary>
    public class CollisionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionInfo" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="obstacle">The obstacle.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="collisionOffset">The collision offset.</param>
        public CollisionInfo(ICollidable source, ICollidable obstacle, Direction direction, Vector2 collisionOffset)
        {
            this.Source = source;
            this.Obstacle = obstacle;
            this.Direction = direction;
            this.CollisionOffset = collisionOffset;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public ICollidable Source { get; private set; }

        /// <summary>
        /// Gets the obstacle.
        /// </summary>
        /// <value>
        /// The obstacle.
        /// </value>
        public ICollidable Obstacle { get; private set; }

        /// <summary>
        /// Gets the direction of impact on the source.
        /// </summary>
        /// <value>
        /// The direction of impact on the source.
        /// </value>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Gets the amount of distance traveled by the source after colliding with the obstacle.
        /// </summary>
        /// <value>
        /// The collision offset.
        /// </value>
        public Vector2 CollisionOffset { get; private set; }
    }
}
