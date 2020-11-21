using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        private string _text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        /// <summary>
        /// Gets or sets the color of the selected menu entry.
        /// </summary>
        /// <value>
        /// The color of the selected.
        /// </value>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the unselected menu entries.
        /// </summary>
        /// <value>
        /// The color of the unselected.
        /// </value>
        public Color UnselectedColor { get; set; }

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public MenuEntry(string text)
        {
            this._text = text;
            this.SelectedColor = Color.Yellow;
            this.UnselectedColor = Color.White;
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                this._selectionFade = Math.Min(this._selectionFade + fadeSpeed, 1);
            else
                this._selectionFade = Math.Max(this._selectionFade - fadeSpeed, 0);
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? this.SelectedColor : this.UnselectedColor;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * this._selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, this._text, this._position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns></returns>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns></returns>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }
    }
}