using Microsoft.Xna.Framework;
namespace SoftProGameWindows
{
    /// <summary>
    /// Marks an item as "collidable" and that the item is not stationary.
    /// </summary>
    public interface IMovingCollidable : ICollidable
    {
        /// <summary>
        /// Gets the previous bounding box.
        /// </summary>
        /// <value>
        /// The previous bounding box.
        /// </value>
        Rectangle PreviousBoundingBox { get; }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        Vector2 Velocity { get; }
    }
}