using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftProGameWindows
{
    /// <summary>
    /// General utility methods.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Utility functions for Vector2 types.
        /// </summary>
        internal static class Vector2Utils
        {
            /// <summary>
            /// Converts the string representation of a vector to its <see cref="Vector2" /> equivalent.
            /// </summary>
            /// <param name="vector">The vector.</param>
            /// <param name="s">The string containing a vector to convert.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException">s is not in the correct format.</exception>
            /// <remarks>
            /// Format is: {X:0 Y:0}
            /// </remarks>
            public static Vector2 Parse(string s)
            {
                if (s == null) throw new ArgumentNullException();
                var re = new Regex(@"^\s*\{X\:(-?\d+)\s+Y\:(-?\d+)\}\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var match = re.Match(s);
                if (match == null) throw new FormatException("s is not in the correct format.");
                var x = Int32.Parse(match.Groups[1].Value);
                var y = Int32.Parse(match.Groups[2].Value);
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// Utility functions for Vector2 types.
        /// </summary>
        internal static class Vector4Utils
        {
            /// <summary>
            /// Converts the string representation of a vector to its <see cref="Vector2" /> equivalent.
            /// </summary>
            /// <param name="vector">The vector.</param>
            /// <param name="s">The string containing a vector to convert.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException">s is not in the correct format.</exception>
            /// <remarks>
            /// Format is: {X:0 Y:0 Z:0 W:0}
            /// </remarks>
            public static Vector4 Parse(string s)
            {
                if (s == null) throw new ArgumentNullException();
                var re = new Regex(@"^\s*\{X\:(-?\d+)\s+Y\:(-?\d+)\s+Z\:(-?\d+)\s+W\:(-?\d+)\}\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var match = re.Match(s);
                if (match == null) throw new FormatException("s is not in the correct format.");
                var x = Int32.Parse(match.Groups[1].Value);
                var y = Int32.Parse(match.Groups[2].Value);
                var z = Int32.Parse(match.Groups[3].Value);
                var w = Int32.Parse(match.Groups[4].Value);
                return new Vector4(x, y, z, w);
            }
        }

        /// <summary>
        /// Utility functions for Rectangle types.
        /// </summary>
        internal static class RectangleUtils
        {
            /// <summary>
            /// Converts the string representation of a rectangle to its <see cref="Microsoft.Xna.Framework.Rectangle" /> equivalent.
            /// </summary>
            /// <param name="rect">The rect.</param>
            /// <param name="s">The string containing a rectangle to convert.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException">s is not in the correct format.</exception>
            /// <remarks>
            /// Format is: {X:0 Y:0 Width:0 Height:0}
            /// </remarks>
            public static Rectangle Parse(string s)
            {
                if (s == null) throw new ArgumentNullException();
                var re = new Regex(@"^\s*\{X\:(-?\d+)\s+Y\:(-?\d+)\s+Width\:(-?\d+)\s+Height\:(-?\d+)\}\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var match = re.Match(s);
                if (match == null) throw new FormatException("s is not in the correct format.");
                var x = Int32.Parse(match.Groups[1].Value);
                var y = Int32.Parse(match.Groups[2].Value);
                var width = Int32.Parse(match.Groups[3].Value);
                var height = Int32.Parse(match.Groups[4].Value);
                return new Rectangle(x, y, width, height);
            }
        }

        /// <summary>
        /// Utility functions for Color types.
        /// </summary>
        internal static class ColorUtils
        {
            /// <summary>
            /// Parses the specified s.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException">s is not in the correct format.</exception>
            public static Color Parse(string s)
            {
                if (s == null) throw new ArgumentNullException();
                var re = new Regex(@"^\s*\{R\:(-?\d+)\s+G\:(-?\d+)\s+B\:(-?\d+)\s+A\:(-?\d+)\}\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var match = re.Match(s);
                if (match == null) throw new FormatException("s is not in the correct format.");
                var r = Int32.Parse(match.Groups[1].Value);
                var g = Int32.Parse(match.Groups[2].Value);
                var b = Int32.Parse(match.Groups[3].Value);
                var a = Int32.Parse(match.Groups[4].Value);
                return new Color(r, g, b, a);
            }
        }
    }
}
