namespace Ereadian.RaspberryPi.Library.Hardware.Joysticks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Joystick.
    /// </summary>
    /// <remarks>
    /// https://www.kernel.org/doc/Documentation/input/joystick-api.txt 
    /// </remarks>
    public class Joystick : IJoystick
    {
        /// <summary>
        /// Joystick system folder name
        /// </summary>
        private const string JoystickDeviceFolderName = "/dev/input";

        /// <summary>
        /// The size of the event buffer.
        /// </summary>
        private static readonly int EventBufferSize = sizeof(uint) + sizeof(short) + 2 * sizeof(byte);

        /// <summary>
        /// Regular express to match joystick device name
        /// </summary>
        private static readonly Regex JoyStickDeviceNameRegEx 
            = new Regex("^js[0-9]+", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The full name of the joystick device.
        /// </summary>
        private readonly string joystickDeviceFullName;

        /// <summary>
        /// Joystick device file stream
        /// </summary>
        /// <remarks>
        /// The stream created at first time event retrieving.
        /// </remarks>
        private FileStream joystickDeviceStream;

        /// <summary>
        /// event queue or unblock event read operation
        /// </summary>
        private ConcurrentQueue<JoystickEvent> eventQueue;

        /// <summary>
        /// Event read worker thread
        /// </summary>
        private Thread dataReaderThread;

        /// <summary>
        /// shudown event to stop event reader worker thread
        /// </summary>
        private ManualResetEventSlim shutdownEvent;

        /// <summary>
        /// Instance constructor
        /// </summary>
        /// <param name="joystickDeviceName">Joystick device name.</param>
        public Joystick(string joystickDeviceName, bool blockMode = false)
        {
            if (string.IsNullOrWhiteSpace(joystickDeviceName))
            {
                throw new ArgumentNullException("parameter \"joystickDeviceName\" should not be null or blank");
            }

            joystickDeviceName = joystickDeviceName.Trim();
            if (!JoyStickDeviceNameRegEx.IsMatch(joystickDeviceName))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture, 
                    "Invalid joystick device name: \"{0}\"",
                    joystickDeviceName);
                throw new ArgumentException(errorMessage);
            }

            this.joystickDeviceFullName = Path.Combine(JoystickDeviceFolderName, joystickDeviceName);
            this.joystickDeviceStream = new FileStream(this.joystickDeviceFullName, FileMode.Open, FileAccess.Read);

            if (blockMode)
            {
                this.eventQueue = null;
                this.shutdownEvent = null;
                this.dataReaderThread = null;
            }
            else
            {
                this.eventQueue = new ConcurrentQueue<JoystickEvent>(); 
                this.shutdownEvent = new ManualResetEventSlim(false);
                this.dataReaderThread = new Thread(this.DataReaderWorker);
                this.dataReaderThread.Start();
            }
        }

        /// <summary>
        /// Get joystick event
        /// </summary>
        /// <returns>The event.</returns>
        public JoystickEvent GetEvent()
        {
            JoystickEvent joystickEvent;

            if (this.shutdownEvent != null)
            {
                if (!this.eventQueue.TryDequeue(out joystickEvent))
                {
                    joystickEvent = null;
                }
            }
            else
            {
                byte[] eventData = new byte[8];
                var dataRead = this.joystickDeviceStream.Read(eventData, 0, eventData.Length);
                if (dataRead == 0)
                {
                    joystickEvent = null;

                }
                else
                {
                    if (dataRead != eventData.Length)
                    {
                        throw new ApplicationException("Failed to read joystick data");
                    }
                
                    joystickEvent = new JoystickEvent(eventData);
                }
            }

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
                if (this.shutdownEvent != null)
                {
                    this.shutdownEvent.Set();
                    this.dataReaderThread.Join();
                    this.shutdownEvent.Dispose();
                    this.dataReaderThread = null;
                    this.shutdownEvent = null;
                }

                if (this.joystickDeviceStream != null)
                {
                    this.joystickDeviceStream.Dispose();
                    this.joystickDeviceStream = null;
                }
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

        /// <summary>
        /// Data Reader worker thread
        /// </summary>
        private void DataReaderWorker()
        {
            var waitHandles = new WaitHandle[]{ this.shutdownEvent.WaitHandle, null };
            var buffer = new byte[EventBufferSize];
            IAsyncResult asyncResult = null;

            while (true)
            {
                try
                {
                    asyncResult = this.joystickDeviceStream.BeginRead(
                        buffer,
                        0,
                        EventBufferSize,
                        null,
                        null);
                    waitHandles[1] = asyncResult.AsyncWaitHandle;
                    if (WaitHandle.WaitAny(waitHandles) == 0)
                    {
                        break;
                    }

                    var dataRead = this.joystickDeviceStream.EndRead(asyncResult);
                    if (dataRead != EventBufferSize)
                    {
                        var errorMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Expected to read {0} bytes from joystick device \"{1}\". However, only {2} bytes returned",
                            EventBufferSize,
                            this.joystickDeviceFullName,
                            dataRead);
                        throw new IOException(errorMessage);
                    }

                    var joystickEvent = new JoystickEvent(buffer);
                    this.eventQueue.Enqueue(joystickEvent);
                }
                catch(Exception exception)
                {
                    Trace.TraceError("Joystick.DataReaderWorker: "+ exception.ToString());
                    break;
                }
            }
        }
    }
}
