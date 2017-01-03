using System;

namespace Ereadian.RaspberryPi.Library.Hardware.Joysticks
{
    public class JoystickEvent
    {
        public JoystickEvent(uint eventTimestamp, int value, JoystickEventType eventType, int number)
        {
            this.EventTimestamp = eventTimestamp;
            this.Value = value;
            this.EventType = eventType;
            this.Number = number;
        }

        /// <summary>
        /// Event timestamp
        /// </summary>
        /// <value>The event time.</value>
        public uint EventTimestamp { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public int Value{get; private set;}

        /// <summary>
        /// Event type
        /// </summary>
        /// <value>The type of the event.</value>
        public JoystickEventType EventType { get; private set; }

        /// <summary>
        /// axis/button number
        /// </summary>
        /// <value>The number.</value>
        public int Number { get; private set; }
    }
}

