using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using QuizGameDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SoftProGameWindows
{
    /// <summary>
    /// The quiz game
    /// </summary>
    class QuizGameScreen : MiniGameScreen
    {
        private Stack<Question> _questions;
        private Question _currentQuestion;
        private List<AnswerEntry> _menuEntries = new List<AnswerEntry>();
        private int _selectedEntry = 0;
        private SoundEffect _correctSound;
        private SoundEffect _incorrectSound;
        private int _correctAnswerPoints;
        private TimeSpan _incorrectAnswerTime;
        private QuizTimer _quizTimer;
        
        private const int MAX_QUESTION_LENGTH = 32;
        private const int MAX_ANSWER_LENGTH = 45;


        /// <summary>
        /// Give me an intro screen message!
        /// </summary>
        /// <value>
        /// The intro message.
        /// </value>
        protected override string IntroMessage
        {
            get 
            {
                StringBuilder introMessage = new StringBuilder();
                introMessage.AppendLine("Surprise pop quiz!");
                introMessage.AppendLine("Joyce wants to make sure we all know our stuff.");
                introMessage.AppendLine("You've got one minute to answer as many questions as you can.");
                introMessage.AppendLine();
                introMessage.AppendLine("Too scared?  You can cancel to try and make it to the shave event.");
                return introMessage.ToString();
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            Question[] questions = Content.Load<Question[]>("Data/QuizGameQuestions");
            _correctSound = Content.Load<SoundEffect>(Constants.SOFTPRODOLLAR_SOUND);
            _incorrectSound = Content.Load<SoundEffect>("Audio/Effects/Doh");
            _correctAnswerPoints = this.ScreenManager.PointManager.GetValue(Constants.QUIZ_CORRECT_ANSWER);
            _incorrectAnswerTime = new TimeSpan(0, 0, this.ScreenManager.PointManager.GetValue(Constants.QUIZ_INCORRECT_ANSWER));

            _questions = ShuffleAndTrimQuestions(questions.ToList());

            SetupNextQuestion();

            // PLayer doesn't exist yet, so defaulting to one minute, will adjust later if needed
            _quizTimer = new QuizTimer(new TimeSpan(0, 1, 0));

            _quizTimer.LoadContent(ScreenManager.Font, Content, ScreenManager.GraphicsDevice);
        }

        /// <summary>
        /// An "update" just for minigames.
        /// Only gets called if your mini game SHOULD be updating,
        /// because it's not paused, the intro is gone, etc..
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void MiniGameUpdate(Microsoft.Xna.Framework.GameTime gametime)
        {
            UpdateTimer(gametime);

            if (_quizTimer.TimeIsUp)
            {
                ExitScreen();
            }

            // Update each nested MenuEntry object.
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                bool isSelected = this.IsActive && (i == this._selectedEntry);

                this._menuEntries[i].Update(isSelected, gametime);
            }
        }

        /// <summary>
        /// Updates the timer.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        private void UpdateTimer(GameTime gametime)
        {
            if (_quizTimer.QuizTime > Player.TimeToLive)
            {
                _quizTimer.QuizTime = Player.TimeToLive - gametime.ElapsedGameTime;
            }

            _quizTimer.Update(gametime);
        }

        /// <summary>
        /// Called when a random sound should be played
        /// </summary>
        /// <param name="manager"></param>
        protected override void OnPlayRandomSound(SoundEffectManager manager)
        {
            manager.PlayQuiz();
        }


        /// <summary>
        /// Setups the next question.
        /// </summary>
        private void SetupNextQuestion()
        {
            // First, detach from the existing entries
            if (_menuEntries != null)
            { 
                _menuEntries.ForEach(e => e.Selected -= entry_Selected);
            }

            // Now, load and hook up the new ones
            if (_questions.Count > 0)
            {
                _currentQuestion = _questions.Pop();

                this._menuEntries = new List<AnswerEntry>();
                foreach (Answer answer in _currentQuestion.Answers)
                {
                    AnswerEntry entry = new AnswerEntry(answer);
                    entry.Selected += entry_Selected;
                    this._menuEntries.Add(entry);
                }
            }
            else
            {
                //  No more questions?  No more game!
                ExitScreen();
            }
        }

        /// <summary>
        /// Handles the Selected event of the entry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        void entry_Selected(object sender, PlayerIndexEventArgs e)
        {
            AnswerEntry entry = (AnswerEntry)sender;
            if (entry.IsCorrect)
            {
                _correctSound.Play();
                Player.AddToScore(_correctAnswerPoints);
            }
            else
            {
                _incorrectSound.Play();
                _quizTimer.SubtractTime(_incorrectAnswerTime);
            }

            SetupNextQuestion();
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            _quizTimer.Draw(ScreenManager.SpriteBatch);

            // make sure our entries are in the right place before we draw them
            this.UpdateMenuEntryLocations();

            GraphicsDevice graphics = this.ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
            SpriteFont font = this.ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                AnswerEntry menuEntry = this._menuEntries[i];

                bool isSelected = this.IsActive && (i == this._selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 120);
            Vector2 titleOrigin = font.MeasureString(this._currentQuestion.QuestionText) / 2;
            Color titleColor = new Color(192, 192, 192) * this.TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, this._currentQuestion.QuestionText, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // Move to the previous menu entry?
            if (input.IsMenuUp(this.ControllingPlayer, out playerIndex))
            {
                this._selectedEntry--;

                if (this._selectedEntry < 0)
                    this._selectedEntry = this._menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(this.ControllingPlayer, out playerIndex))
            {
                this._selectedEntry++;

                if (this._selectedEntry >= this._menuEntries.Count)
                    this._selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
            {
                this.OnSelectEntry(this._selectedEntry, playerIndex);
            }

            base.HandleInput(input);
        }

        /// <summary>
        /// Called when [select entry].
        /// </summary>
        /// <param name="entryIndex">Index of the entry.</param>
        /// <param name="playerIndex">Index of the player.</param>
        protected void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (_menuEntries.Count > entryIndex)
            {
                this._menuEntries[entryIndex].OnSelectEntry(playerIndex);
            }
        }

        /// <summary>
        /// Updates the menu entry locations.
        /// </summary>
        protected void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 310);

            // update each menu entry's location in turn
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                AnswerEntry menuEntry = this._menuEntries[i];

                // each entry is to be centered horizontally
                position.X = this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }


        /// <summary>
        /// Trims the size of the question.
        /// </summary>
        /// <param name="question">The question.</param>
        private void TrimQuestionSize(Question question)
        {
            if (question != null)
            {
                question.QuestionText = BreakUpString(question.QuestionText, MAX_QUESTION_LENGTH);

                foreach (Answer answer in question.Answers)
                {
                    answer.AnswerText = BreakUpString(answer.AnswerText, MAX_ANSWER_LENGTH);
                }
            }
        }

        /// <summary>
        /// Breaks up string so that it doesn't extend beyond the screen... nothing fancy, just hardcoded limits
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private string BreakUpString(string original, int length)
        {
            if (original.Length > length)
            {
                StringBuilder newSting = new StringBuilder();

                string[] parts = original.Split(' ');

                int charCount = parts[0].Length;
                newSting.Append(parts[0]);

                for (int i = 1; i < parts.Length; i++)
                {
                    string part = parts[i];
                    if (charCount > length)
                    {
                        newSting.AppendLine();
                        charCount = 0;
                        newSting.Append(part);
                    }
                    else
                    {
                        newSting.Append(' ' + part);
                    }
                    charCount += part.Length;
                }

                return newSting.ToString();
            }

            return original;
        }

        /// <summary>
        /// Shuffles the and trim questions.
        /// </summary>
        /// <param name="questions">The questions.</param>
        /// <returns></returns>
        public Stack<Question> ShuffleAndTrimQuestions(List<Question> questions)
        {
            Stack<Question> questionsStack = new Stack<Question>();

            Random rng = new Random();
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Question value = questions[k];
                questions[k] = questions[n];
                questions[n] = value;
            }

            foreach (Question question in questions)
            {
                TrimQuestionSize(question);
                questionsStack.Push(question);
            }

            return questionsStack;
        }
    }
}
