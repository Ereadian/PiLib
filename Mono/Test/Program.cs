namespace Ereadian.RaspberryPi.Library.Hardware.Test
{
    using System;
    using System.Diagnostics;

    class MainClass
    {
        public static void Main(string[] args)
        {
            const int maxDelay = 10;
            var delay = maxDelay;
            var pins = new int[]{5, 17};
            var buttonPin = 18;
            var values = new GpioPinValue[]{ GpioPinValue.Low, GpioPinValue.High };
            var value = 0;
            using (var gpio = new Gpio( PinNaming.BCM))
            {
                foreach (var pin in pins)
                {
                    gpio.SetPinDirection(pin, GpioPinDirection.Output);
                }
                gpio.SetButtonMode(buttonPin, ButtonMode.PudUp);

                var delayCount = 0;
                while (!Console.KeyAvailable)
                {
                    if ( -- delayCount < 1)
                    {
                        gpio[pins[0]] = values[value];
                        value = 1 - value;
                        gpio[pins[1]] = values[value];
                        delayCount = delay;
                    }

                    if (gpio[buttonPin] == GpioPinValue.Low)
                    {
                        if (--delay < 1)
                        {
                            delay = maxDelay;
                        }
                    }

                    System.Threading.Thread.Sleep(50);
                }
            }
        }
    }
}
