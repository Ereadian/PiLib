namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;

    /// <summary>
    /// IC 74HC959 operation
    /// </summary>
    /// <remarks>>
    /// http://www.ti.com/product/SN74HC595
    ///     +-- --+
    ///  Q1 | +-+ | Vcc
    ///  Q2 |     | Q0
    ///  Q3 |     | DS
    ///  Q4 |     | OE
    ///  Q5 |     | ST CP
    ///  Q6 |     | SH CP
    ///  Q7 |     | MR
    /// GND |     | Q7`
    ///     +-----+
    /// 0:high bit, 7:low bit
    /// </remarks>
    public class SN74HC595
    {
        private readonly IGpio gpio;
        private readonly int dsPinNumber;
        private readonly int stPinNumber;
        private readonly int shPinNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.GPIO.SN74HC595"/> class.
        /// </summary>
        /// <param name="gpio">gpio instance</param>
        /// <param name="dsPinNumber">pin number which connected to DS (SDI serial data input)</param>
        /// <param name="shPinNumber">pin number which connected to SH CP (shift register clock input).</param>
        /// <param name="stPinNumber">pin number which connected to ST CP (RCLK emory clock input).</param>
        public SN74HC595(IGpio gpio, int dsPinNumber, int shPinNumber, int stPinNumber)
        {
            this.gpio = gpio;
            this.dsPinNumber = dsPinNumber;
            this.stPinNumber = stPinNumber;
            this.shPinNumber = shPinNumber;

            gpio.SetPinDirection(dsPinNumber, GpioPinDirection.Output);
            gpio.SetPinDirection(stPinNumber, GpioPinDirection.Output);
            gpio.SetPinDirection(shPinNumber, GpioPinDirection.Output);

            this.gpio[dsPinNumber] = GpioPinValue.Low;
            this.gpio[stPinNumber] = GpioPinValue.Low;
            this.gpio[shPinNumber] = GpioPinValue.Low;
        }

        public void Send(byte data)
        {
            byte mask = 0x80;
            for (var i = 0; i < 8; i++)
            {
                this.gpio[dsPinNumber] = ((data & mask) == 0) ? GpioPinValue.Low : GpioPinValue.High;
                this.Pulse(shPinNumber);
                mask = (byte)(mask >> 1);
            }

            this.Pulse(stPinNumber);
        }

        private void Pulse(int pinNumber)
        {
            this.gpio[pinNumber] = GpioPinValue.Low;
            this.gpio[pinNumber] = GpioPinValue.High;
        }
    }
}

