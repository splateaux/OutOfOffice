using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// A Heads-up display
    /// </summary>
    public class Hud
    {
        private SpriteFont _hudFont;
        private Texture2D _hudTexture;
        private ScreenManager _manager;

        private Color _timeColor;
        private string _timeString;
        private TimeSpan _warningTime = TimeSpan.FromSeconds(30);

        private string _scoreString;
        private Color _defaultFontColor = new Color(103, 143, 192);
        private Color _warningColor = Color.Red;

        private string _message;

        private Rectangle _hudRectangle;
        private Vector2 _scoreLocation;
        private Vector2 _timeLocation;
        private Vector2 _messageLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hud"/> class.
        /// </summary>
        /// <param name="screenManager">The screen manager.</param>
        public Hud(ScreenManager screenManager)
        {
            this._manager = screenManager;
        }

        /// <summary>
        /// Gets or sets the message to display in the HUD, setting it to blank is fine too
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            // Load fonts
            this._hudFont = content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            this._hudTexture = content.Load<Texture2D>("Textures/Overlays/HUD");

            // Setup our static locations
            this._hudRectangle = _manager.GraphicsDevice.Viewport.Bounds;
            this._timeLocation = new Vector2(_hudRectangle.Center.X - 30, _manager.GraphicsDevice.Viewport.Bounds.Height - 30);
            this._scoreLocation = new Vector2(_hudRectangle.Center.X + 480, _timeLocation.Y);
            this._messageLocation = new Vector2(40, _timeLocation.Y);
        }

        /// <summary>
        /// Updates the HUD
        /// </summary>
        /// <param name="player">The player.</param>
        public void Update(Player player)
        {
            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            _timeString = string.Format("{0}:{1}", player.TimeToLive.Minutes.ToString("00"), player.TimeToLive.Seconds.ToString("00"));

            if (player.TimeToLive < _warningTime &&
                (int)player.TimeToLive.TotalSeconds % 2 == 0)
            {
                _timeColor = _warningColor;
            }
            else
            {
                _timeColor = _defaultFontColor;
            }

            _scoreString = player.Score.ToString();
        }

        /// <summary>
        /// Draws the HUD
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Draw the Hud overlay.
            spriteBatch.Draw(this._hudTexture, this._hudRectangle, Color.White);

            // Draw time
            DrawingUtils.DrawShadowedString(spriteBatch, this._hudFont, _timeString, _timeLocation, _timeColor);

            // Draw score
            DrawingUtils.DrawShadowedString(spriteBatch, this._hudFont, string.IsNullOrEmpty(_scoreString) ? "$0.00" : String.Format("${0}", _scoreString), _scoreLocation, _defaultFontColor);

            // IF there's a message, show it
            if (!string.IsNullOrEmpty(_message))
            {
                DrawingUtils.DrawShadowedString(spriteBatch, this._hudFont, _message, _messageLocation, _defaultFontColor);
            }

            spriteBatch.End();
        }
    }
}