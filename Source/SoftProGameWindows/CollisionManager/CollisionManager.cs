using System.Linq;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SoftProGameWindows
{
    /// <summary>
    /// Manages collisions and collision events between all gameplay objects.
    /// </summary>
    public class CollisionManager
    {
        #region Singleton

        /// <summary>
        /// Singleton for collision management.
        /// </summary>
        private static CollisionManager _collisionManager;

        /// <summary>
        /// Gets the current collision manager.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public static Quadtree Collection
        {
            get
            {
                if (_collisionManager != null)
                {
                    return _collisionManager._collisionTree;
                }

                return null;
            }
        }

        #endregion Singleton

        private Quadtree _collisionTree;
        private HashSet<ICollidable> _allObjects;
        private HashSet<IMovingCollidable> _movingObjects;
        private Layer _collisionLayer;

        /// <summary>
        /// Constructs a new collision manager.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="collisionLayer">The collision layer.</param>
        private CollisionManager(Rectangle bounds, Layer collisionLayer = null)
        {
            this._collisionTree = new Quadtree(bounds);
            this._allObjects = new HashSet<ICollidable>();
            this._movingObjects = new HashSet<IMovingCollidable>();
            this._collisionLayer = collisionLayer;

            this._collisionTree.ItemInserted += _collisionTree_ItemInserted;
            this._collisionTree.ItemRemoved += _collisionTree_ItemRemoved;
        }

        /// <summary>
        /// Handles the ItemInserted event of the _collisionTree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="QuadtreeItemEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _collisionTree_ItemInserted(object sender, QuadtreeItemEventArgs e)
        {
            var collidable = e.Item as IMovingCollidable;
            if (collidable != null)
            {
                this._movingObjects.Add(collidable);
            }
            this._allObjects.Add((ICollidable)e.Item);
        }

        /// <summary>
        /// Handles the ItemRemoved event of the _collisionTree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="QuadtreeItemEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _collisionTree_ItemRemoved(object sender, QuadtreeItemEventArgs e)
        {
            var collidable = e.Item as IMovingCollidable;
            if (collidable != null)
            {
                this._movingObjects.Remove(collidable);
            }
            this._allObjects.Remove((ICollidable)e.Item);
        }

        /// <summary>
        /// Collisions the manager scope.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        public static IDisposable NewScope(Rectangle bounds, Layer collisionLayer = null)
        {
            return new CollisionManagerScope(bounds, collisionLayer);
        }

        /// <summary>
        /// Suppresses the collision manager scope.
        /// </summary>
        /// <returns></returns>
        public static IDisposable Suppress()
        {
            return new CollisionManagerScope();
        }

        /// <summary>
        /// Gets the smallest rectangle that surrounds the visible pixels of a texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="sourceRectangle">The source rectangle within the texture to be painted.</param>
        /// <returns>A rectangle around the visible pixels.</returns>
        public static Rectangle GetSmallestRectangleFromTexture(Texture2D texture, Rectangle? sourceRectangle)
        {
            // Initialize some variables
            var width = (sourceRectangle.HasValue ? sourceRectangle.Value.Width : texture.Width);
            var height = (sourceRectangle.HasValue ? sourceRectangle.Value.Height : texture.Height);

            // Initialize the bounding box sides
            var top = height;
            var left = width;
            var bottom = 0;
            var right = 0;

            // Get color data for the texture
            var colors = new Color[sourceRectangle.Value.Width * sourceRectangle.Value.Height];
            texture.GetData<Color>(0, sourceRectangle, colors, 0, colors.Length);

            // Starting from the top of the image (or source rectangle) scan through the y-positions
            // looking for the top- and bottom-most non-transparent pixels.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (colors[x + y * width].A != 0)
                    {
                        if (y < top) top = y;
                        if (x < left) left = x;
                        if (y > bottom) bottom = y;
                        if (x > right) right = x;
                    }
                }
            }

            // Return the smallest rectangle
            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }

        /// <summary>
        /// Converts the world position of the given layer to the world position on the collision layer.
        /// </summary>
        /// <param name="worldPosition">The world position.</param>
        /// <param name="layer">The layer for which the position applies.</param>
        /// <returns>The world position on the collision layer.</returns>
        public static Vector2 ConvertWorldPosition(Vector2 worldPosition, Layer layer)
        {
            // Check to see if the position needs to be converted or if it can be converted
            if ((_collisionManager == null) ||
                (_collisionManager._collisionLayer == null) ||
                (_collisionManager._collisionLayer == layer))
            {
                return worldPosition;
            }
            else
            {
                return _collisionManager._collisionLayer.ScreenToWorld(layer.WorldToScreen(worldPosition));
            }
        }

        /// <summary>
        /// Converts the screen position to the world position on the collision layer.
        /// </summary>
        /// <param name="screenPosition">The screen position.</param>
        /// <returns>The world position on the collision layer.</returns>
        public static Vector2 ConvertScreenPosition(Vector2 screenPosition)
        {
            // Check to see if the position needs to be converted or if it can be converted
            if ((_collisionManager == null) ||
                (_collisionManager._collisionLayer == null))
            {
                return screenPosition;
            }
            else
            {
                return _collisionManager._collisionLayer.ScreenToWorld(screenPosition);
            }
        }

        /// <summary>
        /// Update the collision system.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <exception cref="System.InvalidOperationException">The collision manager has not yet been initialized.</exception>
        public static void Update(GameTime gameTime)
        {
            // Safety-check the singleton
            if (_collisionManager == null) return;

            // Get all the moving objects
            var movingObjects = _collisionManager._movingObjects.ToArray();

            // Update the positions of all the objects
            _collisionManager._collisionTree.Clear();
            foreach (var item in _collisionManager._allObjects)
            {
                _collisionManager._collisionTree.Insert(item);
            }

            // Handle the collisions between moving objects and other objects
            foreach (var movingObject in movingObjects)
            {
                HandleCollisions(Direction.Vertical, movingObject);
                HandleCollisions(Direction.Horizontal, movingObject);
            }

            // Apply pending removals (e.g. objects that died because of a collision)
            _collisionManager._collisionTree.ApplyPendingRemovals();
        }

        /// <summary>
        /// Handles the collisions.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="source">The source.</param>
        public static void HandleCollisions(Direction direction, IMovingCollidable source)
        {
            // Safety-check the singleton
            if (_collisionManager == null) return;

            if (source.IsActive)
            {
                var sourcePreviousBoundingBox = source.PreviousBoundingBox;

                // Loop through the possible targets
                foreach (var target in _collisionManager._collisionTree.Retrieve(source))
                {
                    // Initialize variables
                    var sourceBoundingBox = source.BoundingBox;
                    var targetBoundingBox = target.BoundingBox;

                    // Determine if source and target currently overlap (e.g. a collision has occurred
                    if ((source != target) && sourceBoundingBox.Intersects(targetBoundingBox))
                    {
                        // Initialize the variables
                        var collisionOffset = Vector2.Zero;
                        var sourceDirection = Direction.None;
                        var targetDirection = Direction.None;

                        // Evaluate only in the directions specfied
                        if (direction.HasFlag(Direction.Top))
                        {
                            // Source Top
                            if (source.PreviousBoundingBox.Top >= targetBoundingBox.Bottom)
                            {
                                collisionOffset = new Vector2(collisionOffset.X, targetBoundingBox.Bottom - sourceBoundingBox.Top);
                                sourceDirection |= Direction.Top;
                                targetDirection |= Direction.Bottom;
                            }
                        }

                        if (direction.HasFlag(Direction.Bottom))
                        {
                            // Source Bottom
                            if (source.PreviousBoundingBox.Bottom <= targetBoundingBox.Top)
                            {
                                collisionOffset = new Vector2(collisionOffset.X, targetBoundingBox.Top - sourceBoundingBox.Bottom);
                                sourceDirection |= Direction.Bottom;
                                targetDirection |= Direction.Top;
                            }
                        }

                        if (direction.HasFlag(Direction.Left))
                        {
                            // Source Left
                            if (source.PreviousBoundingBox.Left >= targetBoundingBox.Right)
                            {
                                collisionOffset = new Vector2(targetBoundingBox.Right - sourceBoundingBox.Left, collisionOffset.Y);
                                sourceDirection |= Direction.Left;
                                targetDirection |= Direction.Right;
                            }
                        }

                        if (direction.HasFlag(Direction.Right))
                        {
                            // Source Right
                            if (source.PreviousBoundingBox.Right <= targetBoundingBox.Left)
                            {
                                collisionOffset = new Vector2(targetBoundingBox.Left - sourceBoundingBox.Right, collisionOffset.Y);
                                sourceDirection |= Direction.Right;
                                targetDirection |= Direction.Left;
                            }
                        }

                        // Handle the collision if one occurred
                        if (sourceDirection != Direction.None)
                        {
                            // Tell the source and target about the collision
                            source.HandleCollisions(new CollisionInfo(source, target, sourceDirection, collisionOffset));
                            target.HandleCollisions(new CollisionInfo(target, source, targetDirection, -collisionOffset));
                        }
                    }
                }
            }
        }

        /*
        // WORKS! But a little too good.... Can't process collisions in any specific order (e.g. vertical collisions before horizontal ones)

        /// <summary>
        /// Get the collision information between two objects
        /// </summary>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceInfo">The source collision information.</param>
        /// <param name="targetInfo">The target collision information.</param>
        /// <returns></returns>
        private static bool GetCollisionInfo(float elapsedTime,
            IMovingCollidable source, ICollidable target,
            out CollisionInfo sourceInfo, out CollisionInfo targetInfo)
        {
            // Initialize some variables
            var sourceBoundingBox = source.BoundingBox;
            var sourceVelocity = source.Velocity;
            var targetBoundingBox = target.BoundingBox;
            var targetVelocity = Vector2.Zero;
            var collisionEdge = Direction.None;
            var distance = Vector2.Zero;
            sourceInfo = null;
            targetInfo = null;

            // Check to see if the target is moving
            var movingTarget = target as IMovingCollidable;
            if (movingTarget != null)
            {
                // Adjust source velocity relative to target velocity (e.g. make target, in essence, stand still)
                sourceVelocity -= movingTarget.Velocity;
            }

            // Determine which sides of source which could have collided with the target based on velocity direction
            var sourceCollisionEdges = Direction.None;
            if (sourceVelocity.X < 0) sourceCollisionEdges |= Direction.Left;
            if (sourceVelocity.X > 0) sourceCollisionEdges |= Direction.Right;
            if (sourceVelocity.Y < 0) sourceCollisionEdges |= Direction.Top;
            if (sourceVelocity.Y > 0) sourceCollisionEdges |= Direction.Bottom;

            // Check for vertical collision first because gravity is always acting on the player
            if (sourceVelocity.Y != 0)
            {
                // Get the distance required to make the x-axis of each shape colinear
                distance.Y = (sourceVelocity.Y < 0) ? targetBoundingBox.Bottom - sourceBoundingBox.Top :
                                                      targetBoundingBox.Top - sourceBoundingBox.Bottom;

                distance.X = sourceVelocity.X * (distance.Y / sourceVelocity.Y);

                // Ensure that the object was actually ouside the bounds during the last update
                if ((distance.Y / sourceVelocity.Y) < elapsedTime)
                {
                    // Create a bounding box that relates to the new source's location
                    var newSourceBoundingBox = new Rectangle(sourceBoundingBox.X + (int)distance.X,
                                                             sourceBoundingBox.Y + (int)distance.Y,
                                                             sourceBoundingBox.Width,
                                                             sourceBoundingBox.Height);

                    // Check for collision by seeing if x-axis sides of the source and target overlap
                    var verticalCollisionTest = Math.Min(newSourceBoundingBox.Right, targetBoundingBox.Right) -
                                                Math.Max(newSourceBoundingBox.Left, targetBoundingBox.Left);

                    // Set the appropriate flags according to the direction of collision (if a collision occured)
                    if (verticalCollisionTest > 0)
                    {
                        collisionEdge = sourceCollisionEdges & Direction.Vertical;
                    }
                    else if (verticalCollisionTest == 0)
                    {
                        collisionEdge = sourceCollisionEdges;
                    }
                }
            }

            // Check for horizontal collision second
            if ((collisionEdge == Direction.None) && (sourceVelocity.X != 0))
            {
                // Get the distance required to make the x-axis of each shape colinear
                distance.X = (sourceVelocity.X < 0) ? targetBoundingBox.Right - sourceBoundingBox.Left :
                                                      targetBoundingBox.Left - sourceBoundingBox.Right;

                distance.Y = sourceVelocity.Y * (distance.X / sourceVelocity.X);

                // Ensure that the object was actually ouside the bounds during the last update
                if ((distance.X / sourceVelocity.X) < elapsedTime)
                {
                    // Create a bounding box that relates to the new source's location
                    var newSourceBoundingBox = new Rectangle(sourceBoundingBox.X + (int)distance.X,
                                                             sourceBoundingBox.Y + (int)distance.Y,
                                                             sourceBoundingBox.Width,
                                                             sourceBoundingBox.Height);

                    // Check for collision by seeing if x-axis sides of the source and target overlap
                    var horizontalCollisionTest = Math.Min(newSourceBoundingBox.Bottom, targetBoundingBox.Bottom) -
                                                  Math.Max(newSourceBoundingBox.Top, targetBoundingBox.Top);

                    // Set the appropriate flags according to the direction of collision (if a collision occured)
                    if (horizontalCollisionTest > 0)
                    {
                        collisionEdge = sourceCollisionEdges & Direction.Horizontal;
                    }
                    else if (horizontalCollisionTest == 0)
                    {
                        collisionEdge = sourceCollisionEdges;
                    }
                }
            }

            // Initialize the collision information, if a collision occurred
            if (collisionEdge != Direction.None)
            {
                // Calculate the target direction of collision
                var targetEdge = Direction.None;
                if (collisionEdge.HasFlag(Direction.Left)) targetEdge |= Direction.Right;
                if (collisionEdge.HasFlag(Direction.Right)) targetEdge |= Direction.Left;
                if (collisionEdge.HasFlag(Direction.Top)) targetEdge |= Direction.Bottom;
                if (collisionEdge.HasFlag(Direction.Bottom)) targetEdge |= Direction.Top;

                // Round off the collision offset to keep it from bouncing
                var collisionOffset = new Vector2((float)Math.Round(distance.X), (float)Math.Round(distance.Y));

                // Set the collision info
                sourceInfo = new CollisionInfo(source, target, collisionEdge, collisionOffset);
                targetInfo = new CollisionInfo(target, source, targetEdge, Vector2.Zero);

                // A collision occurred
                return true;
            }
            else
            {
                // No collision occurred
                return false;
            }
        }
        */

        //static bool PerPixelCollision(ICollidable source, ICollidable target)
        //{
        //    // Get Color data of each Texture
        //    Color[] sourceColors = new Color[source.BoundingBox.Width * source.BoundingBox.Height];
        //    source.Texture.GetData(sourceColors);
        //    Color[] targetColors = new Color[target.BoundingBox.Width * target.BoundingBox.Height];
        //    target.Texture.GetData(targetColors);

        //    // Calculate the intersecting rectangle
        //    int x1 = Math.Max(source.BoundingBox.X, target.BoundingBox.X);
        //    int x2 = Math.Min(source.BoundingBox.X + source.BoundingBox.Width, target.BoundingBox.X + target.BoundingBox.Width);

        //    int y1 = Math.Max(source.BoundingBox.Y, target.BoundingBox.Y);
        //    int y2 = Math.Min(source.BoundingBox.Y + source.BoundingBox.Height, target.BoundingBox.Y + target.BoundingBox.Height);

        //    // For each single pixel in the intersecting rectangle
        //    for (int y = y1; y < y2; ++y)
        //    {
        //        for (int x = x1; x < x2; ++x)
        //        {
        //            // Get the color from each texture
        //            Color a = sourceColors[(x - source.BoundingBox.X) + (y - source.BoundingBox.Y) * source.BoundingBox.Width];
        //            Color b = targetColors[(x - target.BoundingBox.X) + (y - target.BoundingBox.Y) * target.BoundingBox.Width];

        //            if (a.A != 0 && b.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    // If no collision occurred by now, we're clear.
        //    return false;
        //}

        #region CollisionManagerScope Class

        /// <summary>
        /// Provides a mechanism for handling collision management context scopes.
        /// </summary>
        private class CollisionManagerScope : IDisposable
        {
            private CollisionManager _previousCollisionManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="CollisionManagerScope" /> class.
            /// </summary>
            /// <param name="bounds">The bounds.</param>
            /// <param name="collisionLayer">The collision layer.</param>
            public CollisionManagerScope(Rectangle? bounds = null, Layer collisionLayer = null)
            {
                this._previousCollisionManager = CollisionManager._collisionManager;

                CollisionManager._collisionManager = bounds.HasValue ? new CollisionManager(bounds.Value, collisionLayer) : null;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            void IDisposable.Dispose()
            {
                CollisionManager._collisionManager = this._previousCollisionManager;
            }
        }

        #endregion
    }
}