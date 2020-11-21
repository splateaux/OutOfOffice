using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SoftProGameWindows
{
    /// <summary>
    /// Loads and makes available point values for objects
    /// </summary>
    class ObjectValueManager
    {
        private Dictionary<string, int> _objectValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectValueManager"/> class.
        /// </summary>
        public ObjectValueManager()
        {
            _objectValues = new Dictionary<string, int>();

            // Load the collection from the app.config file
            NameValueCollection collection = ConfigurationManager.GetSection("PointValues") as NameValueCollection;

            foreach (var pair in collection.AllKeys.SelectMany(collection.GetValues, (k, v) => new { key = k, value = v }))
            {
                _objectValues.Add(pair.key, Convert.ToInt32(pair.value));
            }
        }

        /// <summary>
        /// Gets the point value for a given object
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int GetValue(string name)
        {
            return _objectValues[name];
        }
    }

}
