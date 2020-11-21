using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// An exit
    /// </summary>
    public class Exit : CollidableSprite
    {
        // Exit state
        private SoundEffect _exitReachedSound;
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="scale">The scale (default = 1).</param>
        public Exit(IServiceProvider site, Vector2 position, Texture2D texture, float scale = 1)
            : base(site, position, CollidableType.Passable, texture, scale)
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exit"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Exit(SpriteInfo info)
            : base(info, CollidableType.Passable)
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content mananger.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            this._exitReachedSound = content.Load<SoundEffect>("Audio/Effects/ExitReached");
        }

        #region ICollidable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public override bool IsActive
        {
            get { return this._isActive; }
        }

        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="info">The collision information.</param>
        public override void HandleCollisions(CollisionInfo info)
        {
            var player = info.Obstacle as Player;
            if ((player != null) && this._isActive)
            {
                this._exitReachedSound.Play();
                this._isActive = false;
                CollisionManager.Collection.QueuePendingRemoval(this);
            }
        }

        #endregion
    }
}
