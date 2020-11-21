using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGame2014
{
    public class LocationEventArgs : EventArgs
    {
        public Vector2 Location { get; set; }

        public LocationEventArgs( Vector2 location)
        {
            Location = location;
        }
    }
}
