using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FridgeGame
{
    /// <summary>
    /// Basic timer for the quiz game... actually, I guess anyone could use it
    /// </summary>
    class MiniGameTimer
    {
        private SpriteFont _hudFont;
        private Color _timeColor;
        private string _timeString;
        private TimeSpan _warningTime = TimeSpan.FromSeconds(10);
        private Color _defaultFontColor = Color.Yellow;
        private Color _warningColor = Color.Red;
        private Vector2 _timeLocation;
        private TimeSpan _miniGameTime;
        private bool _timeIsUp;

        /// <summary>
        /// Gets or sets the quiz time.
        /// </summary>
        /// <value>
        /// The quiz time.
        /// </value>
        public TimeSpan MiniGameTime
        {
            get { return _miniGameTime; }
            set { _miniGameTime = value; }
        }

        /// <summary>
        /// Gets a value indicating whether [time is up].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [time is up]; otherwise, <c>false</c>.
        /// </value>
        public bool TimeIsUp
        {
            get { return _timeIsUp; }
        }

        /// <summary>
        /// Subtracts the supplied time from the remaining time
        /// </summary>
        /// <param name="timeLost">The time lost.</param>
        public void SubtractTime(TimeSpan timeLost)
        {
            _miniGameTime -= timeLost;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizTimer"/> class.
        /// </summary>
        /// <param name="quizTime">The quiz time.</param>
        public MiniGameTimer(TimeSpan quizTime)
        {
            _miniGameTime = quizTime;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="content">The content.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        public void LoadContent(SpriteFont font, ContentManager content, GraphicsDevice graphicsDevice)
        {
            this._hudFont = font;

            this._timeLocation = new Vector2(30, 20);
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            if (!_timeIsUp)
            {
                _miniGameTime -= gameTime.ElapsedGameTime;
                if (_miniGameTime.TotalSeconds <= 0)
                {
                    _timeIsUp = true;
                    return;
                }

                // Draw time remaining. Uses modulo division to cause blinking when the
                // player is running out of time.
                _timeString = string.Format("{0}", _miniGameTime.TotalSeconds.ToString("00"));

                if (_miniGameTime < _warningTime &&
                    (int)_miniGameTime.TotalSeconds % 2 == 0)
                {
                    _timeColor = _warningColor;
                }
                else
                {
                    _timeColor = _defaultFontColor;
                }

            }
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            DrawShadowedString(spriteBatch, this._hudFont, _timeString, _timeLocation, _timeColor);
            spriteBatch.End();
        }

        private void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
        {
            if (string.IsNullOrEmpty(value)) return;

            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
