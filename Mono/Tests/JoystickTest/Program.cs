namespace JoystickTest
{
    using System;
    using System.Threading;
    using Ereadian.RaspberryPi.Library.Hardware.Joysticks;

    class MainClass
    {
        public static void Main(string[] args)
        {
            var deviceNames = Joystick.GetJoystickDeviceNames();
            if (deviceNames.Count < 1)
            {
                Console.WriteLine("No joystick");
                return;
            }

            using (var joystick = new Joystick(deviceNames[0]))
            {
                while (!Console.KeyAvailable)
                {
                    var data = joystick.GetEvent();
                    if (data == null)
                    {
                        Thread.Sleep(50);
                    }
                    else
                    {
                        Console.WriteLine("Time:{0} Type:{1}, Data:{2}, Dev id:{3}", 
                            data.EventTimestamp,
                            data.EventType, 
                            data.Value, 
                            data.Number);
                    }
                }

                Console.WriteLine("Exit");
            }
        }
    }
}
