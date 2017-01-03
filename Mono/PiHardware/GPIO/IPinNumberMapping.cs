using System;

namespace Ereadian.RaspberryPi.Library.Hardware.GPIO
{
    /// <summary>
    /// Pin number mapping.
    /// </summary>
    public interface IPinNumberMapping
    {
        /// <summary>
        /// Try to map pin number from one naming system to another.
        /// </summary>
        /// <returns><c>true</c>, if get pin number was mapped successfully, <c>false</c> pin number is not supported.</returns>
        /// <param name="sourceNameing">Source nameing.</param>
        /// <param name="targetNaming">Target naming.</param>
        /// <param name="sourcePinNumber">Source pin number.</param>
        /// <param name="targetPinNumber">Target pin number.</param>
        bool TryGetPinNumber(
            PinNaming sourceNameing, 
            PinNaming targetNaming, 
            int sourcePinNumber, 
            out int targetPinNumber);
    }
}

