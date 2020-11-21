using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// A basic representation of a game object.
    /// </summary>
    public class Sprite : IServiceProvider
    {
        private IServiceProvider _site;
        private Vector2 _position;
        private Texture2D _texture;
        private Rectangle? _sourceRectangle;
        private Color _tintColor;
        private float _rotation;
        private Vector2 _origin;
        private float _scale;
        private SpriteEffects _effects;
        private float _layerDepth;
        private Layer _layer;
        private SpriteInfo _info;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.</param>
        public Sprite(IServiceProvider site, Vector2 position, Texture2D texture = null, Rectangle? sourceRectangle = null)
            : this(site, position, texture, sourceRectangle, Color.White, 0.0f)
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.</param>
        /// <param name="tint">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        public Sprite(IServiceProvider site, Vector2 position, Texture2D texture, Rectangle? sourceRectangle, Color tint, float rotation = 0.0f)
            : this(site, position, texture, sourceRectangle, tint, rotation, Vector2.Zero)
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="position">The location (in screen coordinates) to draw the sprite.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="sourceRectangle">A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.</param>
        /// <param name="tint">The color to tint a sprite. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">Specifies the angle (in radians) to rotate the sprite about its center.</param>
        /// <param name="origin">The sprite origin; the default is (0,0) which represents the upper-left corner.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="effects">Effects to apply.</param>
        public Sprite(IServiceProvider site, Vector2 position, Texture2D texture, Rectangle? sourceRectangle, Color tint, float rotation,
            Vector2 origin, float scale = 0.0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            this._site = site;
            this._position = position;
            this._texture = texture;
            this._sourceRectangle = sourceRectangle;
            this._tintColor = tint;
            this._rotation = rotation;
            this._origin = origin;
            this._scale = scale;
            this._effects = effects;
            this._layerDepth = layerDepth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="info">The sprite information.</param>
        public Sprite(SpriteInfo info)
        {
            this._info = info;
            this._position = info.Position;
            this._sourceRectangle = info.SourceRectangle;
            this._tintColor = info.TintColor;
        }

        /// <summary>
        /// Gets or sets the location (in screen coordinates) to draw the sprite.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        /// <remarks>
        /// This location represents the upper-left hand corner.
        /// </remarks>
        public Vector2 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }

        /// <summary>
        /// Gets the texture (image) of the sprite.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture2D Texture
        {
            get { return this._texture; }
            set { this._texture = value; }
        }

        /// <summary>
        /// Gets the rectangle that specifies (in texels) the source texels from a texture.
        /// </summary>
        /// <value>
        /// The source rectangle.
        /// </value>
        /// <remarks>
        /// Use null to draw the entire texture. This rectangle is aligned on the upper-left hand corner.
        /// </remarks>
        public virtual Rectangle? SourceRectangle
        {
            get { return this._sourceRectangle; }
            set { this._sourceRectangle = value; }
        }

        /// <summary>
        /// Gets the color to tint the sprite.
        /// </summary>
        /// <value>
        /// The color of the tint.
        /// </value>
        /// <remarks>
        /// Use Color.White for full color with no tinting.
        /// </remarks>
        public Color TintColor
        {
            get { return this._tintColor; }
            set { this._tintColor = value; }
        }

        /// <summary>
        /// Gets or sets the angle (in radians) to rotate the sprite about its center.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public float Rotation
        {
            get { return this._rotation; }
            set { this._rotation = value; }
        }

        /// <summary>
        /// Gets or sets the sprite origin.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        /// <remarks>
        /// The default is (0,0) which represents the upper-left corner.
        /// </remarks>
        public Vector2 Origin
        {
            get { return this._origin; }
            set { this._origin = value; }
        }

        /// <summary>
        /// Gets or sets the scale factor.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public float Scale
        {
            get { return this._scale; }
            set { this._scale = value; }
        }

        /// <summary>
        /// Gets or sets the effects to apply.
        /// </summary>
        /// <value>
        /// The effects.
        /// </value>
        public SpriteEffects Effects
        {
            get { return this._effects; }
            set { this._effects = value; }
        }

        /// <summary>
        /// Gets or sets the layer where the sprite will be drawn.
        /// </summary>
        /// <value>
        /// The layer.
        /// </value>
        public Layer Layer
        {
            get { return this._layer; }
            set { this._layer = value; }
        }

        /// <summary>
        /// Gets or sets the depth within a layer.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        /// <remarks>
        /// By default, 0 represents the front of the layer and 1 represents a back of the layer.
        /// Use SpriteSortMode if you want sprites to be sorted during drawing.
        /// </remarks>
        public float LayerDepth
        {
            get { return this._layerDepth; }
            set { this._layerDepth = value; }
        }

        /// <summary>
        /// Loads the content for the sprite.
        /// </summary>
        /// <param name="content">The content mananger.</param>
        public virtual void LoadContent(ContentManager content)
        {
            if ((this.Texture == null) && (this._info != null) && (!string.IsNullOrWhiteSpace(this._info.Texture)))
            {
                this.Texture = content.Load<Texture2D>(this._info.Texture);
            }
        }

        /// <summary>
        /// Unloads the content of the sprite.
        /// </summary>
        public virtual void UnloadContent()
        {
        }

        /// <summary>
        /// Handles input, performs physics, and animates the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="input">The input.</param>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame.
        /// </remarks>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Allows the sprite to handle user input.
        /// </summary>
        /// <param name="input">The input.</param>
        public virtual void HandleInput(InputState input)
        {
        }

        /// <summary>
        /// Draws the sprite.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (this.Texture != null)
            {
                spriteBatch.Draw(this.Texture, this.Position, this.SourceRectangle, Color.White);

                //spriteBatch.Draw(this.Texture, this.Position, this.SourceRectangle,
                //    this.TintColor, this.Rotation, this.Origin,
                //    this.Scale, this.Effects, this.LayerDepth);

#if DEBUG
                // Draw debug outline
                if (ScreenManager.DEBUG_ENABLED)
                {
                    var boundingBox = this.SourceRectangle.HasValue ? this.SourceRectangle.Value : this.Texture.Bounds;
                    boundingBox = new Rectangle((int)this.Position.X, (int)this.Position.Y, boundingBox.Width, boundingBox.Height);
                    DrawingUtils.DrawBorder(spriteBatch, boundingBox, 1, Color.White);
                }
#endif
            }
        }

        /// <summary>
        /// Sets the site.
        /// </summary>
        /// <param name="site">The site.</param>
        public virtual void SetSite(IServiceProvider site)
        {
            this._site = site;
        }

        #region IServiceProvider Implementation

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (this._site != null)
            {
                return this._site.GetService(serviceType);
            }

            return null;
        }

        #endregion
    }
}
