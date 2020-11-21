using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents a hierarchical tree that manages objects in close
    /// proximity to one another.  This class helps the collision manager
    /// to limit the number of collision determination that need
    /// to occur.
    /// </summary>
    public class Quadtree
    {
        private const int MAX_OBJECTS = 10; // Defines how many objects a node can hold before it splits
        private int MAX_LEVELS = 5; // The deepest level sub-node

        private static readonly object _insertEventKey = new object();
        private static readonly object _removeEventKey = new object();

        private readonly HashSet<ICollidable> _objects;
        private readonly Dictionary<ICollidable, HashSet<Quadtree>> _objectLookup;
        private readonly EventHandlerList _eventDelegates;
        private readonly HashSet<ICollidable> _pendingRemovals;

        private int _level;
        private Rectangle _bounds;
        private Quadtree[] _nodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadtree" /> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public Quadtree(Rectangle bounds)
            : this(bounds, 0, new HashSet<ICollidable>(), new Dictionary<ICollidable, HashSet<Quadtree>>(), new EventHandlerList())
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadtree" /> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="level">The level.</param>
        /// <param name="pendingRemovals">The pending removals.</param>
        /// <param name="lookups">The lookups.</param>
        /// <param name="eventDelegates">The event delegates.</param>
        private Quadtree(Rectangle bounds, int level,
            HashSet<ICollidable> pendingRemovals,
            Dictionary<ICollidable, HashSet<Quadtree>> lookups,
            EventHandlerList eventDelegates)
        {
            this._bounds = bounds;
            this._level = level;
            this._pendingRemovals = pendingRemovals;
            this._objectLookup = lookups;
            this._eventDelegates = eventDelegates;
            this._objects = new HashSet<ICollidable>();
            this._nodes = new Quadtree[4];
        }

        /// <summary>
        /// Clears all the objects from the <see cref="Quadtree"/>.
        /// </summary>
        public void Clear()
        {
            this._objects.Clear();

            for (int i = 0; i < this._nodes.Length; i++)
            {
                if (this._nodes[i] != null)
                {
                    this._nodes[i].Clear();
                    this._nodes[i] = null;
                }
            }

            this._objectLookup.Clear();
        }

        /// <summary>
        /// Insert the object into the <see cref="Quadtree"/>. If the node
        /// exceeds the capacity, it will split and add all
        /// objects to their corresponding nodes.
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        public void Insert(ICollidable collidableObject)
        {
            if (this._nodes[0] != null)
            {
                foreach (int quadrant in this.GetQuadrant(collidableObject))
                {
                    this._nodes[quadrant].Insert(collidableObject);
                }
                return;
            }

            this._objects.Add(collidableObject);

            if (!this._objectLookup.ContainsKey(collidableObject))
            {
                this._objectLookup[collidableObject] = new HashSet<Quadtree>();
            }

            this._objectLookup[collidableObject].Add(this);

            if ((this._objects.Count > MAX_OBJECTS) && (this._level < MAX_LEVELS))
            {
                if (this._nodes[0] == null)
                {
                    this.Split();
                }

                foreach (var existingCollidableObject in this._objects.ToArray())
                {
                    this._objects.Remove(existingCollidableObject);
                    _objectLookup[existingCollidableObject].Remove(this);

                    foreach (int quadrant in this.GetQuadrant(existingCollidableObject))
                    {
                        this._nodes[quadrant].Insert(existingCollidableObject);
                    }
                }
            }

            this.OnItemInserted(new QuadtreeItemEventArgs(collidableObject));
        }

        /// <summary>
        /// Removes the specified collidable object.
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        public void Remove(ICollidable collidableObject)
        {
            HashSet<Quadtree> trees;
            if (this._objectLookup.TryGetValue(collidableObject, out trees))
            {
                foreach (var tree in trees)
                {
                    tree._objects.Remove(collidableObject);
                }

                this._objectLookup.Remove(collidableObject);

                this.OnItemRemoved(new QuadtreeItemEventArgs(collidableObject));
            }
        }

        /// <summary>
        /// Updates the specified collidable object within the <see cref="Quadtree"/>.
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        public void Update(ICollidable collidableObject)
        {
            this.Remove(collidableObject);
            this.Insert(collidableObject);
        }

        /// <summary>
        /// Return all objects that could collide with the given object
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        /// <returns>A list of possible collisions.</returns>
        public List<ICollidable> Retrieve(ICollidable collidableObject)
        {
            var possibleCollisions = new List<ICollidable>();

            // Use the dictionary to find all the objects
            HashSet<Quadtree> trees;
            if (this._objectLookup.TryGetValue(collidableObject, out trees))
            {
                possibleCollisions = trees.SelectMany(t => t._objects)
                                          .Distinct()
                                          .Where(o => o != collidableObject)
                                          .ToList();
            }

            // Return the results
            return possibleCollisions;
        }

        /// <summary>
        /// Queues the removal of a specific object.
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        public void QueuePendingRemoval(ICollidable collidableObject)
        {
            if (this._objectLookup.ContainsKey(collidableObject))
            {
                this._pendingRemovals.Add(collidableObject);
            }
        }

        /// <summary>
        /// Applies the pending removals.
        /// </summary>
        public void ApplyPendingRemovals()
        {
            foreach (var collidableObject in this._pendingRemovals)
            {
                this.Remove(collidableObject);
            }

            this._pendingRemovals.Clear();
        }

        /// <summary>
        /// Occurs when an item is inserted into the tree.
        /// </summary>
        public event EventHandler<QuadtreeItemEventArgs> ItemInserted
        {
            add
            {
                this._eventDelegates.AddHandler(_insertEventKey, value);
            }
            remove
            {
                this._eventDelegates.RemoveHandler(_insertEventKey, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ItemInserted" /> event.
        /// </summary>
        /// <param name="e">The <see cref="QuadtreeItemEventArgs"/> instance containing the event data.</param>
        protected virtual void OnItemInserted(QuadtreeItemEventArgs e)
        {
            var handler = (EventHandler<QuadtreeItemEventArgs>)this._eventDelegates[_insertEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when item is removed from the tree.
        /// </summary>
        public event EventHandler<QuadtreeItemEventArgs> ItemRemoved
        {
            add
            {
                this._eventDelegates.AddHandler(_removeEventKey, value);
            }
            remove
            {
                this._eventDelegates.RemoveHandler(_removeEventKey, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ItemRemoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="QuadtreeItemEventArgs" /> instance containing the event data.</param>
        protected virtual void OnItemRemoved(QuadtreeItemEventArgs e)
        {
            var handler = (EventHandler<QuadtreeItemEventArgs>)this._eventDelegates[_removeEventKey];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Splits this instance into four sub-nodes by dividing the node into
        /// four equal parts and initializing the four sub-nodes with the new
        /// bounds.
        /// </summary>
        private void Split()
        {
            var subWidth = this._bounds.Width / 2;
            var subHeight = this._bounds.Height / 2;
            var x = this._bounds.X;
            var y = this._bounds.Y;

            this._nodes[0] = new Quadtree(new Rectangle(x + subWidth, y, subWidth, subHeight), this._level + 1, this._pendingRemovals, this._objectLookup, this._eventDelegates);
            this._nodes[1] = new Quadtree(new Rectangle(x, y, subWidth, subHeight), this._level + 1, this._pendingRemovals, this._objectLookup, this._eventDelegates);
            this._nodes[2] = new Quadtree(new Rectangle(x, y + subHeight, subWidth, subHeight), this._level + 1, this._pendingRemovals, this._objectLookup, this._eventDelegates);
            this._nodes[3] = new Quadtree(new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight), this._level + 1, this._pendingRemovals, this._objectLookup, this._eventDelegates);
        }

        /// <summary>
        /// Determines where an object belongs in the
        /// <see cref="Quadtree" /> by
        /// determining which node the object can fit into.
        /// </summary>
        /// <param name="collidableObject">The collidable object.</param>
        /// <returns></returns>
        private List<int> GetQuadrant(ICollidable collidableObject)
        {
            var indexes = new List<int>();
            var verticalMidpoint = this._bounds.X + (this._bounds.Width / 2d);
            var horizontalMidpoint = this._bounds.Y + (this._bounds.Height / 2d);
            var boundingBox = collidableObject.BoundingBox;

            // Object can completely fit within the top quadrants
            var topQuadrant = ((boundingBox.Y + boundingBox.Height) < horizontalMidpoint);

            // Object can completely fit within the bottom quadrants
            var bottomQuadrant = (boundingBox.Y > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if ((boundingBox.X + boundingBox.Width) < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indexes.Add(1);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                }
                else
                {
                    indexes.Add(1);
                    indexes.Add(2);
                }
            }
            // Object can completely fit within the right quadrants
            else if (boundingBox.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    indexes.Add(0);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(3);
                }
                else
                {
                    indexes.Add(0);
                    indexes.Add(3);
                }
            }
            // Object is between the left and right quadrants
            else
            {
                if (topQuadrant)
                {
                    indexes.Add(0);
                    indexes.Add(1);
                }
                else if (bottomQuadrant)
                {
                    indexes.Add(2);
                    indexes.Add(3);
                }
                else
                {
                    indexes.Add(0);
                    indexes.Add(1);
                    indexes.Add(2);
                    indexes.Add(3);
                }
            }

            return indexes;
        }
    }

    /// <summary>
    /// Defines data that can be used to identify an item that was added or removed.
    /// </summary>
    public class QuadtreeItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadtreeItemEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public QuadtreeItemEventArgs(ICollidable item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public ICollidable Item { get; private set; }
    }
}