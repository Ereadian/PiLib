namespace Ereadian.RaspberryPi.Library.Hardware.Joysticks
{
    using System;

    /// <summary>
    /// Joystick operation interface
    /// </summary>
    public interface IJoystick : IDisposable
    {
        JoystickEvent GetEvent();
    }
}

