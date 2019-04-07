using System;
using System.Diagnostics;
using System.Numerics;

namespace Crypto.Primitives
{
    internal class WeierstrassCurve : EllypticCurve
    {
        public WeierstrassCurve(BigInteger aCoefficient, BigInteger bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        public WeierstrassCurve(BigInteger aCoefficient, BigInteger bConstant, BigInteger modulo)
            : base(aCoefficient, bConstant, modulo)
        {

        }


        public WeierstrassCurve(int aCoefficient, int bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        internal override BigInteger CalculateAdditionXCoordinate(BigInteger beta, Point p1, Point p2)
        {
            return BigInteger.Subtract(
                BigInteger.Pow(beta, 2),
                BigInteger.Add(p1.XCoordinate, p2.XCoordinate)
                );
        }

        internal override BigInteger CalculateBeta(Point p)
        {
            return BigInteger.Multiply(
                BigInteger.Add(
                    BigInteger.Multiply(
                        new BigInteger(3),
                        BigInteger.Pow(p.XCoordinate, 2)), 
                    Coefficient),
                FindModularInverse(
                    BigInteger.Multiply(
                        new BigInteger(2),
                        p.YCoordinate)
                    ));
        }

        public override BigInteger CalculateLeftSideOfEquality(Point point)
        {
            return BigInteger.Remainder(
                BigInteger.Pow(point.YCoordinate, 2),
                Modulo);
        }

        public override BigInteger CalculateRightSideOfEquality(Point point)
        {
            var lx3 = BigInteger.Pow(point.XCoordinate, 3);
            var lax = Coefficient * point.XCoordinate;

            var l2 = (lx3 + lax + Constant) % Modulo;

            //var x3 = BigInteger.Remainder(
            //    BigInteger.Multiply(
            //        BigInteger.Remainder(
            //            BigInteger.Multiply(point.XCoordinate, point.XCoordinate),
            //            Modulo),
            //        point.XCoordinate),
            //    Modulo);
            //var ax = BigInteger.Remainder(
            //    BigInteger.Multiply(this.Coefficient, point.XCoordinate),
            //    Modulo);

            //var ret = BigInteger.Remainder(
            //        BigInteger.Add(x3,
            //        BigInteger.Remainder(BigInteger.Add(ax, this.Constant), Modulo)
            //        ), Modulo);

            return l2;
        }
    }
}
