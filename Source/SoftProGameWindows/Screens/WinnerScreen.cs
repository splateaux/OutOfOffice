using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SoftProGameWindows
{
    /// <summary>
    /// The intro screen shows the video story at the beginning of the game sequence.
    /// </summary>
    public class WinnerScreen : MenuScreen
    {
        private ContentManager _content;
        private string _selectedCharacterName;
        private int _score;
        private TimeSpan _timeToLive;
        private TimeSpan _transitionTime;
        private TransitionHelper _transitionHelper;
        private int _maxTimeAllowed;

        private Texture2D _background;
        private Rectangle _scoreLocation;
        private SpriteFont _font;
        private Color _color;
        private int _pointsToAward;
        private float _percentPointsToAdd;
        private int _lastPercentSounded;

        private bool _winningSoundPlayed;

        // Textbox
        private Texture2D _nameDialog;
        private TextBox _textBox;
        private Texture2D _textBoxTexture;
        private Texture2D _caretTexture;
        private SpriteFont _textBoxFont;
        private bool _showTextBox;

        // Menu
        private SpriteFont _menuFont;
        private string _menuText;
        private Color _menuTextColor;
        private float _selectionFade;
        private Vector2 _menuPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinnerScreen" /> class.
        /// </summary>
        /// <param name="selectedCharacterName">Name of the selected character.</param>
        /// <param name="score">The score.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="maxTimeAllowed">The maximum time allowed.</param>
        public WinnerScreen(string selectedCharacterName, int score, TimeSpan timeToLive, int maxTimeAllowed)
            : base("")
        {
            this._selectedCharacterName = selectedCharacterName;
            this._score = score;
            this._timeToLive = timeToLive;
            this._maxTimeAllowed = maxTimeAllowed;
            this._transitionTime = TimeSpan.FromSeconds(3);
            this._transitionHelper = new TransitionHelper(this._transitionTime, TransitionHelper.TransitionDirection.Up);
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            if ((this._content == null) && (this.ScreenManager != null))
            {
                this._content = new ContentManager(this.ScreenManager, "Content");
            }

            // Assign the menu information
            this._menuFont = this.ScreenManager.Font;
            this._menuText = "Press A to continue";
            this._menuTextColor = Color.Yellow;
            this._menuPosition = new Vector2(756.0f, 670.0f);

            // Intialize the textbox info
            this._nameDialog = this._content.Load<Texture2D>("Textures/Overlays/ScoreDialog");
            this._textBoxTexture = this._content.Load<Texture2D>("Textures/TextBoxBlue");
            this._caretTexture = this._content.Load<Texture2D>("Textures/Caret");
            this._textBoxFont = this._content.Load<SpriteFont>("Fonts/NameEntry");

            this._textBox = new TextBox(this._textBoxTexture, this._caretTexture, this._textBoxFont);
            this._textBox.X = this.ScreenManager.GraphicsDevice.Viewport.Width / 2;
            this._textBox.Y = this.ScreenManager.GraphicsDevice.Viewport.Height / 2;
            this._textBox.Width = 200;

            this._showTextBox = false;

            // Load and add the images, sounds, etc.
            if (this._selectedCharacterName.Equals("Don", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/WinnerScreen/Don");
                this._scoreLocation = new Rectangle(300, 490, 355, 115);
                this._font = this._content.Load<SpriteFont>("Fonts/Stencil");
                this._color = Color.Black;
            }
            else if (this._selectedCharacterName.Equals("John", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/WinnerScreen/John");
                this._scoreLocation = new Rectangle(340, 465, 290, 120);
                this._font = this._content.Load<SpriteFont>("Fonts/Impact");
                this._color = Color.White;
            }
            else if (this._selectedCharacterName.Equals("McB", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/WinnerScreen/McB");
                this._scoreLocation = new Rectangle(340, 465, 290, 120);
                this._font = this._content.Load<SpriteFont>("Fonts/Impact");
                this._color = Color.CornflowerBlue;
            }

            // Get the points allowed and gather other information
            var vm = this.GetService<ObjectValueManager>();
            var maxPointsAwarded = (float)vm.GetValue(Constants.MAX_POINTS_AWARDED_FOR_TIME);
            var maxTimeAllowed = (float)(this._maxTimeAllowed * 1000);
            var minTimeAllowed = (float)(60 * 1000);  // 00:01:00.000
            var playerTime = (float)this._timeToLive.TotalMilliseconds;

            // Calculating awarded points (thanks to Phil!)
            this._pointsToAward = (int)Math.Round((maxPointsAwarded / 2.0f) * (Math.Cos(Math.PI * ((playerTime - minTimeAllowed) / (maxTimeAllowed - minTimeAllowed))) + 1.0f));

            // Play the celebration sound (volume adjusted down so that it doesn't overpower the real player winning sound)
            var cheer = this._content.Load<SoundEffect>("Audio/Effects/Cheering");
            cheer.Play(.1f, 0f, 0f);
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            if (this._content != null)
            {
                this._content.Unload();
            }
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            this.ScreenManager.RemoveScreen(this);
            LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(Constants.MAINGAME_BACKGROUND_TEXTURE), new MainMenuScreen());
        }

        /// <summary>
        /// Called when update occurs.
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            this._transitionHelper.Update(gametime);
            this._percentPointsToAdd = (float)Math.Pow(this._transitionHelper.Position, 2);
            if (this._percentPointsToAdd > 0.99f) this._percentPointsToAdd = 1.0f;

            if ((int)(this._percentPointsToAdd * 100) / 5 != this._lastPercentSounded)
            {
                this._lastPercentSounded = (int)(this._percentPointsToAdd * 100) / 5;
                if (!_winningSoundPlayed)
                {
                    _winningSoundPlayed = true;
                    this.GetService<SoundEffectManager>().PlayWinner();
                }
            }

            if (this._showTextBox)
            {
                this._textBox.Update(gametime);
            }

            float fadeSpeed = (float)gametime.ElapsedGameTime.TotalSeconds * 4;
            this._selectionFade = Math.Min(this._selectionFade + fadeSpeed, 1);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        /// <param name="input">Helper for reading input from keyboard, gamepad, and touch input.</param>
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if ((this._transitionHelper.State == TransitionHelper.TransitionState.Complete) && (input != null))
            {
                var sm = this.GetService<SettingsManager>();
                var useScoreKeeper = Boolean.Parse(sm.GetValue("UseScoreKeeper"));

                PlayerIndex index;
                if (input.IsPrimaryPressed(null, out index))
                {
                    if (useScoreKeeper)
                    {
                        if (!this._showTextBox)
                        {
                            var kb = this.GetService<KeyboardDispatcher>();
                            kb.Subscriber = this._textBox;
                            this._showTextBox = true;
                        }
                    }
                    else
                    {
                        // Return to the main menu
                        this.ScreenManager.RemoveScreen(this);
                        LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(Constants.MAINGAME_BACKGROUND_TEXTURE), new MainMenuScreen());
                    }
                }
                else if (useScoreKeeper && this._showTextBox && input.IsSecondaryPressed(null, out index))
                {
                    try
                    {
                        // Submit text to service
                        Scoring.Reporter.Send(this._selectedCharacterName, this._textBox.Text, this._score + this._pointsToAward);
                    }
                    catch
                    {
                        // Eat the exception, don't blow up the app!
                    }

                    // Return to the main menu
                    this.ScreenManager.RemoveScreen(this);
                    LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(Constants.MAINGAME_BACKGROUND_TEXTURE), new MainMenuScreen());
                }
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            this.ScreenManager.SpriteBatch.Begin();

            // Draw the background
            this.ScreenManager.SpriteBatch.Draw(this._background, this.ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);

            // Calculate the string position
            var text = (this._score + (int)(this._percentPointsToAdd * this._pointsToAward)).ToString();
            var centerOfLocation = (int)(this._scoreLocation.X + ((this._scoreLocation.Right - this._scoreLocation.Left) / 2));
            var leftOfCenter = (int)this._font.MeasureString(text).X / -2;
            var position = new Vector2(centerOfLocation + leftOfCenter, this._scoreLocation.Y);

            // Draw the score
            DrawingUtils.DrawShadowedString(this.ScreenManager.SpriteBatch, this._font, text, position, this._color);

            // Show the continue "menu" item
            if (this._showTextBox)
            {
                var centerX = this.ScreenManager.GraphicsDevice.Viewport.Width / 2;
                var centerY = this.ScreenManager.GraphicsDevice.Viewport.Height / 2;
                var centerXDialog = this._nameDialog.Width / 2;
                var centerYDialog = this._nameDialog.Height / 2;
                var dialogPosition = new Rectangle(centerX - centerXDialog, centerY - centerYDialog, this._nameDialog.Width, this._nameDialog.Height);

                this.ScreenManager.SpriteBatch.Draw(this._nameDialog, dialogPosition, Color.White);

                this._textBox.X = dialogPosition.X + 58;
                this._textBox.Y = dialogPosition.Y + 166;
                this._textBox.Width = 440;

                this._textBox.Draw(this.ScreenManager.SpriteBatch, gameTime);
            }
            else if (this._transitionHelper.State == TransitionHelper.TransitionState.Complete)
            {
                // Pulsate the size of the selected menu entry.
                double time = gameTime.TotalGameTime.TotalSeconds;

                float pulsate = (float)Math.Sin(time * 6) + 1;

                float scale = 1 + pulsate * 0.05f * this._selectionFade;

                this.ScreenManager.SpriteBatch.DrawString(
                    this._menuFont, this._menuText, this._menuPosition, this._menuTextColor, 0,
                    Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            this.ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}