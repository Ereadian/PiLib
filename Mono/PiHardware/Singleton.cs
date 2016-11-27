namespace Ereadian.RaspberryPi.Library.Hardware
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Singleton support
    /// </summary>
    public static class Singleton<T> where T: new()
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                return Inner.Instance;
            }        
        }

        /// <summary>
        /// Inner class for instance creation and store
        /// </summary>
        private static class Inner
        {
            /// <summary>
            /// Initializes the <see cref="Ereadian.RaspberryPi.Library.Hardware.Singleton`1+Inner"/> class.
            /// </summary>
            static Inner()
            {
                try
                {
                    Instance = new T();
                }
                catch(Exception exception)
                {
                    var errorMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to create singleton instance for type {0}. Exception: {1}",
                        typeof(T).FullName,
                        exception.ToString());
                    Trace.TraceError(errorMessage);
                    throw;
                }
            }

            internal static T Instance {get; private set;}
        }
    }
}

