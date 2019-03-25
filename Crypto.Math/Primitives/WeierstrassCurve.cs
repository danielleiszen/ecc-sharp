using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Primitives
{
    internal class WeierstrassCurve : EllypticCurve
    {
        public WeierstrassCurve(BigDecimal aCoefficient, BigDecimal bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        public WeierstrassCurve(double aCoefficient, double bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        internal override BigDecimal CalculateAdditionXCoordinate(BigDecimal beta, Point p1, Point p2)
        {
            return BigMath.Subtract(
                BigMath.Pow(beta, 2, Context), 
                BigMath.Add(p1.XCoordinate, p2.XCoordinate, Context), Context);
        }

        internal override BigDecimal CalculateBeta(Point p)
        {
            return BigMath.Divide(
                BigMath.Add(
                    BigMath.Multiply(
                        BigDecimal.Parse("3", Context), 
                        BigMath.Pow(p.XCoordinate, 2, Context)), 
                    Coefficient),
                BigMath.Multiply(
                    BigDecimal.Parse("2", Context),
                    p.YCoordinate), Context);
        }
    }
}
