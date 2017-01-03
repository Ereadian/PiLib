namespace Ereadian.RaspberryPi.Library.Hardware.Joysticks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Joystick.
    /// </summary>
    /// <remarks>
    /// https://www.kernel.org/doc/Documentation/input/joystick-api.txt 
    /// </remarks>
    public class Joystick : IDisposable
    {
        /// <summary>
        /// Joystick system folder name
        /// </summary>
        private const string JoystickDeviceFolderName = "/dev/input";

        /// <summary>
        /// Regular express to match joystick device name
        /// </summary>
        private static readonly Regex JoyStickDeviceNameRegEx 
            = new Regex("^js[0-9]+", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Joystick event types
        /// </summary>
        private static IReadOnlyList<JoystickEventType> AllJoystickEventTypes;

        /// <summary>
        /// Joystick final full device name
        /// </summary>
        /// <remarks>
        /// For example: /dev/input/js0
        /// </remarks>
        private readonly string joystickDeviceFullName;

        /// <summary>
        /// Joystick device file stream
        /// </summary>
        /// <remarks>
        /// The stream created at first time event retrieving.
        /// </remarks>
        private FileStream joystickDeviceStream;

        /// <summary>
        /// Type constructor
        /// </summary>
        static Joystick()
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
        /// Instance constructor
        /// </summary>
        /// <param name="joystickDeviceName">Joystick device name.</param>
        public Joystick(string joystickDeviceName)
        {
            if (joystickDeviceName == null)
            {
                throw new ArgumentNullException("parameter \"joystickDeviceName\" should not be null");
            }

            if (!JoyStickDeviceNameRegEx.IsMatch(joystickDeviceName))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture, 
                    "Invalid joystick device name: \"{0}\"",
                    joystickDeviceName);
                throw new ArgumentException(errorMessage);
            }

            joystickDeviceFullName = Path.Combine(JoystickDeviceFolderName, joystickDeviceName);
            this.joystickDeviceStream = null;
        }

        /// <summary>
        /// Get joystick event
        /// </summary>
        /// <returns>The event.</returns>
        public JoystickEvent GetEvent()
        {
            byte[] eventData = new byte[8];
            var dataRead = this.DeviceStream.Read(eventData, 0, eventData.Length);
            if (dataRead == 0)
            {
                return null;
            }

            if (dataRead != eventData.Length)
            {
                throw new ApplicationException("Failed to read joystick data");
            }

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

            var joystickEvent = new JoystickEvent(
                timestamp,
                value,
                eventType,
                number
            );

            return joystickEvent;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.Joystick"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.Joystick"/>. The <see cref="Dispose"/> method
        /// leaves the <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.Joystick"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.Joystick"/> so the garbage collector can reclaim
        /// the memory that the <see cref="Ereadian.RaspberryPi.Library.Hardware.Joysticks.Joystick"/> was occupying.</remarks>
        public void Dispose()
        {
            this.DisposeResources();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        protected virtual void DisposeResources()
        {
            lock (this)
            {
                if (this.joystickDeviceStream != null)
                {
                    this.joystickDeviceStream.Dispose();
                    this.joystickDeviceStream = null;
                }
            }
        }

        /// <summary>
        /// Get joystream device stream
        /// </summary>
        /// <value>The device stream.</value>
        protected FileStream DeviceStream
        {
            get
            {
                FileStream stream = this.joystickDeviceStream;
                if (stream == null)
                {
                    lock (this)
                    {
                        stream = this.joystickDeviceStream;
                        if (stream == null)
                        {
                            stream = new FileStream(this.joystickDeviceFullName, FileMode.Open, FileAccess.Read);
                            this.joystickDeviceStream = stream;
                        }
                    }
                }

                return stream;
            }
        }

        /// <summary>
        /// Get all joystick device names
        /// </summary>
        /// <returns>The joystick device name collection.</returns>
        public static IReadOnlyList<string> GetJoystickDeviceNames()
        {
            var directoryInformation = new DirectoryInfo(JoystickDeviceFolderName);
            var deviceFileInformationList = directoryInformation.GetFiles();
            var joyStickNames = new List<string>(deviceFileInformationList.Length);
            foreach (var deviceFileInformation in deviceFileInformationList)
            {
                var deviceName = deviceFileInformation.Name;
                if (JoyStickDeviceNameRegEx.IsMatch(deviceName))
                {
                    joyStickNames.Add(deviceName);
                }
            }

            joyStickNames.Sort();
            return joyStickNames;
        }
    }
}

