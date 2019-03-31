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

        public override bool IsOnCurve(Point point)
        {
            var left = BigInteger.Remainder(
                BigInteger.Pow(point.YCoordinate, 2), 
                Modulo);

            var right = BigInteger.Remainder(
                BigInteger.Remainder(
                    BigInteger.Multiply(
                        BigInteger.Remainder(
                            BigInteger.Multiply(point.XCoordinate, point.XCoordinate),
                            Modulo),
                        point.XCoordinate),
                    Modulo) +
                    BigInteger.Remainder(this.Coefficient * point.XCoordinate, Modulo) +
                    this.Constant,
                Modulo);

            left = left.ToPositiveMod(Modulo);
            right = right.ToPositiveMod(Modulo);

            return BigInteger.Compare(left, right) == 0;
        }
    }
}
