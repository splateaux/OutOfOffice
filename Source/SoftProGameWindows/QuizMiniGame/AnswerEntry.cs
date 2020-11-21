using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuizGameDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// A menuEntry specifically setup for quiz answers
    /// </summary>
    class AnswerEntry : MenuEntry
    {
        private bool _isCorrect;
        private string _text;
        private float _selectionFade;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerEntry"/> class.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public AnswerEntry(Answer answer)
            : base(answer.AnswerText)
        {
            _text = answer.AnswerText;
            _isCorrect = answer.isCorrect;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is correct.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is correct; otherwise, <c>false</c>.
        /// </value>
        public bool IsCorrect
        {
            get { return _isCorrect; }
        }

        /// <summary>
        /// Updates the specified is selected.
        /// </summary>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="gameTime">The game time.</param>
        public void Update(bool isSelected, GameTime gameTime)
        {
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
        public virtual void Draw(QuizGameScreen screen, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? this.SelectedColor : this.UnselectedColor;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, this._text, this.Position, color, 0,
                                   origin, 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns></returns>
        public int GetWidth(QuizGameScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns></returns>
        public int GetHeight(QuizGameScreen screen)
        {
            int lineNumbers = _text.Count(c => c == '\n') + 1;
            return screen.ScreenManager.Font.LineSpacing * lineNumbers;
        }
    }
}
