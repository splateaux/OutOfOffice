using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputController;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MazeGame
{
    /// <summary>
    /// A runner of in a maze
    /// </summary>
    public class MazeRunner
    {
        private Texture2D _texture;
        private Cell _currentCell;
        private NintendoController _nintendoController;
        private Maze _maze;
        private Direction _validDirections = Direction.All;
        private NintendoControllerState _controllerState;
        private KeyboardState _keyboardState;
        private SoundEffect _dollarSound;
        private int _dollarValue;
        private int _points;
        private bool _endReached;

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeRunner"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="maze">The maze.</param>
        public MazeRunner(Game game, MazeRunnerOptions options)
        { 
            this._nintendoController = new NintendoController(game);
            this._maze = options.Maze;
            this._texture = options.Texture;
            this._dollarSound = options.DollarSound;
            this._dollarValue = options.DollarValue;

            this._currentCell = this._maze.StartingCell;
        }

        /// <summary>
        /// Gets the current cell.
        /// </summary>
        /// <value>
        /// The current cell.
        /// </value>
        public Cell CurrentCell
        {
            get { return _currentCell; }
        }

        /// <summary>
        /// Gets a value indicating whether [end reached].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [end reached]; otherwise, <c>false</c>.
        /// </value>
        public bool EndReached
        {
            get { return _endReached; }
        }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public int Points
        {
            get { return _points; }
            set { _points = value; }
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            // duh?
            if (_endReached) return;

            _controllerState = _nintendoController.GetState();
            _keyboardState = Keyboard.GetState();

            ClearInputs();
            Direction? destinationDirection = GetInput();
            Cell destinationCell = null;

            if (destinationDirection.HasValue)
            {
                if (_currentCell.IsEnd && destinationDirection.Value == Direction.Up)
                {
                    _endReached = true;
                    return;
                }

                destinationCell = _maze.GetDestinationCell(_currentCell, destinationDirection.Value);
            }
            
            if (destinationCell != null)
            {
                _currentCell = destinationCell;
            }

            if (_currentCell.HasDollar)
            {
                this._dollarSound.Play();
                this._currentCell.RemoveDollar();
                this._points += _dollarValue;
            }

        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _currentCell.BoundingBox, Color.White);
        }

        /// <summary>
        /// Gets the input.
        /// </summary>
        /// <returns></returns>
        private Direction? GetInput()
        {
            // Pressing down, there's no wall there, and we're not the starting cell...
            if ((_controllerState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                _keyboardState.IsKeyDown(Keys.Down)) &&
                _validDirections.HasFlag(Direction.Down) &&
                !_currentCell.Walls.HasFlag(Direction.Down) &&
                !_currentCell.IsStart)
            {
                _validDirections &= ~Direction.Down;
                return Direction.Down;
            }

            // Pressing up, there's no wall there, and we're not the ending cell...
            if ((_controllerState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                _keyboardState.IsKeyDown(Keys.Up))  &&
                _validDirections.HasFlag(Direction.Up) &&
                !_currentCell.Walls.HasFlag(Direction.Up))
            {
                _validDirections &= ~Direction.Up;
                return Direction.Up;
            }

            // Pressing left, there's no wall there
            if ((_controllerState.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                _keyboardState.IsKeyDown(Keys.Left))  &&
                _validDirections.HasFlag(Direction.Left) &&
                !_currentCell.Walls.HasFlag(Direction.Left))
            {
                _validDirections &= ~Direction.Left;
                return Direction.Left;
            }

            // Pressing right, there's no wall there
            if ((_controllerState.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed ||
                _keyboardState.IsKeyDown(Keys.Right)) &&
                _validDirections.HasFlag(Direction.Right) &&
                !_currentCell.Walls.HasFlag(Direction.Right))
            {
                _validDirections &= ~Direction.Right;
                return Direction.Right;
            }

            return null;
        }

        /// <summary>
        /// Clears any directions which have already been used, IF their direction is no longer pressed
        /// </summary>
        private void ClearInputs()
        {
            if (_controllerState.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Released && _keyboardState.IsKeyUp(Keys.Down))
            {
                _validDirections |= Direction.Down;
            }

            // Pressing up, there's no wall there, and we're not the ending cell...
            if (_controllerState.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Released && _keyboardState.IsKeyUp(Keys.Up))
            {
                _validDirections |= Direction.Up;
            }

            // Pressing left, there's no wall there
            if (_controllerState.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Released && _keyboardState.IsKeyUp(Keys.Left))
            {
                _validDirections |= Direction.Left;
            }

            // Pressing right, there's no wall there
            if (_controllerState.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Released && _keyboardState.IsKeyUp(Keys.Right))
            {
                _validDirections |= Direction.Right;
            }            
        }

    }
}
