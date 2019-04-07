using System.Collections.Generic;
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

        public static bool EulersCriterion(this BigInteger a, BigInteger modulo)
        {
            var ex = (modulo - 1) / 2;
            var res = BigInteger.ModPow(a, ex, modulo);
            //var pres = res.ToPositiveMod(modulo);

            if(res == 1) // || pres == 1)
            {
                return true;
            }
            else if(res == -1 || res == modulo - 1)
            {
                return false;
            }
            else
            {
                throw new InvalidProgramException("The Euler's Criterion calculation is wrong");
            }
        }

        public static Tuple<BigInteger, BigInteger> FindModularSquareRoots(this BigInteger q, BigInteger modulo)
        {
            if (q.EulersCriterion(modulo))
            {
                // Step 1: Find a, omega2
                BigInteger a = -1;
                BigInteger omega2;

                do
                {
                    a += 1;
                    omega2 = (a * a + modulo - q) % modulo;
                } while (omega2.EulersCriterion(modulo));

                Tuple<BigInteger, BigInteger> mul(Tuple<BigInteger, BigInteger> aa, Tuple<BigInteger, BigInteger> bb)
                {
                    return new Tuple<BigInteger, BigInteger>(
                        (aa.Item1 * bb.Item1 + aa.Item2 * bb.Item2 * omega2) % modulo,
                        (aa.Item1 * bb.Item2 + bb.Item1 * aa.Item2) % modulo
                    );
                }

                // Step 2: Compute power
                Tuple<BigInteger, BigInteger> r = new Tuple<BigInteger, BigInteger>(1, 0);
                Tuple<BigInteger, BigInteger> s = new Tuple<BigInteger, BigInteger>(a, 1);
                BigInteger nn = ((modulo + 1) >> 1) % modulo;

                while (nn > 0)
                {
                    if ((nn & 1) == 1)
                    {
                        r = mul(r, s);
                    }
                    s = mul(s, s);
                    nn >>= 1;
                }

                // Step 3: Check x in Fp
                if (r.Item2 != 0)
                {
                    return new Tuple<BigInteger, BigInteger>(0, 0);
                }

                // Step 5: Check x * x = n
                if (r.Item1 * r.Item1 % modulo != q)
                {
                    return new Tuple<BigInteger, BigInteger>(0, 0);
                }

                // Step 4: Solutions
                return new Tuple<BigInteger, BigInteger>(r.Item1, modulo - r.Item1);
            }

            // there is no root
            return new Tuple<BigInteger, BigInteger>(0, 0);
        }

        public static BigInteger ToPositiveMod(this BigInteger a, BigInteger m)
        {
            return BigInteger.Remainder(BigInteger.Add(BigInteger.Remainder(a, m), m), m);
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

        public static BigInteger SqRtN(this BigInteger N)
        {
            /*++
             *  Using Newton Raphson method we calculate the
             *  square root (N/g + g)/2
             */
            BigInteger rootN = N;
            int bitLength = 1; // There is a bug in finding bit length hence we start with 1 not 0
            while (rootN / 2 != 0)
            {
                rootN /= 2;
                bitLength++;
            }
            bitLength = (bitLength + 1) / 2;
            rootN = N >> bitLength;

            BigInteger lastRoot = BigInteger.Zero;
            do
            {
                lastRoot = rootN;
                rootN = (BigInteger.Divide(N, rootN) + rootN) >> 1;
            }
            while (!((rootN ^ lastRoot).ToString() == "0"));
            return rootN;
        } // SqRtN

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