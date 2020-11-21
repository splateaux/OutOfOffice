using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Manages Sound Effects... and stuff
    /// </summary>
    internal class SoundEffectManager
    {
        private SoundEffect _dollarSound;
        private string _playerName = null;
        private ContentManager _content;
        private Random _rand;
        private TimeSpan _soundWaitPeriod;
        private DateTime _soundLastPlayed;


        // Audio Collections
        private SoundEffect[] _playerChooseMe;
        private SoundEffect[] _playerCrushBug;
        private SoundEffect[] _playerFridge;
        private SoundEffect[] _playerHitByBug;
        private SoundEffect[] _playerLoser;
        private SoundEffect[] _playerMain;
        private SoundEffect[] _playerMaze;
        private SoundEffect[] _playerQuiz;
        private SoundEffect[] _playerJoyce;
        private SoundEffect[] _playerWinner;
        private SoundEffect[] _bugSquish;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffectManager"/> class.
        /// </summary>
        public SoundEffectManager()
        { 
            _rand = new Random();
            _soundLastPlayed = DateTime.Now;
        }

        /// <summary>
        /// Loads the sound effects
        /// </summary>
        /// <param name="content">The content.</param>
        public void LoadContent(ContentManager content)
        {
            _content = content;

            _dollarSound = content.Load<SoundEffect>("Audio/Effects/SoftProDollarCollected");

            LoadAudioArray(out _bugSquish, "Audio\\Effects\\BugSquish");
        }

        /// <summary>
        /// Sets the player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void SetPlayer(Player player)
        {
            // If the player name isn't changing, don't worry about reloading any audio
            if (_playerName != player.CharacterName)
            {
                _playerName = player.CharacterName;
                LoadPlayerAudio();
            }
        }

        /// <summary>
        /// Loads the player audio.
        /// </summary>
        private void LoadPlayerAudio()
        {
            LoadAudioArray(out _playerChooseMe, "Audio\\VoiceOvers\\ChooseMeScreen\\" + _playerName);
            LoadAudioArray(out _playerCrushBug, "Audio\\VoiceOvers\\CrushBug\\" + _playerName);
            LoadAudioArray(out _playerFridge, "Audio\\VoiceOvers\\FridgeGame\\" + _playerName);
            LoadAudioArray(out _playerHitByBug, "Audio\\VoiceOvers\\HitByBug\\" + _playerName);
            LoadAudioArray(out _playerLoser, "Audio\\VoiceOvers\\LoserScreen\\" + _playerName);
            LoadAudioArray(out _playerMain, "Audio\\VoiceOvers\\MainBoard\\" + _playerName);
            LoadAudioArray(out _playerMaze, "Audio\\VoiceOvers\\MazeGame\\" + _playerName);
            LoadAudioArray(out _playerQuiz, "Audio\\VoiceOvers\\QuizGame\\" + _playerName);
            LoadAudioArray(out _playerJoyce, "Audio\\VoiceOvers\\SeeJoyce\\" + _playerName);
            LoadAudioArray(out _playerWinner, "Audio\\VoiceOvers\\WinnerScreen\\" + _playerName);
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
        /// Plays the dollar sound.
        /// </summary>
        public void PlayDollarSound()
        {
            _dollarSound.Play();
        }

        /// <summary>
        /// Plays a random player-based choose me sound.
        /// </summary>
        public void PlayChooseMe()
        {
            PlayRandomSound(_playerChooseMe);
        }

        /// <summary>
        /// Plays a random player-based crush bug sound.
        /// </summary>
        public void PlayCrushBug()
        {
            PlayRandomSound(_playerCrushBug);
        }

        /// <summary>
        /// Plays a random player-based fridge sound.
        /// </summary>
        public void PlayFridge()
        {
            PlayRandomSound(_playerFridge);
        }

        /// <summary>
        /// Plays a random player-based hit by bug sound.
        /// </summary>
        public void PlayHitByBug()
        {
            PlayRandomSound(_playerHitByBug);
        }

        /// <summary>
        /// Plays a random player-based loser sound.
        /// </summary>
        public void PlayLoser()
        {
            PlayRandomSound(_playerLoser);
        }

        /// <summary>
        /// Plays a random player-based main sound.
        /// </summary>
        public void PlayMain()
        {
            PlayRandomSound(_playerMain);
        }

        /// <summary>
        /// Plays a random player-based maze sound.
        /// </summary>
        public void PlayMaze()
        {
            PlayRandomSound(_playerMaze);
        }

        /// <summary>
        /// Plays a random player-based quiz sound.
        /// </summary>
        public void PlayQuiz()
        {
            PlayRandomSound(_playerQuiz);
        }

        /// <summary>
        /// Plays a random player-based joyce sound.
        /// </summary>
        public void PlayJoyce()
        {
            PlayRandomSound(_playerJoyce);
        }

        /// <summary>
        /// Plays a random player-based winner sound.
        /// </summary>
        public void PlayWinner()
        {
            PlayRandomSound(_playerWinner);
        }

        /// <summary>
        /// Plays a random bug squishing sound
        /// </summary>
        public void PlayBugSquish()
        {
            int randomNumber = _rand.Next(_bugSquish.Count());
            _bugSquish[randomNumber].Play(.3f, 0f, 0f);
        }

        /// <summary>
        /// Plays a random player-based random sound.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void PlayRandomSound(SoundEffect[] collection)
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
