using System;

namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    public abstract class GpioBase : IGpio
    {
        /// <summary>
        /// The current pin naming.
        /// </summary>
        private PinNaming currentPinNaming;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.GpioBase"/> class.
        /// </summary>
        /// <param name="pinNaming">Pin naming.</param>
        public GpioBase(PinNaming pinNaming)
        {
            this.currentPinNaming = pinNaming;
        }

        #region implement interface
        /// <summary>
        /// Gets the current pin naming.
        /// </summary>
        /// <value>The current pin naming.</value>
        public virtual PinNaming CurrentPinNaming
        {
            get
            {
                return this.currentPinNaming;
            }
        }

        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        public abstract void SetPinDirection(int pinNumber, GpioPinDirection direction);

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        public abstract GpioPinValue this[int pinNumber]{ get; set;}

        /// <summary>
        /// Set button mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="mode">Button mode.</param>
        public abstract void SetButtonMode(int pinNumber, ButtonMode mode);

        /// <summary>
        /// Releases all resource used by the <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> so the garbage collector can reclaim the
        /// memory that the <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> was occupying.</remarks>
        public void Dispose()
        {
            this.DisposeInstance();
        }
        #endregion implement inferface

        /// <summary>
        /// Disposes resources of current instance.
        /// </summary>
        protected virtual void DisposeInstance()
        {
        }

        /// <summary>
        /// Gets the target pin number.
        /// </summary>
        /// <returns>The target pin number.</returns>
        /// <param name="pinNumber">Pin number to map.</param>
        /// <param name="targetPinNaming">Target pin naming system</param>
        protected virtual int GetTargetPinNumber(int pinNumber, PinNaming targetPinNaming = PinNaming.WiringPi)
        {
            var mapping = Singleton<PinNumberMapping>.Instance;
            return mapping.MapPinNumber(this.CurrentPinNaming, targetPinNaming, pinNumber);
        }
    }
}

