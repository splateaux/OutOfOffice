using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SoftProGameWindows
{
    /// <summary>
    /// The camera that follows the field of view within a game.
    /// </summary>
    public class Camera
    {
        private readonly Viewport _viewport;
        private Vector2 _position;
        private Rectangle? _limits;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public Camera(Viewport viewport)
        {
            this._viewport = viewport;
            this.Origin = new Vector2(this._viewport.Width / 2.0f, this._viewport.Height / 2.0f);
            this.Zoom = 1.0f;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector2 Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;

                // If there's a limit set and there's no zoom or rotation clamp the position
                if (this.Limits != null && this.Zoom == 1.0f && this.Rotation == 0.0f)
                {
                    this._position.X = MathHelper.Clamp(this._position.X, this.Limits.Value.X, this.Limits.Value.X + this.Limits.Value.Width - this._viewport.Width);
                    this._position.Y = MathHelper.Clamp(this._position.Y, this.Limits.Value.Y, this.Limits.Value.Y + this.Limits.Value.Height - this._viewport.Height);
                }
            }
        }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        public float Zoom { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the limits of the camera.
        /// </summary>
        /// <value>
        /// The limits.
        /// </value>
        public Rectangle? Limits
        {
            get
            {
                return this._limits;
            }
            set
            {
                if (value != null)
                {
                    // Assign limit but make sure it's always bigger than the viewport
                    this._limits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = Math.Max(this._viewport.Width, value.Value.Width),
                        Height = Math.Max(this._viewport.Height, value.Value.Height)
                    };

                    // Validate camera position with new limit
                    this.Position = this.Position;
                }
                else
                {
                    this._limits = null;
                }
            }
        }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <param name="parallax">The parallax.</param>
        /// <returns></returns>
        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-this.Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-this.Origin, 0.0f)) *
                   Matrix.CreateRotationZ(this.Rotation) *
                   Matrix.CreateScale(this.Zoom, this.Zoom, 1.0f) *
                   Matrix.CreateTranslation(new Vector3(this.Origin, 0.0f));
        }

        /// <summary>
        /// Looks at the specified position with the camera.
        /// </summary>
        /// <param name="position">The position.</param>
        public void LookAt(Vector2 position)
        {
            this.Position = position - new Vector2(this._viewport.Width / 2.0f, this._viewport.Height / 2.0f);
        }

        /// <summary>
        /// Moves the camera the specified displacement.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        /// <param name="respectRotation">if set to <c>true</c> respect rotation.</param>
        public void Move(Vector2 displacement, bool respectRotation = false)
        {
            if (respectRotation)
            {
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
            }

            this.Position += displacement;
        }
    }
}