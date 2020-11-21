using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SoftProGameWindows
{
    /// <summary>
    /// Provides configuration for each of the levels of the game.
    /// </summary>
    public class LevelConfiguration
    {
        /// <summary>
        /// The list of levels of the game.
        /// </summary>
        public List<LevelInfo> Levels;
    }

    /// <summary>
    /// Provides configuration information for a given level.
    /// </summary>
    public class LevelInfo
    {
        /// <summary>
        /// The maximum allowable time in seconds to complete the level.
        /// </summary>
        public int AllowableTimeInSeconds;

        /// <summary>
        /// The content manager path to the level soundtrack.
        /// </summary>
        public string SoundTrack;

        /// <summary>
        /// The width of the level.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the level.
        /// </summary>
        public int Height;

        /// <summary>
        /// The X-axis view margin for level. It controls where and when camera panning will begin along the x-axis.
        /// </summary>
        public float XAxisViewMargin;

        /// <summary>
        /// The Y-axis view margin for level. It controls where and when camera panning will begin along the y-axis.
        /// </summary>
        public float YAxisViewMargin;

        /// <summary>
        /// Gets or sets the player start position.
        /// </summary>
        /// <value>
        /// The player start position.
        /// </value>
        [XmlIgnore]
        public Vector2 PlayerStartPosition;

        /// <summary>
        /// The player's start position represented as a string for serialization.
        /// </summary>
        [XmlElement("PlayerStartPosition")]
        public string PlayerStartPositionString
        {
            get { return this.PlayerStartPosition.ToString(); }
            set { this.PlayerStartPosition = Utils.Vector2Utils.Parse(value); }
        }

        /// <summary>
        /// The index of the layer where the player will be drawn.
        /// </summary>
        public int PlayerLevelIndex;

        /// <summary>
        /// The individual layers that compose the viewable display of the level.
        /// </summary>
        /// <remarks>
        /// Layers are rendered in list order, thus the last layer in the list
        /// will be the top layer or foreground.
        /// </remarks>
        public List<LayerInfo> Layers;
    }

    /// <summary>
    /// Provides configuration information for a particular layer of a given level.
    /// </summary>
    public class LayerInfo
    {
        /// <summary>
        /// The rate of scroll for the layer.
        /// </summary>
        public float ScrollRate;

        /// <summary>
        /// The sprites to be drawn on the layer.  They will be drawn in list order.
        /// </summary>
        public List<SpriteInfo> Sprites;
    }

    /// <summary>
    /// Provides configuration information for a particular sprite.
    /// </summary>
    public class SpriteInfo
    {
        /// <summary>
        /// The fully qualified type name of the sprite.
        /// </summary>
        [XmlIgnore]
        public Type SpriteType;

        /// <summary>
        /// The fully qualified type name of the sprite represented as a string for serialization.
        /// </summary>
        [XmlElement("SpriteType")]
        public string SpriteTypeString
        {
            get { return this.SpriteType.FullName; }
            set { this.SpriteType = Type.GetType(value); }
        }

        /// <summary>
        /// The initial position of the sprite.
        /// </summary>
        [XmlIgnore]
        public Vector2 Position;

        /// <summary>
        /// The initial position of the sprite represented as a string for serialization.
        /// </summary>
        [XmlElement("Position")]
        public string PositionString
        {
            get { return this.Position.ToString(); }
            set { this.Position = Utils.Vector2Utils.Parse(value); }
        }

        /// <summary>
        /// The content path to the texture file.
        /// </summary>
        public string Texture;

        /// <summary>
        /// The color to tint the sprite.
        /// </summary>
        [XmlIgnore]
        public Color TintColor;

        /// <summary>
        /// The color to tint the sprite represented as a string for serialization.
        /// </summary>
        [XmlElement("TintColor")]
        public string TintColorString
        {
            get { return this.TintColor.ToString(); }
            set { this.TintColor = Utils.ColorUtils.Parse(value); }
        }

        /// <summary>
        /// The rectangle that specifies (in texels) the source texels from a texture.
        /// </summary>
        [XmlIgnore]
        public Rectangle? SourceRectangle;

        /// <summary>
        /// The rectangle that specifies (in texels) the source texels from a texture represented as a string for serialization.
        /// </summary>
        [XmlElement("SourceRectangle")]
        public string SourceRectangleString
        {
            get { return this.SourceRectangle.ToString(); }
            set { this.SourceRectangle = Utils.RectangleUtils.Parse(value); }
        }

        /// <summary>
        /// Additional settings as applicable by sprite type.
        /// </summary>
        public List<Setting> Settings;
    }

    /// <summary>
    /// Settings necessary by each sprite type.
    /// </summary>
    public class Setting
    {       
        /// <summary>
        /// The settings value (e.g. Impassable)
        /// </summary>
        [XmlAttribute]
        public string Value;

        /// <summary>
        /// The settings key (e.g. CollidableType).
        /// </summary>
        [XmlAttribute]
        public string Key;
    }
}
