using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGame
{
    /// <summary>
    /// Represents a "cell" in the overall grid in a mze
    /// </summary>
    public class Cell
    {
        private Texture2D _texture;
        private Texture2D _dollarTexture;
        private Direction _walls;
        private int _x;
        private int _y;
        private Vector2 _position;
        private Rectangle _boundingBox;
        private int _wallWidth;
        private int _cellSize;

        /// <summary>
        /// Is this the starting cell?
        /// </summary>
        public bool IsStart;

        /// <summary>
        /// Is this the ending cell?
        /// </summary>
        public bool IsEnd;


        /// <summary>
        /// Gets a value indicating whether this instance has dollar.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has dollar; otherwise, <c>false</c>.
        /// </value>
        public bool HasDollar
        {
            get
            {
                return _dollarTexture != null;
            }
        }

        /// <summary>
        /// Gets or sets the dollar texture (setting to null 'default' removes the dollar)
        /// </summary>
        /// <value>
        /// The dollar texture.
        /// </value>
        public Texture2D DollarTexture
        {
            get { return _dollarTexture; }
            set { _dollarTexture = value; }
        }

        /// <summary>
        /// Gets the x
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets or sets the walls on the cell
        /// </summary>
        /// <value>
        /// The walls.
        /// </value>
        public Direction Walls
        {
            get { return _walls; }
            set { _walls = value; }
        }

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public Rectangle BoundingBox
        {
            get { return _boundingBox; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Cell(Texture2D texture, int x, int y, Vector2 startingPosition, int cellSize, int wallWidth)
        {
            _wallWidth = wallWidth;
            _cellSize = cellSize;
            _x = x;
            _y = y;
            _texture = texture;
            _position = new Vector2(((x + startingPosition.X) * _cellSize), ((y + startingPosition.Y) * _cellSize));
            _walls = Direction.All;
            _boundingBox = new Rectangle(Convert.ToInt32(_position.X), Convert.ToInt32(_position.Y), _cellSize, _cellSize);
        }

        /// <summary>
        /// Removes the dollar.
        /// </summary>
        public void RemoveDollar()
        {
            _dollarTexture = null;
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // A "cell" is really just made up of it's walls that are still up
            if (Walls.HasFlag(Direction.Up))
            {
                DrawLine(spriteBatch,
                    _position,
                    new Vector2(_position.X + _cellSize, _position.Y));
            }

            if (Walls.HasFlag(Direction.Right))
            {
                DrawLine(spriteBatch,
                    new Vector2(_position.X + _cellSize, _position.Y),
                    new Vector2(_position.X + _cellSize, _position.Y + _cellSize));
            }

            if (Walls.HasFlag(Direction.Down))
            {
                DrawLine(spriteBatch,
                    new Vector2(_position.X, _position.Y + _cellSize),
                    new Vector2(_position.X + _cellSize, _position.Y + _cellSize));
            }

            if (Walls.HasFlag(Direction.Left))
            {
                DrawLine(spriteBatch,
                    new Vector2(_position.X, _position.Y),
                    new Vector2(_position.X, _position.Y + _cellSize));
            }

            if (_dollarTexture != null)
            { 
                spriteBatch.Draw(_dollarTexture, new Rectangle(_boundingBox.X, _boundingBox.Y + _cellSize/4, _boundingBox.Width, _dollarTexture.Height), Color.White);
            }

        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="sb">The spritebatch to draw this line as part of</param>
        /// <param name="start">starting vector</param>
        /// <param name="end">ending vector</param>
        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;

            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            sb.Draw(_texture,
                new Rectangle((int)start.X, (int)start.Y,
                    (int)edge.Length(),
                    _wallWidth), 
                null,
                Color.White, //colour of line
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0);
        }

    }

    /// <summary>
    /// Direction... it's a thing!
    /// </summary>
    public enum Direction
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
        All = Up | Right | Down | Left
    }
}
