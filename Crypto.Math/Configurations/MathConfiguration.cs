using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Crypto.Configurations
{
    public class MathConfiguration
    {
        public const int DEFAULT_KEY_BITLENGTH = 200;
        public const int DEFAULT_MODULO = 23;

        public static int KeyBitLength { get; set; }
        public static BigInteger Modulo { get; set; }

        static MathConfiguration()
        {
            KeyBitLength = DEFAULT_KEY_BITLENGTH;
            Modulo = new BigInteger(DEFAULT_MODULO);
        }

        public static BigInteger GenerateSECP256K1()
        {
            var b = new BigInteger(2);

            return 
                BigInteger.Subtract(
                    BigInteger.Subtract(
                        BigInteger.Subtract(
                            BigInteger.Subtract(
                                BigInteger.Subtract(
                                    BigInteger.Subtract(
                                        BigInteger.Subtract(
                                            BigInteger.Pow(b, 256),
                                            BigInteger.Pow(b, 32)),
                                        BigInteger.Pow(b, 9)),
                                    BigInteger.Pow(b, 8)),
                                BigInteger.Pow(b, 7)),
                            BigInteger.Pow(b, 6)),
                        BigInteger.Pow(b, 4)),
                    BigInteger.Pow(b, 0));
        }
    }
}
