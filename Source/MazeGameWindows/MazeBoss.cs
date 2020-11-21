using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGame
{
    /// <summary>
    /// The maze boss...  character that walks accross the top to the conference room
    /// </summary>
    public class MazeBoss
    {
        private Texture2D _texture;
        private Texture2D _conferenceRoom;
        private Rectangle _boundingBox;
        private Rectangle _conferenceRoomBoundingBox;
        private bool _isAtDestination;
        private Vector2 _velocity;
        private Vector2 _endingPosition;
        private Vector2 _currentPosition;
        private TimeSpan _travelTime;
        private float _speed;


        /// <summary>
        /// Gets a value indicating whether this instance is at destination.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is at destination; otherwise, <c>false</c>.
        /// </value>
        public bool IsAtDestination
        {
            get { return _isAtDestination; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeBoss"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="startingPosition">The starting position.</param>
        /// <param name="endingPosition">The ending position.</param>
        /// <param name="travelTime">The travel time.</param>
        /// <param name="conferenceRoom">The conference room.</param>
        public MazeBoss(Texture2D texture, Vector2 startingPosition, Vector2 endingPosition, TimeSpan travelTime, Texture2D conferenceRoom)
        {
            _texture = texture;
            _currentPosition = startingPosition;
            _endingPosition = endingPosition;
            _travelTime = travelTime;
            _conferenceRoom = conferenceRoom;

            _conferenceRoomBoundingBox = new Rectangle(Convert.ToInt32(endingPosition.X), Convert.ToInt32(endingPosition.Y), conferenceRoom.Width, conferenceRoom.Height);

            // Figure out how fast we need to travel to get there in time
            float distanceToTravel = endingPosition.X - startingPosition.X;
            _speed = distanceToTravel / (Convert.ToInt32(Math.Round(travelTime.TotalSeconds)));
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            if (Math.Round(_currentPosition.X, 0) == _endingPosition.X)
            {
                _isAtDestination = true;
                return;
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _velocity = Vector2.Normalize(_endingPosition - _currentPosition);
            _velocity *= _speed;

            _currentPosition += _velocity * elapsed;
            _boundingBox = new Rectangle(Convert.ToInt32(_currentPosition.X), Convert.ToInt32(_currentPosition.Y), _texture.Width, _texture.Height);
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_conferenceRoom, _conferenceRoomBoundingBox, Color.White);
            spriteBatch.Draw(_texture, _boundingBox, Color.White);
        }
    }
}
