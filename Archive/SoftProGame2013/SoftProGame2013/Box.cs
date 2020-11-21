using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{
    /// <summary>
    /// A Box... it's a thing
    /// </summary>
    class Box : Sprite, ICollidable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public Box(Texture2D texture, Vector2 position, float scale = 1)
            : base(texture, position, scale)
        { }

        /// <summary>
        /// Gets the type of the collidable.
        /// </summary>
        /// <value>
        /// The type of the collidable.
        /// </value>
        public ICollidableType CollidableType
        {
            get { return ICollidableType.ImmoveableObject; }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="targetOfCollision">The target of collision.</param>
        public void OnCollision(ICollidable targetOfCollision)
        {
            // Nothing happens... I WIN!
        }
    }
}
