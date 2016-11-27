namespace Ereadian.RaspberryPi.Library.Hardware
{
	using System;

    /// <summary>
    /// GPIO operation interface
    /// </summary>
    public interface IGpio : IDisposable
	{
        /// <summary>
        /// Gets the current pin naming.
        /// </summary>
        /// <value>The current pin naming.</value>
        PinNaming CurrentPinNaming { get; }

        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        void SetPinDirection(int pinNumber, GpioPinDirection direction);

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        GpioPinValue this[int pinNumber]{ get; set;}
	}
}
