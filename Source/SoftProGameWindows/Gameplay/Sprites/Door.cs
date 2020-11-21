using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// An exit
    /// </summary>
    public class Door : CollidableSprite
    {
        // Door state
        private string _levelScreenName;
        private SoundEffect _doorSound;
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box" /> class.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="levelScreenName">Name of the level.</param>
        public Door(IServiceProvider site, Vector2 position, Texture2D texture, string levelScreenName)
            : base(site, position, CollidableType.Passable, texture)
        {
            this._levelScreenName = levelScreenName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Door(SpriteInfo info)
            : base(info, CollidableType.Passable)
        {
            var setting = info.Settings.FirstOrDefault(s => s.Key.Equals("LevelScreenName", StringComparison.OrdinalIgnoreCase));
            this._levelScreenName = setting.Value;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content mananger.</param>
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            this._doorSound = content.Load<SoundEffect>("Audio/Effects/OpenDoor");
            this._isActive = true;
        }

        /// <summary>
        /// Allows the sprite to handle user input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void OnEnter(Player player)
        {
            // Play the sound and deactivate the door
            this._doorSound.Play();
            this._isActive = false;
            CollisionManager.Collection.QueuePendingRemoval(this);

            // Create the level
            var screenType = TypeDelegator.GetType("SoftProGameWindows." + this._levelScreenName);
            var screen = (LevelScreen)Activator.CreateInstance(screenType);

            // Add it to the screen manager
            var sm = this.GetService<ScreenManager>();
            sm.AddScreen(screen, null);

            // Spawn the player
            screen.SpawnPlayer(player);
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

        #endregion
    }
}
