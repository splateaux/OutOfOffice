using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Provides extension methods for a service provider.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to locate.</typeparam>
        /// <param name="site">The <see cref="IServiceProvider"/>.</param>
        /// <returns>A service object of type <paramref name="T" />.-or- null if there is no service object of type <paramref name="T" />.</returns>
        public static T GetService<T>(this IServiceProvider site)
        {
            if (site != null)
            {
                return (T)site.GetService(typeof(T));
            }

            return default(T);
        }
    }
}
