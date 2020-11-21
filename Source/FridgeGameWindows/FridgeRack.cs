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
    public class FridgeRack
    {
        private Vector2 _position;

        // Rack
        private Breakroom _breakroom;
        private Rectangle _relativeBoundingBox;
        private SodaCan.Brand _rackSodaBrand;
        int canCount;
        private SpriteFont _counterFont;
        
        Texture2D box;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public SodaCan.Brand RackSodaBrand
        {
            get { return _rackSodaBrand; }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + _relativeBoundingBox.X,
                    (int)Position.Y + _relativeBoundingBox.Y,
                    _relativeBoundingBox.Width ,
                    _relativeBoundingBox.Height);
            }
        }

        /// <summary>
        /// Constructs a rack to hold cand
        /// </summary>
        public FridgeRack(Vector2 startPosition, Breakroom breakroom, SodaCan.Brand rackType)
        {
            _position = startPosition;
            _rackSodaBrand = rackType;
            _breakroom = breakroom;
            _relativeBoundingBox = new Rectangle(0, 0, 77, 137);

            LoadContent();
        }

        
        public void LoadContent()
        {

            box = new Texture2D(_breakroom.Device, 1, 1);
            box.SetData(new[] { Color.White });

            _counterFont = _breakroom.Content.Load<SpriteFont>("Fonts/ScoreBoardFont_41pt");
        }

        public void ApplyPhysics(GameTime gameTime)
        {
                
        }

        public void Update(GameTime gameTime)
        {
            
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(box, _position, _relativeBoundingBox, Color.Blue);
            spriteBatch.DrawStringCentered(_counterFont, canCount.ToString("D2"), _position, Color.Red);
            //spriteBatch.DrawString(_counterFont, canCount.ToString("D2"), _position, Color.Red, 0,
            //                       new Vector2(-16,128), 1, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public void OnDeposited(SodaCan can)
        {
            canCount++;
        }

    }

    internal static class SpriteBatchExtensions
    {
        public static void DrawStringCentered(this SpriteBatch spriteBatch, SpriteFont spriteFont, String text, Vector2 position, Color color)
        {
            Vector2 textBounds = spriteFont.MeasureString(text);
            Single centerX = position.X - textBounds.X * 0.5f;

            spriteBatch.DrawString(spriteFont, text, new Vector2(centerX, position.Y), color, 0, new Vector2(-46, 128), 1, SpriteEffects.None, 0);
        }
    }
    
}
