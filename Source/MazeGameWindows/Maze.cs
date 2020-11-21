using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputController;
using Microsoft.Xna.Framework.Content;

namespace MazeGame
{
    public class Maze
    {
        private Cell[,] _cells;
        private int _height;
        private int _width;
        private Texture2D _texture;
        private Texture2D _dollarTexture;
        private GraphicsDevice _device;
        private Cell _startingCell;
        private int _cellSize;
        private int _cellWallWidth;
        private Vector2 _startingVector;


        /// <summary>
        /// Initializes a new instance of the <see cref="Maze"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Maze(GraphicsDevice device, MazeOptions options)
        {
            _texture = new Texture2D(device, 1, 1);
            _texture.SetData<Color>(new Color[] { options.WallColor });
            _device = device;
            _dollarTexture = options.DollarTexture;

            _height = options.Height;
            _width = options.Width;
            _cellSize = options.CellSize;
            _cellWallWidth = options.CellWallWidth;
            _startingVector = options.StartingVector;

            GenerateMaze();
        }

        /// <summary>
        /// Gets the starting cell.
        /// </summary>
        /// <value>
        /// The starting cell.
        /// </value>
        internal Cell StartingCell
        {
            get { return _startingCell; }
        }


        /// <summary>
        /// Gets the destination cell.
        /// </summary>
        /// <param name="startingCell">The starting cell.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public Cell GetDestinationCell(Cell startingCell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return _cells[startingCell.X, startingCell.Y - 1];
                case Direction.Right:
                    return _cells[startingCell.X + 1, startingCell.Y];
                case Direction.Down:
                    return _cells[startingCell.X, startingCell.Y + 1];
                case Direction.Left:
                    return _cells[startingCell.X - 1, startingCell.Y];
            }

            return null;
        }

        /// <summary>
        /// Generates the maze.
        /// </summary>
        private void GenerateMaze()
        {
            _cells = new Cell[_width, _height];
            Random rand = new Random();

            for (int w = 0; w < _width; w++)
                for (int h = 0; h < _height; h++)
                {
                    _cells[w, h] = new Cell(_texture, w, h, _startingVector, _cellSize, _cellWallWidth);                    
                }


            Stack<Cell> cellStack = new Stack<Cell>();
            int totalCells = _width * _height;
            Cell currentCell = _cells[rand.Next(_width), rand.Next(_height)]; 
            int vistedCells = 1;
            List<Cell> neighbors;
            Cell newCell;

            while (vistedCells < totalCells)
            {
                neighbors = GetSolidNeighbors(currentCell);

                if (neighbors.Count > 0)
                {
                    newCell = neighbors[rand.Next(neighbors.Count)];
                    if (currentCell.X == newCell.X)
                    {
                        if (currentCell.Y > newCell.Y)
                        {
                            // Up
                            currentCell.Walls &= ~Direction.Up;
                            newCell.Walls &= ~Direction.Down;
                        }
                        else
                        {
                            //down
                            currentCell.Walls &= ~Direction.Down;
                            newCell.Walls &= ~Direction.Up;
                        }
                    }
                    else
                    {
                        if (currentCell.X > newCell.X)
                        {
                            // Left
                            currentCell.Walls &= ~Direction.Left;
                            newCell.Walls &= ~Direction.Right;
                        }
                        else
                        { 
                            // Right
                            currentCell.Walls &= ~Direction.Right;
                            newCell.Walls &= ~Direction.Left;
                        }
                    }

                    cellStack.Push(currentCell);
                    currentCell = newCell;
                    vistedCells++;
                }
                else
                {
                    currentCell = cellStack.Pop();
                }
            }

            // Set start/end, currently hardcoded as bottom left and top right
            _startingCell = _cells[0, _height - 1];
            _startingCell.IsStart = true;
            _startingCell.Walls &= ~Direction.Down;
            _cells[_width - 1, 0].IsEnd = true;
            _cells[_width - 1, 0].Walls &= ~Direction.Up;

            AddDollars();
        }

        /// <summary>
        /// Dolla dolla bills ya'll!
        /// </summary>
        private void AddDollars()
        {
            Random rand = new Random();

            foreach (Cell cell in _cells)
            { 
                if (rand.Next(0, 30) == 1 || 
                    (cell.Walls.HasFlag(Direction.Down) && cell.Walls.HasFlag(Direction.Left) && cell.Walls.HasFlag(Direction.Up)) ||
                    (cell.Walls.HasFlag(Direction.Left) && cell.Walls.HasFlag(Direction.Up) && cell.Walls.HasFlag(Direction.Right)) ||
                    (cell.Walls.HasFlag(Direction.Up) && cell.Walls.HasFlag(Direction.Right) && cell.Walls.HasFlag(Direction.Down)) ||
                    (cell.Walls.HasFlag(Direction.Right) && cell.Walls.HasFlag(Direction.Down) && cell.Walls.HasFlag(Direction.Left)))
                {
                    cell.DollarTexture =_dollarTexture;
                }
            }
        }

        /// <summary>
        /// Returns all the adjacent cells that have all of their walls intact
        /// </summary>
        /// <param name="startingCell">The starting cell.</param>
        /// <returns></returns>
        private List<Cell> GetSolidNeighbors(Cell startingCell)
        {
            List<Cell> neighbors = new List<Cell>();

            // Left neighbor
            if (startingCell.X > 0 && _cells[startingCell.X - 1, startingCell.Y].Walls == Direction.All)
            {
                neighbors.Add(_cells[startingCell.X - 1, startingCell.Y]);
            }

            // Right neighbor
            if (startingCell.X < (_width - 1) && _cells[startingCell.X + 1, startingCell.Y].Walls == Direction.All)
            {
                neighbors.Add(_cells[startingCell.X + 1, startingCell.Y]);
            }

            // Top neighbor
            if (startingCell.Y > 0 && _cells[startingCell.X, startingCell.Y - 1].Walls == Direction.All)
            {
                neighbors.Add(_cells[startingCell.X, startingCell.Y - 1]);
            }

            // Bottom neighbor
            if (startingCell.Y < (_height - 1) && _cells[startingCell.X, startingCell.Y + 1].Walls == Direction.All)
            {
                neighbors.Add(_cells[startingCell.X, startingCell.Y + 1]);
            }

            return neighbors;
        }


        /// <summary>
        /// Draws the Maze
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Cell cell in _cells)
            {
                cell.Draw(spriteBatch);
            }
        }
    }
}
