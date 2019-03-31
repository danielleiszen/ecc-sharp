using System.Linq;
using System.Text;

namespace System.Numerics
{
    /// <summary>
    /// Extension methods to convert <see cref="System.Numerics.BigInteger"/>
    /// instances to hexadecimal, octal, and binary strings.
    /// </summary>
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a binary
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToBinaryString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        /// <summary>
        /// Find the great common divisor for a and b
        /// </summary>
        public static BigInteger FindGCD(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            // Base Case 
            if (a == 0)
            {
                x = BigInteger.Zero;
                y = BigInteger.One;

                return b;
            }

            // To store results of recursive call 
            var gcd = FindGCD(BigInteger.Remainder(b, a), a, out BigInteger x1, out BigInteger y1);

            // Update x and y using results of recursive 
            // call 
            x = BigInteger.Subtract(y1, BigInteger.Multiply(BigInteger.Divide(b, a), x1));
            y = new BigInteger(x1.ToByteArray());

            return gcd;
        }

        public static BigInteger? FindModularInverse(this BigInteger a, BigInteger m)
        {
            if(BigInteger.Compare(a, BigInteger.Zero) == 0)
            {
                return BigInteger.Zero;
            }

            var g = FindGCD(a, m, out BigInteger x, out BigInteger y);

            if (g.CompareTo(BigInteger.One) != 0)
            {
                return default;
            }
            else
            {
                // m is added to handle negative x 
                return x.ToPositiveMod(m);
            }
        }

        public static BigInteger ToPositiveMod(this BigInteger a, BigInteger m)
        {
            return BigInteger.Remainder(BigInteger.Add(BigInteger.Remainder(a, m), m), m);
        }

        public static BigInteger SquareRoot(this BigInteger x)
        {
            int b = 15; // this is the next bit we try 
            var r = new BigInteger(BigInteger.Zero.ToByteArray()); // r will contain the result
            var r2 = new BigInteger(r.ToByteArray()); // here we maintain r squared

            while (b >= 0)
            {
                var sr2 = r2;
                var sr = r;
                // compute (r+(1<<b))**2, we have r**2 already.
                r2 += (uint)((r << (1 + b)) + (1 << (b + b)));
                r += (uint)(1 << b);
                if (r2 > x)
                {
                    r = sr;
                    r2 = sr2;
                }
                b--;
            }

            return r;
        }

        public static BigInteger SqareRoot2(this BigInteger n)
        {
            BigInteger a = BigInteger.One;
            BigInteger b = BigInteger.Add(n >> 5, new BigInteger(8));

            while (BigInteger.Compare(b, a) >= 0)
            {
                var mid = BigInteger.Add(a, b) >> 1;
                var midp2 = BigInteger.Multiply(mid, mid);

                if (BigInteger.Compare(midp2, n) > 0)
                {
                    b = BigInteger.Subtract(mid, BigInteger.One);
                }
                else
                {
                    a = BigInteger.Add(mid, BigInteger.One);
                }
            }

            return BigInteger.Subtract(a, BigInteger.One);
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static BigInteger FromBigEndianByteArray(byte[] bytes)
        {
            var array = bytes.Reverse().ToArray(); // little endian to big endian
            var bitlen = array.Length * 8; // shift bits when negative

            var data = new BigInteger(array);

            if (BigInteger.Compare(data, BigInteger.Zero) < 0)
            {
                data = BigInteger.Add(data, (BigInteger.One << bitlen));
            }

            return data;
        }

        public static BigInteger FromBigEndianHexString(string hex)
        {
            var bytes = HexStringToByteArray(hex);

            return FromBigEndianByteArray(bytes);
        }

        public static BigInteger ComputeSHA1(string message)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(message));

            return FromBigEndianByteArray(bytes);
        }
    }
}