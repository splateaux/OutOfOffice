using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Utilities for various drawing methods.
    /// </summary>
    public static class DrawingUtils
    {
        /// <summary>
        /// Draws a border around the specified rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="rectangleToDraw">The rectangle to draw.</param>
        /// <param name="thicknessOfBorder">The thickness of border.</param>
        /// <param name="borderColor">Color of the border.</param>
        public static void DrawBorder(SpriteBatch spriteBatch, Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });

            // Draw top line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="rectangle">The record.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="solid">if set to <c>true</c> draw a solid rectangle.</param>
        /// <param name="thickness">The thickness of the border.</param>
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Texture2D texture, Color color, bool solid, int thickness)
        {
            if (!solid)
            {

                Vector2 Position = new Vector2(rectangle.X, rectangle.Y);
                int border = thickness;

                int borderWidth = (int)(rectangle.Width) + (border * 2);
                int borderHeight = (int)(rectangle.Height) + (border);

                DrawStraightLine(spriteBatch, new Vector2((int)rectangle.X, (int)rectangle.Y), new Vector2((int)rectangle.X + rectangle.Width, (int)rectangle.Y), texture, color, thickness); //top bar 
                DrawStraightLine(spriteBatch, new Vector2((int)rectangle.X, (int)rectangle.Y + rectangle.Height), new Vector2((int)rectangle.X + rectangle.Width, (int)rectangle.Y + rectangle.Height), texture, color, thickness); //bottom bar 
                DrawStraightLine(spriteBatch, new Vector2((int)rectangle.X, (int)rectangle.Y), new Vector2((int)rectangle.X, (int)rectangle.Y + rectangle.Height), texture, color, thickness); //left bar 
                DrawStraightLine(spriteBatch, new Vector2((int)rectangle.X + rectangle.Width, (int)rectangle.Y), new Vector2((int)rectangle.X + rectangle.Width, (int)rectangle.Y + rectangle.Height), texture, color, thickness); //right bar 
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Vector2((float)rectangle.X, (float)rectangle.Y), rectangle, color, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draws a straight line.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="A">The A vertex.</param>
        /// <param name="B">The B vertex.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The line thickness.</param>
        public static void DrawStraightLine(SpriteBatch spriteBatch, Vector2 A, Vector2 B, Texture2D texture, Color color, int thickness)
        {
            Rectangle rectangle;

            if (A.X < B.X) // horizontal line 
            {
                rectangle = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
            }
            else //vertical line 
            {
                rectangle = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
            }

            spriteBatch.Draw(texture, rectangle, color);
        }

        /// <summary>
        /// Draws the shadowed string.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="font">The font.</param>
        /// <param name="value">The value.</param>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
        {
            if (string.IsNullOrEmpty(value)) return;

            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
