using System;
using System.Collections.Generic;
using System.Text;
using Deveel.Math;

namespace Crypto.Configurations
{
    public class MathConfiguration
    {
        public const int DEFAULT_KEY_BITLENGTH = 200;
        public const int DEFAULT_PRECISION = 200;
        public const int DEFAULT_SCALE = 50;

        public static int Precision { get; set; }
        public static int KeyBitLength { get; set; }
        public static int Scale { get; set; }

        static MathConfiguration()
        {
            Scale = DEFAULT_SCALE;
            Precision = DEFAULT_PRECISION;
            KeyBitLength = DEFAULT_KEY_BITLENGTH;
        }

        public static MathContext CreateContext()
        {
            return new MathContext(Precision, RoundingMode.HalfUp);
        }
    }
}
