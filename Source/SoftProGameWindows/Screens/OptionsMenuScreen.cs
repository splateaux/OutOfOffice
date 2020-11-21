namespace SoftProGameWindows
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class OptionsMenuScreen : MenuScreen
    {
        private MenuEntry _ungulateMenuEntry;
        private MenuEntry _languageMenuEntry;
        private MenuEntry _frobnicateMenuEntry;
        private MenuEntry _elfMenuEntry;

        private enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            Llama,
        }

        private static Ungulate currentUngulate = Ungulate.Dromedary;

        private static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        private static int currentLanguage = 0;

        private static bool frobnicate = true;

        private static int elf = 23;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            this._ungulateMenuEntry = new MenuEntry(string.Empty);
            this._languageMenuEntry = new MenuEntry(string.Empty);
            this._frobnicateMenuEntry = new MenuEntry(string.Empty);
            this._elfMenuEntry = new MenuEntry(string.Empty);

            this.SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            this._ungulateMenuEntry.Selected += this.UngulateMenuEntrySelected;
            this._languageMenuEntry.Selected += this.LanguageMenuEntrySelected;
            this._frobnicateMenuEntry.Selected += this.FrobnicateMenuEntrySelected;
            this._elfMenuEntry.Selected += this.ElfMenuEntrySelected;
            back.Selected += this.OnCancel;

            // Add entries to the menu.
            this.MenuEntries.Add(this._ungulateMenuEntry);
            this.MenuEntries.Add(this._languageMenuEntry);
            this.MenuEntries.Add(this._frobnicateMenuEntry);
            this.MenuEntries.Add(this._elfMenuEntry);
            this.MenuEntries.Add(back);
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        private void SetMenuEntryText()
        {
            this._ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
            this._languageMenuEntry.Text = "Language: " + languages[currentLanguage];
            this._frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            this._elfMenuEntry.Text = "elf: " + elf;
        }

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        private void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;

            if (currentUngulate > Ungulate.Llama)
                currentUngulate = 0;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        private void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        private void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            frobnicate = !frobnicate;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        private void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            elf++;

            SetMenuEntryText();
        }
    }
}