using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Primitives
{
    public class CurvePoint
    {
        public EllypticCurve Curve { get; private set; }
        public Point Point { get; private set; }

        public CurvePoint(EllypticCurve curve, Point point)
        {
            Curve = curve;
            Point = point;
        }

        public CurvePoint(CurvePoint generation, BigInteger multiplier)
        {
            Curve = generation.Curve;
            Point = Curve.CalculateMultiplication(generation.Point, multiplier);
        }

        public override string ToString()
        {
            return $"{Point}";
        }
    }
}
