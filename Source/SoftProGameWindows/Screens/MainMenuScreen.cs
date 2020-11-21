using Microsoft.Xna.Framework;

namespace SoftProGameWindows
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    public class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("")
        {
            // Create our menu entries.
            var playGameMenuEntry = new MenuEntry("Start Game");
            var exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            exitMenuEntry.Selected += this.OnCancel;

            // Add entries to the menu.
            this.MenuEntries.Add(playGameMenuEntry);
            this.MenuEntries.Add(exitMenuEntry);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs" /> instance containing the event data.</param>
        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            this.ScreenManager.AddScreen(new PlayerSelectScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        /// <param name="playerIndex">The index of the player calling the event.</param>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit the game?";

            var confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += this.ConfirmExitMessageBoxAccepted;

            this.ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs" /> instance containing the event data.</param>
        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            this.ScreenManager.Game.Exit();
        }
    }
}