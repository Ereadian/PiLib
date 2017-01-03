namespace Ereadian.RaspberryPi.Library.Hardware.Joysticks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Joystick event.
    /// </summary>
    public class JoystickEvent
    {
        /// <summary>
        /// Joystick event types
        /// </summary>
        private static IReadOnlyList<JoystickEventType> AllJoystickEventTypes;

        /// <summary>
        /// Type constructor
        /// </summary>
        static JoystickEvent()
        {
            var enventNames = Enum.GetNames(typeof(JoystickEventType));
            var eventTypes = new JoystickEventType[enventNames.Length];
            for (var i = 0; i < enventNames.Length; i++)
            {
                eventTypes[i] = (JoystickEventType)Enum.Parse(typeof(JoystickEventType), enventNames[i]);
            }

            AllJoystickEventTypes = eventTypes;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.JoystickEvent"/> class.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public JoystickEvent(byte[] eventData)
        {
            var timestamp = BitConverter.ToUInt32(eventData, 0);
            var value = BitConverter.ToInt16(eventData, 4);
            var type = eventData[6];
            int number = (int)eventData[7];

            var eventType = JoystickEventType.None;
            for (var i = 0; i < AllJoystickEventTypes.Count; i++)
            {
                var joystickEventType = AllJoystickEventTypes[i];
                if ((type & (byte)joystickEventType) != 0)
                {
                    eventType |= joystickEventType;
                }
            }

            this.EventTimestamp = timestamp;
            this.Value = value;
            this.EventType = eventType;
            this.Number = number;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.JoystickEvent"/> class.
        /// </summary>
        /// <param name="eventTimestamp">Event timestamp.</param>
        /// <param name="value">Value.</param>
        /// <param name="eventType">Event type.</param>
        /// <param name="number">Number.</param>
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

