using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Primitives
{
    internal class KublitzCurve : EllypticCurve
    {
        public KublitzCurve(BigDecimal aCoefficient, BigDecimal bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        public KublitzCurve(double aCoefficient, double bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        internal override BigDecimal CalculateBeta(Point p)
        {
            return BigMath.Divide(
                BigMath.Subtract(
                    BigMath.Add(
                        BigMath.Multiply(
                            BigDecimal.Parse("3"), 
                            BigMath.Pow(p.XCoordinate, 2, Context), Context),
                        BigMath.Multiply(
                            BigMath.Multiply(
                                BigDecimal.Parse("2"), 
                                Coefficient, Context),
                            p.XCoordinate, Context), Context), 
                    p.YCoordinate, Context), 
                BigMath.Add(
                    BigMath.Multiply(
                        BigDecimal.Parse("2"), 
                        p.YCoordinate), 
                    p.XCoordinate, Context), Context);
        }

        internal override BigDecimal CalculateAdditionXCoordinate(BigDecimal beta, Point p1, Point p2)
        {
            return
                BigMath.Subtract(
                    BigMath.Add(
                        BigMath.Pow(beta, 2, Context),
                        beta, Context)
                    ,
                    BigMath.Add(
                        p1.XCoordinate, 
                        p2.XCoordinate, Context),
                    Context);
        }
    }
}
