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
    class SettingsManager
    {
        private Dictionary<string, string> _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectValueManager"/> class.
        /// </summary>
        public SettingsManager()
        {
            _settings = new Dictionary<string, string>();

            // Load the collection from the app.config file
            NameValueCollection collection = ConfigurationManager.GetSection("Settings") as NameValueCollection;

            foreach (var pair in collection.AllKeys.SelectMany(collection.GetValues, (k, v) => new { key = k, value = v }))
            {
                _settings.Add(pair.key, pair.value);
            }
        }

        /// <summary>
        /// Gets the point value for a given object
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            return _settings[name];
        }
    }

}
