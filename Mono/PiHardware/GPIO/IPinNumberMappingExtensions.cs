namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Pin number mapping extensions
    /// </summary>
    public static class IPinNumberMappingExtensions
    {
        /// <summary>
        /// Gets the target pin number.
        /// </summary>
        /// <returns>The target pin number.</returns>
        /// <param name="sourcePinNaming">Source pin naming system</param>
        /// <param name="targetPinNaming">Target pin naming system</param>
        /// <param name="pinNumber">Current pin number.</param>
        public static int MapPinNumber(
            this IPinNumberMapping mapping, 
            PinNaming sourcePinNaming, 
            PinNaming targetPinNaming, 
            int pinNumber)
        {
            if (mapping == null)
            {
                throw new ArgumentException("mapping");
            }

            int targetPinNumber;
            if (!mapping.TryGetPinNumber(sourcePinNaming, targetPinNaming, pinNumber, out targetPinNumber))
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot convert pin number {0} from {1} to {2}",
                    pinNumber,
                    sourcePinNaming,
                    targetPinNaming);
                Trace.TraceError(errorMessage);
                throw new ArgumentException(errorMessage);
            }

            return targetPinNumber;
        }
    }
}

