namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Remote GPIO operator
    /// </summary>
    public class RemoteGpio : GpioBase
    {
        /// <summary>
        /// Default remote address
        /// </summary>
        private const string LocalAddress = "localhost";

        /// <summary>
        /// Default port
        /// </summary>
        private const int DefaultPort = 5555;
        /// <summary>
        /// The socket connected to the remote
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> class.
        /// </summary>
        public RemoteGpio() : this(PinNaming.BCM)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ereadian.RaspberryPi.Library.Hardware.RemoteGpio"/> class.
        /// </summary>
        /// <param name="pinNaming">Pin naming.</param>
        /// <param name="remoteAddress">Remote address.</param>
        /// <param name="port">Port.</param>
        public RemoteGpio(PinNaming pinNaming, string remoteAddress = null, int port = DefaultPort) : base(pinNaming)
        {
            this.socket = null;

            if (string.IsNullOrEmpty(remoteAddress))
            {
                remoteAddress = LocalAddress;
            }

            var addressList = Dns.GetHostAddresses(remoteAddress);
            if ((addressList == null) || (addressList.Length < 1))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Bad address: {0} to operation GPIO on remote",
                    remoteAddress);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            var endpoint = new IPEndPoint(addressList[0], port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);
            Trace.TraceInformation("Remote GPIO has been connected");
        }

        #region abstract class implementation
        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        public override void SetPinDirection(int pinNumber, GpioPinDirection direction)
        {
            this.SendPackage(
                "Failed to send package to remote when setting pin direction. Number of bytes get sent:{0}",
                (byte)Command.SetPinDirection,
                (byte)this.GetTargetPinNumber(pinNumber),
                (byte)direction);
        }

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        public override GpioPinValue this[int pinNumber]
        {
            get
            {
                this.SendPackage(
                    "Failed to send package to remote when reading pin value. Number of bytes get sent:{0}",
                    (byte)Command.GetPinValue,
                    (byte)this.GetTargetPinNumber(pinNumber));

                var data = this.ReadPackage(
                   "Failed to read package from remote when reading pin value. Number of bytes get received:{0}",
                   1);
                return (GpioPinValue)data[0];
            }
            set
            {
                this.SendPackage(
                    "Failed to send package to remote when setting pin value. Number of bytes get sent:{0}",
                    (byte)Command.SetPinValue,
                    (byte)this.GetTargetPinNumber(pinNumber),
                    (byte)value);
            }
        }

        /// <summary>
        /// Set button mode
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="mode">Button mode.</param>
        public override void SetButtonMode(int pinNumber, ButtonMode mode)
        {
            this.SendPackage(
                "Failed to send package to remote when setting button mode. Number of bytes get sent:{0}",
                (byte)Command.SetButtonMode,
                (byte)this.GetTargetPinNumber(pinNumber),
                (byte)mode);
        }
        #endregion abstract class implementation

        /// <summary>
        /// Disposes resources of current instance.
        /// </summary>
        protected override void DisposeInstance()
        {
            lock (this)
            {
                if (this.socket != null)
                {
                    try
                    {
                        var buffer = new byte[] { (byte)Command.End };
                        this.socket.Send(buffer);
                    }
                    catch
                    {
                    }
                    this.socket.Dispose();
                    this.socket = null;
                }
            }
        }

        /// <summary>
        /// Sends a package to network.
        /// </summary>
        /// <param name="errorMessageTemplate">Error message template.</param>
        /// <param name="package">Package.</param>
        private void SendPackage(string errorMessageTemplate, params byte[] package)
        {
            var n = this.socket.Send(package);
            if (n != package.Length)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    errorMessageTemplate,
                    n);
                Trace.TraceError(errorMessage);
                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// Reads a package from network
        /// </summary>
        /// <returns>The package.</returns>
        /// <param name="errorMessageTemplate">Error message template.</param>
        /// <param name="packageSize">Package size.</param>
        private byte[] ReadPackage(string errorMessageTemplate, int packageSize)
        {
            var package = new byte[packageSize];
            var n = this.socket.Receive(package);
            if (n != packageSize)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    errorMessageTemplate,
                    n);
                Trace.TraceError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            return package;
        }

        /// <summary>
        /// Command types
        /// </summary>
        private enum Command : byte
        {
            /// <summary>
            /// Finish the network communication
            /// </summary>
            End = 0,

            /// <summary>
            /// Set pin model
            /// </summary>
            SetPinDirection = 1,

            /// <summary>
            /// Set pin value
            /// </summary>
            SetPinValue = 2,

            /// <summary>
            /// Get pin value
            /// </summary>
            GetPinValue = 3,

            /// <summary>
            /// Set button mode
            /// </summary>
            SetButtonMode = 4,
        }
    }
}

