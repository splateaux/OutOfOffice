using System;
using System.Collections.Generic;
using System.Linq;
using InputController;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FridgeGame
{
    public class Breakroom
    {
        int score;
        private Texture2D _texture;
        private GraphicsDevice _device;
        private Random _random;
        private List<SodaCan> _cans;
        private MiniPlayer _player;
        private Game _game;
        private ContentManager _content;
        private List<FridgeRack> _racks;
        private string _name;

        public ContentManager Content
        { 
            get { return _content; } 
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        
        public GraphicsDevice Device
        {
            get { return _device; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Breakroom"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public Breakroom(IServiceProvider serviceProvider, GraphicsDevice device, string playerName)
        {
            _content = new ContentManager(serviceProvider, "Content");
            _device = device;
            _cans = new List<SodaCan>();
            _racks = new List<FridgeRack>();
            _name = playerName;

            this._random = new Random();

            LoadContent();
            
        }

        private void LoadContent()
        {
            _texture = _content.Load<Texture2D>("Textures/Objects/FridgeGame/DrinkFridgeBackground");
            
            // Load the player
            _player = new MiniPlayer(this, new Vector2(50, _device.Viewport.Height - 400), _name);

            _racks.Add(new FridgeRack(new Vector2(456, 455), this, SodaCan.Brand.Can_DrDocuments_Small));
            _racks.Add(new FridgeRack(new Vector2(595, 455), this, SodaCan.Brand.Can_ImplementationMist_Small));
            _racks.Add(new FridgeRack(new Vector2(741, 455), this, SodaCan.Brand.Can_SupportSoda_Small));

            DropNewCan();
        }

        private void HandleCollisions()
        {
            foreach (SodaCan can in _cans)
            {
                if (!can.IsCollected && _player.BoundingBox.Intersects(can.BoundingBox) && _player.IsCatching)
                {
  
                    _player.CurrentlyHolding = can;
                    OnCanCollected(can, _player);
                    
                }
                else if (can.IsCollected)
                {
                    foreach(FridgeRack rack in _racks)
                    {
                        if (can.DrinkType == rack.RackSodaBrand && rack.BoundingBox.Intersects(can.BoundingBox))
                        {
                            OnCanDeposited(can, rack);
                        }
                    }

                }
            }
        }

        private void OnCanCollected(SodaCan can, MiniPlayer collectedBy)
        {
            can.OnCollected(collectedBy);
        }

        private void OnCanDeposited(SodaCan can, FridgeRack rack)
        {
            score += SodaCan.PointValue;
            can.IsDeposited = true;
            rack.OnDeposited(can);
        }

        public void OnCanReleased(SodaCan can)
        {
            can.IsCollected = true;
            can.Position = _player.Position;
            _cans.Add(can);
        }

        private void HandleFallingCans()
        {
            _cans.RemoveAll(c => c.Position.Y > (_device.Viewport.Height + 20) || c == _player.CurrentlyHolding || c.IsDeposited);

            if (_cans.Count == 0)
                DropNewCan();

        }

        private void DropNewCan()
        {
            int xPos = _random.Next(_device.Viewport.Width - 50);

            _cans.Add(new SodaCan(new Vector2(xPos, -100), this));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_texture, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            foreach (FridgeRack rack in _racks)
            {
                rack.Draw(gameTime, spriteBatch);
            }
            
            foreach (SodaCan can in _cans)
            {
                can.Draw(gameTime, spriteBatch); ;
            }

            spriteBatch.Begin();
            _player.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            
        }

        public void Update(GameTime gameTime, NintendoControllerState controllerState,KeyboardState keyboardState)
        {
            
            _player.Update(gameTime, controllerState, keyboardState);

            foreach (SodaCan can in _cans)
            {
                can.Update(gameTime);
            }

            HandleFallingCans();

            HandleCollisions();
        }
    }
}
