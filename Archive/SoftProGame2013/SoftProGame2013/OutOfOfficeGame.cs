using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using InputController;

namespace SoftProGame2014
{
    public class OutOfOfficeGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        ScrollingBackground _scrollingBG;

        Player _player;
        int _speed;
        UserInput _userInput;
        List<Sprite> _sprites;
        bool _debugCollisionsMode;
        bool _isBackgroundMoving;

        public OutOfOfficeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // New up the fields.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _speed = 4;
            //_userInput = UserInput.GetClassInstance(this.Window.Handle);
            _userInput = UserInput.GetClassInstance();
            //_debugCollisionsMode = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load scroller background.
            _scrollingBG = new ScrollingBackground(Content.Load<Texture2D>("Textures/Backgrounds/RoadAndTrees1"),
                                 Content.Load<Texture2D>("Textures/Backgrounds/RoadAndTrees2"));
            // Load character.
            Texture2D texture = Content.Load<Texture2D>("Textures/Sprites/John_LowRes_2Rows");
            AtlasInfo characterAtlas = new AtlasInfo(texture, 4, 4, 8, 11, 4, 7, 5, 9);
            _player = new Player(characterAtlas);

            _sprites = new List<Sprite>();

            _sprites.Add(new SquishyBug(Content.Load<Texture2D>("Textures/Sprites/Turtle"), new Vector2(600, 385), .5f));
            _sprites.Add(new Box(Content.Load<Texture2D>("Textures/Sprites/Box"), new Vector2(700, 315), .5f));
            _sprites.Add(new SoftproDollar(Content.Load<Texture2D>("Textures/Sprites/Dollar"), new Vector2(800, 200), .1f));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Check for key press.
            _userInput.PollUserInput();
            ControllerKey direction = _userInput.GetDirectionKeys();

            CheckCollisions();

            _isBackgroundMoving = (_player.Position.X > 350 && direction.HasFlag(ControllerKey.Right));

            _player.Update(gameTime, direction, _isBackgroundMoving, _speed);

            _scrollingBG.Update(_speed, _isBackgroundMoving);

            _sprites.ForEach(s => s.Update(_isBackgroundMoving, _speed));

            // If any sprites thing they were destroyed... finish the job.
            _sprites.RemoveAll(s => s.IsDestroyed);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // The order that you load the sprites will determine the Z order of the images.
            if (_debugCollisionsMode)
            {
                // Just playing around with drawing boundingboxes
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                RasterizerState state = new RasterizerState();
                state.FillMode = FillMode.WireFrame;
                _spriteBatch.GraphicsDevice.RasterizerState = state;
            }
            else
            {
                _spriteBatch.Begin();
            }

            _scrollingBG.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);

            _sprites.ForEach(b => b.Draw(_spriteBatch));

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks for collisions.  Let's each collidable object know what it collided with.
        /// </summary>
        private void CheckCollisions()
        {
            foreach (ICollidable thing1 in _sprites.Where(s => s is ICollidable))
            {
                // Will this inner loop be needed?  Can non-player things run into each other?  I'm thinking bug walking into a wall?
                foreach (ICollidable thing2 in _sprites.Where(s => s != thing1 && s is ICollidable))
                {
                    if (thing1.BoundingBox.Intersects(thing2.BoundingBox))
                    {
                        thing1.OnCollision(thing2);
                    }
                }

                if (_player.BoundingBox.Intersects(thing1.BoundingBox))
                {
                    _player.OnCollision(thing1);
                    thing1.OnCollision(_player);
                }
            }
        }
    }
}
