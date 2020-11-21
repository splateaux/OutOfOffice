using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SoftProGameWindows
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        protected List<MenuEntry> _menuEntries = new List<MenuEntry>();
        protected int _selectedEntry = 0;
        protected string _menuTitle;

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return this._menuEntries; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="menuTitle">The menu title.</param>
        public MenuScreen(string menuTitle)
        {
            this._menuTitle = menuTitle;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        /// <param name="input">Helper for reading input from keyboard, gamepad, and touch input.</param>
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
            else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
            {
                this.OnCancel(playerIndex);
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        /// <param name="entryIndex">Index of the entry.</param>
        /// <param name="playerIndex">Index of the player.</param>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (this._menuEntries.Count > 0)
            {
                this._menuEntries[entryIndex].OnSelectEntry(playerIndex);
            }
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            this.ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs"/> instance containing the event data.</param>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            this.OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 490f);

            // update each menu entry's location in turn
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                MenuEntry menuEntry = this._menuEntries[i];

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
        /// Called when [update].
        /// </summary>
        /// <param name="gametime">The gametime.</param>
        protected override void OnUpdate(GameTime gametime)
        {
            base.OnUpdate(gametime);

            // Update each nested MenuEntry object.
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                bool isSelected = this.IsActive && (i == this._selectedEntry);

                this._menuEntries[i].Update(this, isSelected, gametime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            this.UpdateMenuEntryLocations();

            GraphicsDevice graphics = this.ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
            SpriteFont font = this.ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < this._menuEntries.Count; i++)
            {
                MenuEntry menuEntry = this._menuEntries[i];

                bool isSelected = this.IsActive && (i == this._selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(this._menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * this.TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, this._menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}