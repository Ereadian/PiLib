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
                while (!Console.KeyAvailable)
                {
                    byte mask = 1;
                    for (var i = 0; i < 8; i++)
                    {
                        sn74hc595.Send(mask);
                        Thread.Sleep(500);
                        mask = (byte)(mask << 1);
                    }
                    mask = 0x80;
                    for (var i = 0; i < 8; i++)
                    {
                        sn74hc595.Send(mask);
                        Thread.Sleep(500);
                        mask = (byte)(mask >> 1);
                    }

                    for (var i = 0; i < 2; i++)
                    {
                        sn74hc595.Send(0xFF);
                        Thread.Sleep(500);
                        sn74hc595.Send(0x00);
                        Thread.Sleep(500);
                    }
                }
            }
        }
    }
}
