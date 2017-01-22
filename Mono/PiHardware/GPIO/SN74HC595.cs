namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;

    /// <summary>
    /// IC 74HC959 operation
    /// </summary>
    /// <remarks>>
    /// http://www.ti.com/product/SN74HC595
    ///     +-- --+
    ///  Qb | +-+ | Vcc
    ///  Qc |     | Qa
    ///  Qd |     | DS (Data line)
    ///  Qe |     | OE
    ///  Qf |     | ST CP (shift lock)
    ///  Qg |     | SH CP (memory lock)
    ///  Qh |     | MR
    /// GND |     | Qh`
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
        /// <param name="stPinNumber">pin number which connected to ST CP (RCLK memory clock input).</param>
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

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="fromHighToLow">send bits from high to low (bit 8 to 1)</param>
        /// <remarks>>
        /// if fromHighToLow is true, the high bit will be shifted first. In another words, the mapping to Chip pin is:
        /// Sent bit:  7  6  5  4  3  2  1  0
        /// Chip pin: Qa Qb Qc Qd Qe Qf Qg Qh
        /// if fromHighToLow is false, the low bit will be shifted first. In another words, the mapping to Chip pin is:
        /// Sent bit:  7  6  5  4  3  2  1  0
        /// Chip pin: Qh Qg Qf Qe Qd Qc Qb Qa
        /// </remarks>
        public void Send(byte data, bool fromHighToLow = true)
        {
            if (fromHighToLow)
            {
                SendByteFromHighToLow(data);
            }
            else
            {
                SendByteFromLowToHigh(data);
            }

            this.Pulse(stPinNumber);
        }

        /// <summary>
        /// Sends the byte from high bits to low bits.
        /// </summary>
        /// <param name="data">Data to send</param>
        private void SendByteFromHighToLow(byte data)
        {
            byte mask = 0x80;
            for (var i = 0; i < 8; i++)
            {
                this.gpio[this.dsPinNumber] = ((data & mask) == 0) ? GpioPinValue.Low : GpioPinValue.High;
                this.Pulse(this.shPinNumber);
                mask = (byte)(mask >> 1);
            }
        }

        /// <summary>
        /// Sends the byte from high bits to low bits.
        /// </summary>
        /// <param name="data">Data to send</param>
        private void SendByteFromLowToHigh(byte data)
        {
            byte mask = 0x01;
            for (var i = 0; i < 8; i++)
            {
                this.gpio[this.dsPinNumber] = ((data & mask) == 0) ? GpioPinValue.Low : GpioPinValue.High;
                this.Pulse(this.shPinNumber);
                mask = (byte)(mask << 1);
            }
        }

        /// <summary>
        /// Pulse the specified pinNumber.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <remarks>>Send a signal to chip</remarks>
        private void Pulse(int pinNumber)
        {
            this.gpio[pinNumber] = GpioPinValue.Low;
            this.gpio[pinNumber] = GpioPinValue.High;
        }
    }
}

