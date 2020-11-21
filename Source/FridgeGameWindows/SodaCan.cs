using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FridgeGame
{
    public class SodaCan
    {
        public const int PointValue = 1200;

        // Physics state
        private Vector2 _basePosition;
        private Vector2 _position;
        private Texture2D _texture;

        // Soda Can
        public readonly Color Color = Color.Yellow;
        private SoundEffect _collectedSound;
        private bool _isActive;
        private float _timer;
        float _dropInterval = 2f;
        private Breakroom _breakroom;
        private Rectangle relativeBoundingBox = new Rectangle(0, 0, 0, 0);
        private Brand _brand;
        private bool _isCollected = false;
        private bool _isDeposited = false;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
        }

        public Brand DrinkType
        {
            get { return _brand; }
        }

        public bool IsCollected
        {
            get { return _isCollected; }
            set { _isCollected = value; }
        }

        public bool IsDeposited
        {
            get { return _isDeposited; }
            set { _isDeposited = value; }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + relativeBoundingBox.X,
                    (int)Position.Y + relativeBoundingBox.Y,
                    (relativeBoundingBox.Width > 0) ? relativeBoundingBox.Width : Texture.Width,
                    (relativeBoundingBox.Height > 0) ? relativeBoundingBox.Height : Texture.Height);
            }
        }

        public SodaCan(Vector2 startPosition, Breakroom breakroom)
        {
            Random random = new Random();

            _position = startPosition;
            _breakroom = breakroom;
            _brand = 
            
             Enum.GetValues(typeof (Brand))
                            .Cast<Brand>()
                            .OrderBy (x => random.Next())
                            .FirstOrDefault();

            LoadContent();
        }

        /// <summary>
        /// Constructs a new can.
        /// </summary>
        public SodaCan(Vector2 startPosition, Breakroom breakroom, Brand brand)
        {
            _position = startPosition;
            _breakroom = breakroom;
            this._isActive = true;

            LoadContent();
        }

   
        public void LoadContent()
        {
            this._texture = _breakroom.Content.Load<Texture2D>(String.Format("Textures/Objects/FridgeGame/{0}", _brand.ToString()));
            this._collectedSound = _breakroom.Content.Load<SoundEffect>("Audio/Effects/SoftProDollarCollected");
            this._isActive = true;
        }

        public void ApplyPhysics(GameTime gameTime)
        {
                _position += new Vector2(0, 3f);
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="input">The input.</param>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame.
        /// </remarks>
        public void Update(GameTime gameTime)
        {
            ApplyPhysics(gameTime);
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _dropInterval)
            {
                _timer = 0f;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_texture, _position, null, Color.White);
            spriteBatch.End();
        }

        public void OnCollected(MiniPlayer collectedBy)
        {
            _collectedSound.Play();
        }

        public enum Brand
        {
            Can_DrDocuments_Small = 0,
            Can_ImplementationMist_Small,
            Can_SupportSoda_Small
        }
    }

}
