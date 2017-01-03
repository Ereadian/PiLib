namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;

    /// <summary>
    /// Pin naming 
    /// </summary>
    public enum PinNaming
    {
        /// <summary>
        /// The physical pin naming system
        /// </summary>
        Physical = 0,

        /// <summary>
        /// The WiringPi pin naming system
        /// </summary>
        WiringPi = 1,

        /// <summary>
        /// The BCM pin naming system
        /// </summary>
        BCM = 2
    }
}

