using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputController;

namespace SoftProGame2014
{
    class ScrollingBackground
    {
        //TODO: Add collection to hold any number of textures.
        public Texture2D texture1;
        public Texture2D texture2;
        public Rectangle rectangle1;
        public Rectangle rectangle2;

        public ScrollingBackground(Texture2D newTexture1, Texture2D newTexture2)
        {
            texture1 = newTexture1;
            texture2 = newTexture2;
            rectangle1 = new Rectangle(0, 0, 800, 500);
            rectangle2 = new Rectangle(800, 0, 800, 500);
        }

        public void Update(int speed, bool scrollOn)
        {
            // Background scroller areas.
            if (rectangle1.X + rectangle1.Width <= 0)
            {
                rectangle1.X = rectangle2.X + rectangle2.Width;
            }
            if (rectangle2.X + rectangle2.Width <= 0)
            {
                rectangle2.X = rectangle1.X + rectangle1.Width;
            }

            // If character has passed our "push line" scroll backgrounds.
            if (scrollOn) 
            {
                rectangle1.X -= speed;
                rectangle2.X -= speed;
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(texture1, rectangle1, Color.White);
            spriteBatch.Draw(texture2, rectangle2, Color.White);
        }

    }
}
