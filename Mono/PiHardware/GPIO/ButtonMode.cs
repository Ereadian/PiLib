using System;

namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    /// <summary>
    /// Button mode.
    /// </summary>
    public enum ButtonMode : byte
    {
        /// <summary>
        /// Turn off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// The pud down.
        /// </summary>
        PudDown = 1,

        /// <summary>
        /// Pull up to 3.3V, make GPIO pin a stable level
        /// </summary>
        PudUp = 2,
    }
}

