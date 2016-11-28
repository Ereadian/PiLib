namespace Ereadian.RaspberryPi.Library.Hardware
{
	using System;
	using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

	/// <summary>
	/// Names Pin number to physical pin number mapping.
	/// </summary>
    public class PinNumberMapping : IPinNumberMapping
	{
        /// <summary>
        /// The invalid pin number.
        /// </summary>
        private const int InvalidPinNumber = -1;

        /// <summary>
        /// The raw pins relationships
        /// </summary>
        private static readonly int[,] Pins = new int[,]
        {
            { 3, 5, 7,  8, 10, 11, 12, 13, 15, 16, 18, 19, 21, 22, 23, 24, 26, 27, 28, 29, 31, 32, 33, 35, 36, 37, 38, 40 }, // Physical pin numbers
            { 8, 9, 7, 15, 16,  0,  1,  2,  3,  4,  5, 12, 13,  6, 14, 10, 11, 30, 31, 21, 22, 26, 23, 24, 27, 25, 28, 29 }, // WiringPi pin numbers
            { 2, 3, 4, 14, 15, 17, 18, 27, 12, 23, 24, 10,  9, 25, 11,  8,  7,  0,  1,  5,  6, 12, 13, 19, 16, 26, 20, 21 }, // BCM pin numbers
        };

        private readonly IReadOnlyList<int>[,] mappings;

        /// <summary>
        /// Initializes the <see cref="Ereadian.RaspberryPi.Library.Hardware.PinNumberMapping"/> class.
        /// </summary>
        public PinNumberMapping()
        {
            var namingTypes = Enum.GetValues(typeof(PinNaming)) as int[];
            var namingTypeCount = namingTypes.Length;
            var mappingTableRows = Pins.GetLength(0);
            if (namingTypeCount != mappingTableRows)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "[Error] Naming type count does not match. Type of {0} has {1} items but pin mapping table has {2}",
                    typeof(PinNaming).FullName,
                    namingTypeCount,
                    mappingTableRows);
                Trace.TraceError(errorMessage);
                throw new ApplicationException(errorMessage);
            }

            mappings = new IReadOnlyList<int>[mappingTableRows, mappingTableRows];
            for (int sourceType = 0; sourceType < mappingTableRows; sourceType++)
            {
                for (int targetType = 0; targetType < mappingTableRows; targetType++)
                {
                    if (sourceType != targetType)
                    {
                        mappings[sourceType, targetType] = CreateMapping(Pins, sourceType, targetType);
                    }
                }
            }
        }
            
        /// <summary>
        /// Try to map pin number from one naming system to another.
        /// </summary>
        /// <returns><c>true</c>, if get pin number was mapped successfully, <c>false</c> pin number is not supported.</returns>
        /// <param name="sourceNameing">Source nameing.</param>
        /// <param name="targetNaming">Target naming.</param>
        /// <param name="sourcePinNumber">Source pin number.</param>
        /// <param name="targetPinNumber">Target pin number.</param>
        public bool TryGetPinNumber(
            PinNaming sourceNameing, 
            PinNaming targetNaming, 
            int sourcePinNumber, 
            out int targetPinNumber)
        {
            if (sourceNameing == targetNaming)
            {
                targetPinNumber = sourcePinNumber;
                return true;
            }

            var sourceType = (int)sourceNameing;
            var targetType = (int)targetNaming;
            var mapping = mappings[sourceType, targetType];
            targetPinNumber = InvalidPinNumber;
            if ((sourcePinNumber >= 0) && (sourcePinNumber < mapping.Count))
            {
                targetPinNumber = mapping[sourcePinNumber];
            }

            return targetPinNumber != InvalidPinNumber;
        }

        /// <summary>
        /// Creates the pin mapping.
        /// </summary>
        /// <returns>The mapping</returns>
        /// <param name="pins">Pins the whole pin relationships</param>
        /// <param name="source">Source row id</param>
        /// <param name="target">Target row id</param>
        private static IReadOnlyList<int> CreateMapping(int[,] pins, int source, int target)
        {
            var numbers = pins.GetLength(1);
            var maxPinId = 0;
            for (var i = 0; i < numbers; i++)
            {
                var sourcePin = pins[source, i];
                if (maxPinId < sourcePin)
                {
                    maxPinId = sourcePin;
                }
            }

            var list = new int[maxPinId + 1];
            for (var i = 0; i < list.Length; i++)
            {
                list[i] = -1;
            }

            for (var i = 0; i < numbers; i++)
            {
                var sourcePin = pins[source, i];
                var targetPin = pins[target, i];
                list[sourcePin] = targetPin;
            }

            return list;
        }
	}
}

