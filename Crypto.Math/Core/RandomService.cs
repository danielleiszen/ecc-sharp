using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Core
{
    public class RandomService
    {
        private static Random Random { get; set; }
        static RandomService()
        {
            Random = new Random(DateTime.Now.Millisecond);
        }

        public static BigInteger Generate(int bits = Configurations.MathConfiguration.DEFAULT_KEY_BITLENGTH)
        {
            return new BigInteger(bits, Random);
        }
    }
}
