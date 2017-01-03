namespace Ereadian.RaspberryPi.Library.Hardware
{
    using System;

    [Flags]
    public enum JoystickEventType : byte
    {
        None = 0x00,
        Button = 0x01,
        Axis = 0x02,
        INIT = 0x80,
    }
}

