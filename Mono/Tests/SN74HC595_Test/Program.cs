namespace SN74HC595_Test
{
    using System;
    using System.Threading;
    using Ereadian.RaspberryPi.Library.Hardware.GPIO;

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var gpio = new Gpio(PinNaming.BCM))
            {
                var sn74hc595 = new SN74HC595(gpio, 17, 27, 18);
                Console.WriteLine("Press any key to exit...");

                bool fromHighToLow = false;
                while (!Console.KeyAvailable)
                {
                    byte mask = 1;
                    for (var i = 0; i < 8; i++)
                    {
                        sn74hc595.Send(mask, fromHighToLow);
                        Thread.Sleep(500);
                        mask = (byte)(mask << 1);
                    }
                    mask = 0x80;
                    for (var i = 0; i < 8; i++)
                    {
                        sn74hc595.Send(mask, fromHighToLow);
                        Thread.Sleep(500);
                        mask = (byte)(mask >> 1);
                    }

                    for (var i = 0; i < 2; i++)
                    {
                        sn74hc595.Send(0xFF, fromHighToLow);
                        Thread.Sleep(500);
                        sn74hc595.Send(0x00, fromHighToLow);
                        Thread.Sleep(500);
                    }
                }
            }
        }
    }
}
