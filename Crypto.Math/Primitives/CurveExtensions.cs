using System;
using System.Numerics;

namespace Crypto.Primitives
{
    public static class CurveExtensions
    {
        public static Point CalculateAddition(this EllypticCurve curve, Point p1, Point p2)
        {
            var beta = default(BigInteger);

            if (p1 != null && p1.Equals(p2))
            {
                beta = curve.CalculateBeta(p1);
            }
            else if(p1 != null && p2 != null)
            {
                beta = curve.CalculateBeta(p1, p2);
            }
            else
            {
                throw new ArgumentException();
            }

            var x = curve.CalculateAdditionXCoordinate(beta, p1, p2);
            var y = curve.CalculateAdditionYCoordinate(beta, p1, x);
            
            return new Point(x.ToPositiveMod(curve.Modulo), y.ToPositiveMod(curve.Modulo));
        }

        public static Point CalculateDoubling(this EllypticCurve curve, Point p)
        {
            return curve.CalculateAddition(p, p);
        }

        public static Point CalculateMultiplication(this EllypticCurve curve, Point g, BigInteger scalar)
        {
            var binary = scalar.ToBinaryString();
            binary = binary.TrimStart('0');

            var p = new Point(g.XCoordinate, g.YCoordinate);

            for(var i = 1; i < binary.Length; i++)
            {
                bool add = binary[i] == '1';

                p = curve.CalculateDoubling(p);

                if (add)
                {
                    p = curve.CalculateAddition(p, g);
                }
            }

            return p;
        }

        public static BigInteger ToSecret(this CurvePoint point)
        {
            return BigInteger.Remainder(BigInteger.Multiply(point.Point.XCoordinate, point.Point.YCoordinate), point.Curve.Modulo);
        }
    }
}
