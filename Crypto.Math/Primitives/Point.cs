using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Crypto.Primitives
{
    public class Point
    {
        public Point(BigInteger x, BigInteger y)
        {
            XCoordinate = x;
            YCoordinate = y;
        }

        public Point(int x, int y)
        {
            XCoordinate = new BigInteger(x);
            YCoordinate = new BigInteger(y);
        }

        public BigInteger XCoordinate { get; private set; }
        public BigInteger YCoordinate { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                XCoordinate.Equals(point.XCoordinate) &&
                YCoordinate.Equals(point.YCoordinate);
        }

        public override int GetHashCode()
        {
            var hashCode = -1219734581;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(XCoordinate);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(YCoordinate);
            return hashCode;
        }

        public override string ToString()
        {
            return $"(x: {XCoordinate:###0}; y: {YCoordinate:###0})";
        }
    }
}
