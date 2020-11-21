using Microsoft.Xna.Framework;

namespace SoftProGameWindows
{
    /// <summary>
    /// Identifies all the various types of collidable types.
    /// </summary>
    public enum CollidableType
    {
        /// <summary>
        /// A passable object is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable object is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform object is one which behaves like a passable object except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2
    }

    /// <summary>
    /// Marks an item as "collidable"
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets the bounding box of the object at the current position.
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
        CollidableType CollidableType { get; }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="info">The collision information.</param>
        void HandleCollisions(CollisionInfo info);
    }
}