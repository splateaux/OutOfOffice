using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// The email screen shows the user's email at the beginning of the game sequence.
    /// </summary>
    public class EmailScreen : MenuScreen
    {
        private ContentManager _content;
        private MenuEntry _continueEntry;
        private SoundEffect _clickSound;
        private SoundEffect _reminderSound;
        private List<Texture2D> _emailMessages;
        private List<SoundEffect> _emailSounds;
        private int _messageIndex;
        private Texture2D _currentBackground;
        private string _selectedCharacterName;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public EmailScreen(string selectedCharacterName)
            : base("")
        {
            // Save the selected character name
            this._selectedCharacterName = selectedCharacterName;

            // Create our menu entries.
            this._continueEntry = new MenuEntry("Press A to Continue");
            this._continueEntry.SelectedColor = Color.Blue;

            // Hook up menu event handlers.
            this._continueEntry.Selected += ContinueEntrySelected;

            // Add entries to the menu.
            this.MenuEntries.Add(this._continueEntry);

            // Initialize the email messages and sounds
            this._emailMessages = new List<Texture2D>();
            this._emailSounds = new List<SoundEffect>();
            this._messageIndex = -1;
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

            // Load and add the email messages
            this._emailMessages.Add(this._content.Load<Texture2D>("Textures/Overlays/Email_Main_1"));
            this._emailMessages.Add(this._content.Load<Texture2D>("Textures/Overlays/Email_Alert_2"));
            this._emailMessages.Add(this._content.Load<Texture2D>("Textures/Overlays/Email_SetTheStage_3"));

            // Load the audio effects
            this._clickSound = this._content.Load<SoundEffect>("Audio/Effects/Click");
            this._reminderSound = this._content.Load<SoundEffect>("Audio/Effects/Reminder");

            // Add a list of email sounds
            this._emailSounds.Add(this._clickSound);
            this._emailSounds.Add(this._reminderSound);
            this._emailSounds.Add(this._reminderSound);

            // Intialize the selected email
            this.ContinueEntrySelected(this, null);
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
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected override void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 650f);

            // each entry is to be centered horizontally
            position.X = this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - this._continueEntry.GetWidth(this) / 2;

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // set the entry's position
            this._continueEntry.Position = position;

            // move down for the next entry the size of this entry
            position.Y += this._continueEntry.GetHeight(this);
        }

        /// <summary>
        /// Continues the entry selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        private void ContinueEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            this._messageIndex++;
            if (this._messageIndex < this._emailMessages.Count)
            {
                this._currentBackground = this._emailMessages[this._messageIndex];
                this._emailSounds[this._messageIndex].Play();
            }
            else
            {
                this.ScreenManager.RemoveScreen(this);
                LoadingScreen.Load(this.ScreenManager, true, e.PlayerIndex, new GameplayScreen(this._selectedCharacterName));
            }
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // Do nothing
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            this.ScreenManager.SpriteBatch.Begin();

            this.ScreenManager.SpriteBatch.Draw(this._currentBackground,
                this.ScreenManager.GraphicsDevice.Viewport.Bounds,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            this.ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}