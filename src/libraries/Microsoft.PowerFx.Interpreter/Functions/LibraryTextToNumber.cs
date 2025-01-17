﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Globalization;

namespace Microsoft.PowerFx.Functions
{
    internal static partial class Library
    {
        private enum ConvertionStatus
        {
            Ok,
            InvalidNumber
        }

        // Converts numbers that can be signed and be percentages
        private static (double, ConvertionStatus) ConvertToNumber(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            var startingPercent = str[0] == '%';
            var endingPercent = str[str.Length - 1] == '%';

            if (startingPercent || endingPercent)
            {
                if (startingPercent && endingPercent)
                {
                    return (0d, ConvertionStatus.InvalidNumber);
                }

                var pct = ConvertPercentageToNumber(startingPercent ? str.Substring(1) : str.Substring(0, str.Length - 1), culture);
                return (pct.Item1 / 100d, pct.Item2);
            }

            return ConvertPossiblySignedNumberNoPercentage(str, culture);
        }

        // Converts numbers that are already identified as percentages
        private static (double, ConvertionStatus) ConvertPercentageToNumber(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '%' || str[str.Length - 1] == '%')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '+')
            {
                return ConvertSignedNumberNoPercentage(str.Substring(1), culture);
            }

            if (str[0] == '-')
            {
                var sn = ConvertSignedNumberNoPercentage(str.Substring(1), culture);
                return (-sn.Item1, sn.Item2);
            }

            return ConvertPossiblySignedNumberNoPercentage(str, culture);
        }

        private static (double, ConvertionStatus) ConvertPossiblySignedNumberNoPercentage(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '+')
            {
                return ConvertSignedNumberPossiblyPercent(str.Substring(1), culture);
            }

            if (str[0] == '-')
            {
                var sn = ConvertSignedNumberPossiblyPercent(str.Substring(1), culture);
                return (-sn.Item1, sn.Item2);
            }

            return ConvertUnsignedNoPercentNumber(str, culture);
        }

        private static (double, ConvertionStatus) ConvertSignedNumberPossiblyPercent(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '+' || str[0] == '-')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            var startingPercent = str[0] == '%';
            var endingPercent = str[str.Length - 1] == '%';

            if (startingPercent || endingPercent)
            {
                if (startingPercent && endingPercent)
                {
                    return (0d, ConvertionStatus.InvalidNumber);
                }

                var pct = ConvertUnsignedNoPercentNumber(startingPercent ? str.Substring(1) : str.Substring(0, str.Length - 1), culture);
                return (pct.Item1 / 100d, pct.Item2);
            }

            return ConvertUnsignedNoPercentNumber(str, culture);
        }

        private static (double, ConvertionStatus) ConvertSignedNumberNoPercentage(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '+' || str[0] == '-')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '%' || str[str.Length - 1] == '%')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            return ConvertUnsignedNoPercentNumber(str, culture);
        }

        private static (double, ConvertionStatus) ConvertUnsignedNoPercentNumber(string str, CultureInfo culture)
        {
            str = str.Trim();

            if (string.IsNullOrEmpty(str))
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '+' || str[0] == '-')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            if (str[0] == '%' || str[str.Length - 1] == '%')
            {
                return (0d, ConvertionStatus.InvalidNumber);
            }

            var err = double.TryParse(str, NumberStyles.Any, culture, out var val);

            return !err ? (0d, ConvertionStatus.InvalidNumber) :
                   IsInvalidDouble(val) ? (0d, ConvertionStatus.InvalidNumber) :
                   (val, ConvertionStatus.Ok);
        }
    }
}
