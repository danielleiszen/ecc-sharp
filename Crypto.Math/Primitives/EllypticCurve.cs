using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Primitives
{
    public abstract class EllypticCurve
    {
        public BigDecimal Coefficient { get; private set; }
        public BigDecimal Constant { get; private set; }
        protected MathContext Context { get; private set; }

        public EllypticCurve(BigDecimal aCoefficient, BigDecimal bConstant)
        {
            Context = Configurations.MathConfiguration.CreateContext();
            Coefficient = aCoefficient;
            Constant = bConstant;
        }

        public EllypticCurve(double aCoefficient, double bConstant)
            : this(new BigDecimal(aCoefficient), new BigDecimal(bConstant))
        {

        }

        public static EllypticCurve CreateWeierstrass(double aCoefficient, double bConstant)
        {
            return new WeierstrassCurve(aCoefficient, bConstant);
        }

        public static EllypticCurve CreateKublitz(double aCoefficient, double bConstant)
        {
            return new KublitzCurve(aCoefficient, bConstant);
        }

        internal virtual BigDecimal CalculateBeta(Point p1, Point p2)
        {
            return BigMath.Divide(
                BigMath.Subtract(p2.YCoordinate, p1.YCoordinate, Context), 
                BigMath.Subtract(p2.XCoordinate, p1.XCoordinate, Context), Context);
        }

        internal abstract BigDecimal CalculateAdditionXCoordinate(BigDecimal beta, Point p1, Point p2);

        internal virtual BigDecimal CalculateAdditionYCoordinate(BigDecimal beta, Point p1, BigDecimal x)
        {
            return BigMath.Subtract(
                BigMath.Multiply(
                    beta,
                    BigMath.Subtract(p1.XCoordinate, x, Context), Context),
                p1.YCoordinate, Context); 
        }

        internal abstract BigDecimal CalculateBeta(Point p);
    }
}
