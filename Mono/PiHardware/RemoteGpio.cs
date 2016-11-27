namespace Ereadian.RaspberryPi.Library.Hardware
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;

    public class RemoteGpio : IGpio
    {
        private const string LocalAddress = "localhost";
        private const int DefaultPort = 5555;

        private PinNaming currentPinNaming;
        private Socket socket;

        public RemoteGpio() : this(PinNaming.Physical)
        {
        }

        public RemoteGpio(PinNaming pinNaming, string remoteAddress = null, int port = DefaultPort)
        {
            this.currentPinNaming = pinNaming;
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

        #region interface implementation
        /// <summary>
        /// Gets the current pin naming.
        /// </summary>
        /// <value>The current pin naming.</value>
        public PinNaming CurrentPinNaming
        {
            get
            {
                return this.currentPinNaming;
            }
        }

        /// <summary>
        /// Sets the pin direction.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="direction">Pin Direction.</param>
        public void SetPinDirection(int pinNumber, GpioPinDirection direction)
        {
            this.SendPackage(
                "Failed to send package to remote when setting pin direction. Number of bytes get sent:{0}",
                (byte)Command.SetModel,
                (byte)this.GetTargetPinNumber(pinNumber),
                (byte)direction);
        }

        /// <summary>
        /// Gets or sets the <see cref="Ereadian.RaspberryPi.Library.Hardware.IGpio"/> value for the specified pin.
        /// </summary>
        /// <param name="pinNumber">Pin number.</param>
        public GpioPinValue this[int pinNumber]
        {
            get
            {
                this.SendPackage(
                    "Failed to send package to remote when reading pin value. Number of bytes get sent:{0}",
                    (byte)Command.GetValue,
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
                    (byte)Command.SetValue,
                    (byte)this.GetTargetPinNumber(pinNumber),
                    (byte)value);
            }
        }

        public void Dispose()
        {
            this.DisposeInstance();
        }
        #endregion interface implementation

        protected virtual void DisposeInstance()
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

        protected virtual int GetTargetPinNumber(int currentPinNumber, PinNaming targetPinName = PinNaming.WiringPi)
        {
            var mapping = Singleton<PinNumberMapping>.Instance;
            int targetPinNumber;
            if (!mapping.TryGetPinNumber(this.CurrentPinNaming, targetPinName, currentPinNumber, out targetPinNumber))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot convert pin number {0} from {1} to {2}",
                    currentPinNumber,
                    this.CurrentPinNaming,
                    targetPinName);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            return targetPinNumber;
        }

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

        private enum Command : byte
        {
            End = 0,
            SetModel = 1,
            SetValue = 2,
            GetValue = 3
        }
    }
}

