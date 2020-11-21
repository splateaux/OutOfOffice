using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGame
{
    /// <summary>
    /// Required maze options
    /// </summary>
    public class MazeOptions
    {
        private Vector2 _startingVector;
        private int _width;
        private Color _wallColor;
        private int _height;
        private int _cellSize;
        private int _cellWallWidth;
        private Texture2D _dollarTexture;


        /// <summary>
        /// Gets the width of the cell wall.
        /// </summary>
        /// <value>
        /// The width of the cell wall.
        /// </value>
        public int CellWallWidth
        {
            get { return _cellWallWidth; }
            set { _cellWallWidth = value; }
        }

        /// <summary>
        /// Gets the size of the cell.
        /// </summary>
        /// <value>
        /// The size of the cell.
        /// </value>
        public int CellSize
        {
            get { return _cellSize; }
            set { _cellSize = value; }
        }

        /// <summary>
        /// Gets the color of the wall.
        /// </summary>
        /// <value>
        /// The color of the wall.
        /// </value>
        public Color WallColor
        {
            get { return _wallColor; }
            set { _wallColor = value; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get { return _height; }
        }


        /// <summary>
        /// Gets the dollar texture.
        /// </summary>
        /// <value>
        /// The dollar texture.
        /// </value>
        public Texture2D DollarTexture
        {
            get { return _dollarTexture; }
        }


        /// <summary>
        /// Gets the starting vector.
        /// </summary>
        /// <value>
        /// The starting vector.
        /// </value>
        public Vector2 StartingVector
        {
            get { return _startingVector; }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeOptions"/> class.
        /// </summary>
        /// <param name="startingVector">The starting vector.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MazeOptions(
            Vector2 startingVector,
            int width,
            int height,
            Texture2D dollarTexture)
        {
            _startingVector = startingVector;
            _width = width;
            _height = height;
            _dollarTexture = dollarTexture;

            _wallColor = Color.White;
            _cellSize = 40;
            _cellWallWidth = 4;
        }

    }
}
