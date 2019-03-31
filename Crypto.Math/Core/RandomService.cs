using System;
using System.Numerics;

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
            byte[] data = new byte[(int)Math.Ceiling((double)bits / 8D)];
            Random.NextBytes(data);
            return BigInteger.Abs(new BigInteger(data));
        }
    }
}
