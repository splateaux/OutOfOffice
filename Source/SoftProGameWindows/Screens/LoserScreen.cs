using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// The intro screen shows the video story at the beginning of the game sequence.
    /// </summary>
    public class LoserScreen : MenuScreen
    {
        private ContentManager _content;
        private string _selectedCharacterName;
        private int _score;
        private TimeSpan _transitionTime;
        private TransitionHelper _transitionHelper;

        private Texture2D _background;
        private Rectangle _scoreLocation;
        private SpriteFont _font;
        private Color _color;
        private float _percentPointsToSubtract;
        private int _lastPercentSounded;

        private bool _losingSoundPlayed;

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
        public LoserScreen(string selectedCharacterName, int score)
            : base("")
        {
            this._selectedCharacterName = selectedCharacterName;
            this._score = score;
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

            // Load and add the images, sounds, etc.
            if (this._selectedCharacterName.Equals("Don", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/LoserScreen/Don");
                this._scoreLocation = new Rectangle(375, 430, 355, 115);
                this._font = this._content.Load<SpriteFont>("Fonts/Stencil");
                this._color = Color.Black;
            }
            else if (this._selectedCharacterName.Equals("John", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/LoserScreen/John");
                this._scoreLocation = new Rectangle(485, 480, 290, 120);
                this._font = this._content.Load<SpriteFont>("Fonts/Impact");
                this._color = Color.White;
            }
            else if (this._selectedCharacterName.Equals("McB", StringComparison.OrdinalIgnoreCase))
            {
                this._background = this._content.Load<Texture2D>("Textures/Backgrounds/LoserScreen/McB");
                this._scoreLocation = new Rectangle(340, 465, 290, 120);
                this._font = this._content.Load<SpriteFont>("Fonts/Impact");
                this._color = Color.CornflowerBlue;
            }

            // Play the celebration sound
            var cheer = this._content.Load<SoundEffect>("Audio/Effects/Boo");
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
            this._percentPointsToSubtract = (float)Math.Pow(this._transitionHelper.Position, 2);
            if (this._percentPointsToSubtract > 0.99f) this._percentPointsToSubtract = 1.0f;

            if ((int)(this._percentPointsToSubtract * 100) / 5 != this._lastPercentSounded)
            {
                this._lastPercentSounded = (int)(this._percentPointsToSubtract * 100) / 5;
                if (!_losingSoundPlayed)
                {
                    _losingSoundPlayed = true;
                    this.GetService<SoundEffectManager>().PlayLoser();
                }
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

            PlayerIndex index;
            if (input.IsPrimaryPressed(null, out index))
            {
                this.ScreenManager.RemoveScreen(this);
                LoadingScreen.Load(this.ScreenManager, false, null, new BackgroundScreen(Constants.MAINGAME_BACKGROUND_TEXTURE), new MainMenuScreen());
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
            var text = (this._score - (int)(this._percentPointsToSubtract * this._score)).ToString();
            var centerOfLocation = (int)(this._scoreLocation.X + ((this._scoreLocation.Right - this._scoreLocation.Left) / 2));
            var leftOfCenter = (int)this._font.MeasureString(text).X / -2;
            var position = new Vector2(centerOfLocation + leftOfCenter, this._scoreLocation.Y);

            // Draw the score
            DrawingUtils.DrawShadowedString(this.ScreenManager.SpriteBatch, this._font, text, position, this._color);

            // Show the continue "menu" item
            if (this._transitionHelper.Position == 1.0f)
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