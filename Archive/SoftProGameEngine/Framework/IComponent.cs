using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoftProGameEngine.Framework
{
    /// <summary>
    /// A generic interface that corresponds to a drawable/updatable component.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Draws the component.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        void Update(GameTime gameTime);
    }
}