namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Local GPIO operation support
    /// </summary>
    public class LocalGpio : GpioBase
    {
        /// <summary>
        /// Initializes the <see cref="Ereadian.RaspberryPi.Library.Hardware.LocalGpio"/> class.
        /// </summary>
        static LocalGpio()
        {
            Trace.TraceInformation("Initializating Raspberry Pi GPIO");
            if (wiringPiSetup() == -1)
            {
                const string errorMessage = "Failed to initialize Raspberry Pi GPIO";
                Trace.TraceError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            Trace.TraceInformation("Raspberry GPIO initializated");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.LocalGpio"/> class.
        /// </summary>
        public LocalGpio() : this(PinNaming.BCM)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.LocalGpio"/> class.
        /// </summary>
        /// <param name="pinNaming">Pin naming systerm.</param>
        public LocalGpio(PinNaming pinNaming) : base(pinNaming)
        {
        }

        #region abstract class implementation
        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        public override void SetPinDirection(int pinNumber, GpioPinDirection direction)
        {
            pinMode(this.GetTargetPinNumber(pinNumber), (int)direction);
        }

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        public override GpioPinValue this[int pinNumber]
        {
            get
            {
                return digitalRead(this.GetTargetPinNumber(pinNumber)) == 0 ? GpioPinValue.Low : GpioPinValue.High;
            }
            set
            {
                digitalWrite(this.GetTargetPinNumber(pinNumber), (int)value);
            }
        }

        /// <summary>
        /// Set button mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="mode">Button mode.</param>
        public override void SetButtonMode(int pinNumber, ButtonMode mode)
        {
            pullUpDnControl(this.GetTargetPinNumber(pinNumber), (int)mode);
        }
        #endregion abstract class implementation

        #region import functions
        /// <summary>
        /// Wirings pi setup.
        /// </summary>
        /// <returns>The pi setup.</returns>
        [DllImport("wiringPi.so")]
        private static extern int wiringPiSetup();

        /// <summary>
        /// Set pin mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="pinMode">Pin mode. 0: inpit, other: output</param>
        [DllImport("wiringPi.so")]
        private static extern void pinMode(int pinNumber, int pinMode);

        /// <summary>
        /// Read pin value.
        /// </summary>
        /// <returns>The value from the pin. 0: LoW, other: High</returns>
        /// <param name="pinNumber">Pin number.</param>
        [DllImport("wiringPi.so")]
        private static extern int digitalRead(int pinNumber);

        /// <summary>
        /// Write pin value.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="value">Value to write.</param>
        [DllImport("wiringPi.so")]
        private static extern void digitalWrite(int pinNumber, int value);

        /// <summary>
        /// Set button mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="pud">Pud. 0: off, 1: PudDown 2: PudUp</param>
        [DllImport("wiringPi.so")]
        private static extern void pullUpDnControl(int pinNumber, int pud);

        [DllImport("wiringPi.so")]
        private static extern void pwmWrite(int pinNumber, int value);

        [DllImport("wiringPi.so")]
        private static extern int analogRead(int pinNumber);

        [DllImport("wiringPi.so")]
        private static extern void analogWrite(int pinNumber, int value);
        #endregion import functions
    }
}

