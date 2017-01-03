using System;

namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    public class Gpio : IGpio
    {
        #if DEBUG
        private const bool UseLocal = false;
        #else
        private const bool UseLocal = true;
        #endif

        /// <summary>
        /// The implementation.
        /// </summary>
        private readonly IGpio implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> class.
        /// </summary>
        public Gpio() : this(PinNaming.BCM)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> class.
        /// </summary>
        /// <param name="pinNaming">Pin naming.</param>
        /// <param name="useLocal">If set to <c>true</c> use local GPIO operation.</param>
        public Gpio(PinNaming pinNaming, bool useLocal = UseLocal)
        {
            if (useLocal)
            {
                this.implementation = new LocalGpio(pinNaming);
            }
            else
            {
                this.implementation = new RemoteGpio(pinNaming);
            }
        }

        #region interface implementation
        /// <summary>
        /// Gets the current pin naming.
        /// </summary>
        /// <value>The current pin naming.</value>
        public PinNaming CurrentPinNaming
        {
            get
            {
                return this.implementation.CurrentPinNaming;
            }
        }

        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        public void SetPinDirection(int pinNumber, GpioPinDirection direction)
        {
            this.implementation.SetPinDirection(pinNumber, direction);
        }

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        public GpioPinValue this[int pinNumber]
        {
            get
            {
                return this.implementation[pinNumber];
            }
            set
            {
                this.implementation[pinNumber] = value;
            }
        }

        /// <summary>
        /// Set button mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="mode">Button mode.</param>
        public void SetButtonMode(int pinNumber, ButtonMode mode)
        {
            this.implementation.SetButtonMode(pinNumber, mode);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> so the garbage collector can reclaim the memory
        /// that the <see cref="Ereadian.RaspberryPi.Library.Hardware.Gpio"/> was occupying.</remarks>
        public void Dispose()
        {
            this.implementation.Dispose();
        }
        #endregion interface implementation
    }
}

