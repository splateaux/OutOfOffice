using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{
    class SquishyBug : Sprite , ICollidable
    {
        ICollidable _thingIHit;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box" /> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        public SquishyBug(Texture2D texture, Vector2 position, float scale = 1)
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
            get { return ICollidableType.SquishableMob; }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="targetOfCollision">The target of collision.</param>
        public void OnCollision(ICollidable targetOfCollision)
        {
            _thingIHit = targetOfCollision;
        }

        /// <summary>
        /// Updates the specified background moving.
        /// </summary>
        /// <param name="backgroundMoving">if set to <c>true</c> [background moving].</param>
        /// <param name="scrollSpeed">The scroll speed.</param>
        public override void Update(bool backgroundMoving, int scrollSpeed)
        {
            if (_thingIHit != null && _thingIHit.CollidableType == ICollidableType.Player)
            {
                if (this.GetCollisionDirection(_thingIHit) == CollisionDirection.Bottom)
                {
                    base.IsDestroyed = true;
                }
            }

            base.Update(backgroundMoving, scrollSpeed);
        }
    }
}
