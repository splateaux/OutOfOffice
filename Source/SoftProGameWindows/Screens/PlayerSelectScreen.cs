using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Represents the lobby screen where player selection and stage transition occurs.
    /// </summary>
    public class PlayerSelectScreen : MenuScreen
    {
        private static string[] _playerSelections = { "John the Architect", "McB the Plaid", "Don the Defender" };
        private static string[] _characterNames = { "John", "McB", "Don" };
        private int _currentPlayerSelection;
        private MenuEntry _playerEntry;
        private ContentManager _content;
        //private Texture2D _backgroundTexture;
        private Dictionary<string, Texture2D> _backgroundTextures;
        private Texture2D _currentBackground;

        // Sound effects
        private SoundEffect[] _mcBSounds;
        private SoundEffect[] _johnSounds;
        private SoundEffect[] _donSounds;
        private TimeSpan _soundWaitPeriod;
        private DateTime _soundLastPlayed;
        private Random _rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSelectScreen"/> class.
        /// </summary>
        public PlayerSelectScreen() :
            base("")
        {
            // Create the menu entries
            this._playerEntry = new MenuEntry(string.Empty);

            // Initialize the entry text
            this.SetMenuSelection();

            // Hook up the menu event handlers
            this._playerEntry.Selected += PlayerEntry_Selected;

            // Add entries to the menu
            this.MenuEntries.Add(this._playerEntry);

            _rand = new Random();
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            if (this._content == null)
            {
                this._content = new ContentManager(this.ScreenManager.Game.Services, "Content");
            }

            // I know this is a messed up way to approach this so please give me some time to 
            // refine things in here before changing the code. It's late and I can't think anymore and I just wanted to see it work. I'll fix it soon.
            this._currentBackground = this._content.Load<Texture2D>("Textures/Backgrounds/ChooseCharacter/John");
            this._backgroundTextures = new Dictionary<string,Texture2D>();
            this._backgroundTextures.Add("John the Architect", this._currentBackground);
            this._backgroundTextures.Add("McB the Plaid", this._content.Load<Texture2D>("Textures/Backgrounds/ChooseCharacter/McB"));
            this._backgroundTextures.Add("Don the Defender", this._content.Load<Texture2D>("Textures/Backgrounds/ChooseCharacter/Don"));

            // Sound up our sound effects
            LoadAudioArray(out _mcBSounds, "Audio\\VoiceOvers\\ChooseMeScreen\\Mcb");
            LoadAudioArray(out _johnSounds, "Audio\\VoiceOvers\\ChooseMeScreen\\John");
            LoadAudioArray(out _donSounds, "Audio\\VoiceOvers\\ChooseMeScreen\\Don");
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public override void UnloadContent()
        {
            if (this._content != null)
            {
                this._content.Unload();
            }
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        /// <param name="input">Helper for reading input from keyboard, gamepad, and touch input.</param>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsRightPressed(null, out playerIndex))
            {
                this._currentPlayerSelection = (this._currentPlayerSelection + 1) % PlayerSelectScreen._playerSelections.Length;
                this.SetMenuSelection();
            }
            else if (input.IsLeftPressed(null, out playerIndex))
            {
                this._currentPlayerSelection = (this._currentPlayerSelection + PlayerSelectScreen._playerSelections.Length - 1) % PlayerSelectScreen._playerSelections.Length;
                this.SetMenuSelection();
            }
            else
            {
                base.HandleInput(input);
            }
        }

        /// <summary>
        /// Sets the menu entry text.
        /// </summary>
        private void SetMenuSelection()
        {
            this._playerEntry.Text = "Player: " + PlayerSelectScreen._playerSelections[this._currentPlayerSelection];

            // OK, I know this is a pretty messed up way to do this and -->> I <<-- will fix it! It's LATE but it's on MY list!
            if(_backgroundTextures != null)
               _backgroundTextures.TryGetValue(PlayerSelectScreen._playerSelections[this._currentPlayerSelection], out _currentBackground);

            // PLay a sound effect for whichever player is selected
            switch (this._currentPlayerSelection)
            {
                case 0:
                    this.PlayPlayerSound(_johnSounds);
                    break;
                case 1:
                    this.PlayPlayerSound(_mcBSounds);
                    break;
                case 2:
                    this.PlayPlayerSound(_donSounds);
                    break;
            }
        }

        /// <summary>
        /// Handles the Selected event of the PlayerEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PlayerIndexEventArgs" /> instance containing the event data.</param>
        private void PlayerEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            this.ScreenManager.RemoveScreen(this);
            var selectedCharacterName = _characterNames[this._currentPlayerSelection];
            //LoadingScreen.Load(this.ScreenManager, false, e.PlayerIndex, new EmailScreen(selectedCharacterName));
            LoadingScreen.Load(this.ScreenManager, false, e.PlayerIndex, new IntroScreen(selectedCharacterName));
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
        }

        /// <summary>
        /// Loads the audio array.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="path">The path.</param>
        private void LoadAudioArray(out SoundEffect[] collection, string path)
        {
            DirectoryInfo dir = new DirectoryInfo(_content.RootDirectory + "\\" + path);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.*");
                collection = new SoundEffect[files.Count()];

                for (int i = 0; i < files.Count(); i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files[i].Name);
                    collection[i] = _content.Load<SoundEffect>(path + "/" + name);
                }
            }
            else
            {
                collection = null;
            }
        }

        /// <summary>
        /// Plays the player sound.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void PlayPlayerSound(SoundEffect[] collection)
        {
            if (collection == null) return;

            if ((DateTime.Now.TimeOfDay - _soundLastPlayed.TimeOfDay) > _soundWaitPeriod)
            {
                int randomNumber = _rand.Next(collection.Count());
                _soundWaitPeriod = collection[randomNumber].Duration;
                collection[randomNumber].Play();

                _soundWaitPeriod = collection[randomNumber].Duration;
                _soundLastPlayed = DateTime.Now;
            }
        }

    }
}