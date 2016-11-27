namespace Ereadian.RaspberryPi.Library.Hardware.Test
{
    using System;

    class MainClass
    {
        public static void Main(string[] args)
        {
            var pins = new int[]{5, 17};
            var values = new GpioPinValue[]{ GpioPinValue.Low, GpioPinValue.High };
            var value = 0;
            using (var gpio = new RemoteGpio( PinNaming.BCM))
            {
                foreach (var pin in pins)
                {
                    gpio.SetPinDirection(pin, GpioPinDirection.Output);
                }

                while (!Console.KeyAvailable)
                {
                    gpio[pins[0]] = values[value];
                    value = 1 - value;
                    gpio[pins[1]] = values[value];
                    System.Threading.Thread.Sleep(200);
                }
            }
        }
    }
}
