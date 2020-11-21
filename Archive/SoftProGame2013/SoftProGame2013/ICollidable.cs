using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{
    /// <summary>
    /// Marks an item as "collidable"
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        Rectangle BoundingBox { get; }

        /// <summary>
        /// Gets the type of the collidable.
        /// </summary>
        /// <value>
        /// The type of the collidable.
        /// </value>
        ICollidableType CollidableType { get; }

        /// <summary>
        /// Called when a collision happens
        /// </summary>
        /// <param name="targetOfCollision">The target of collision.</param>
        void OnCollision(ICollidable targetOfCollision);

    }
}
