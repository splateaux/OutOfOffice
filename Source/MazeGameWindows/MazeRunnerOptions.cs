using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGame
{
    /// <summary>
    /// Required options for a mazeRunner
    /// </summary>
    public class MazeRunnerOptions
    {
        private Maze _maze;
        private Texture2D _texture;
        private int _dollarValue;
        private SoundEffect _dollarSound;

        /// <summary>
        /// Gets the dollar sound.
        /// </summary>
        /// <value>
        /// The dollar sound.
        /// </value>
        public SoundEffect DollarSound
        {
            get { return _dollarSound; }
        }

        /// <summary>
        /// Gets the dollar value.
        /// </summary>
        /// <value>
        /// The dollar value.
        /// </value>
        public int DollarValue
        {
            get { return _dollarValue; }
        }

        /// <summary>
        /// Gets the maze.
        /// </summary>
        /// <value>
        /// The maze.
        /// </value>
        public Maze Maze
        {
            get { return _maze; }
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeRunnerOptions"/> class.
        /// </summary>
        /// <param name="maze">The maze.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="dollarSound">The dollar sound.</param>
        /// <param name="dollarValue">The dollar value.</param>
        public MazeRunnerOptions(
            Maze maze,
            Texture2D texture,
            SoundEffect dollarSound,
            int dollarValue)
        {
            _maze = maze;
            _texture = texture;
            _dollarSound = dollarSound;
            _dollarValue = dollarValue;
        }

    }
}
