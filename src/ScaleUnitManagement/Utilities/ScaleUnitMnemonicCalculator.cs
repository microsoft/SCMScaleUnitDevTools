// ------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------------------------

using System;
using System.Linq;

namespace ScaleUnitManagement.Utilities
{
    public static class ScaleUnitMnemonicCalculator
    {
        public static string ToMnemonic(int integralValue)
        {
            if (integralValue < 0)
            {
                throw new Exception($"Unable to convert integral value of {integralValue} to a scale unit mnemonic as it is negative.");
            }

            if (integralValue == 0)
            {
                return "@@";
            }

            string mnemonicString = "";
            const char BaseChar = '@';
            const int NumberBase = 27;
            while (integralValue > 0)
            {
                int remainder = integralValue % NumberBase;
                integralValue /= NumberBase;
                mnemonicString += (char)(BaseChar + remainder);
            }
            string result = new string(mnemonicString.Reverse().ToArray());
            if (result.Count() == 1)
            {
                // Prefix with additive identity.
                result = BaseChar + result;
            }
            return result;
        }

        public static int ToIntegralValue(string mnemonic)
        {
            mnemonic = mnemonic.ToUpperInvariant();

            // Use '@' as base so that 'A' isn't logically zero, making 'AAA' is different from 'AA', e.g.
            const char BaseChar = '@';
            int integralValue = 0;
            for (int i = 0; i < mnemonic.Length; i++)
            {
                int value = mnemonic[i] - BaseChar;
                if (value < 0 || value >= 27)
                {
                    throw new Exception($"Scale set mnemonic has the invalid character '{mnemonic[i]}'. Valid character values are '@' and the alpha characters A through Z.");
                }

                int position = mnemonic.Length - 1 - i;
                integralValue += value * (int)Math.Pow(27, position);
            }

            return integralValue;
        }
    }
}
