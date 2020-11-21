#region File Description

//-----------------------------------------------------------------------------
// Tile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameWindows
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    public class Tile : ICollidable
    {
        public int x, y;
        public Vector2 Position;
        public Texture2D Texture;
        public TileCollision Collision;

        private Rectangle _boundingBox;

        public const int Width = 40;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(int x, int y, Texture2D texture, TileCollision collision)
        {
            this.x = x;
            this.y = y;

            //var position = (new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height)).GetBottomCenter();
            //this._boundingBox = new Rectangle((int)position.X, (int)position.Y, Tile.Width, Tile.Height);
            this._boundingBox = new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);

            this.Texture = texture;
            this.Collision = collision;
            if (collision != TileCollision.Passable) CollisionManager.Collection.Add(this);
        }

        // HACK: Plan to remove tiles altogether eventually
        #region ICollidable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public Rectangle BoundingBox
        {
            get
            {
                return this._boundingBox;
            }
        }

        /// <summary>
        /// Gets the type of the collidable.
        /// </summary>
        /// <value>
        /// The type of the collidable.
        /// </value>
        public ICollidableType CollidableType
        {
            get { return ICollidableType.Impassable; }
        }

        /// <summary>
        /// Called when a collision happens
        /// </summary>
        /// <param name="targetOfCollision">The target of collision.</param>
        public void OnCollision(ICollidable targetOfCollision)
        {
            // Do nothing
        }

        #endregion
    }
}