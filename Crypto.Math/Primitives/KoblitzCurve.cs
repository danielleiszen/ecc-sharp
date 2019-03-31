using System.Numerics;

namespace Crypto.Primitives
{
    internal class KoblitzCurve : EllypticCurve
    {
        public KoblitzCurve(BigInteger aCoefficient, BigInteger bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        public KoblitzCurve(int aCoefficient, int bConstant)
            : base(aCoefficient, bConstant)
        {

        }

        internal override BigInteger CalculateBeta(Point p)
        {
            return BigInteger.Multiply(
                BigInteger.Subtract(
                    BigInteger.Add(
                        BigInteger.Multiply(
                            new BigInteger(3), 
                            BigInteger.Pow(p.XCoordinate, 2)),
                        BigInteger.Multiply(
                            BigInteger.Multiply(
                                new BigInteger(2), 
                                Coefficient),
                            p.XCoordinate)), 
                    p.YCoordinate),
                FindModularInverse(
                    BigInteger.Add(
                        BigInteger.Multiply(
                            new BigInteger(2), 
                            p.YCoordinate), 
                        p.XCoordinate)));
        }

        internal override BigInteger CalculateAdditionXCoordinate(BigInteger beta, Point p1, Point p2)
        {
            return
                BigInteger.Subtract(
                    BigInteger.Add(
                        BigInteger.Pow(beta, 2),
                        beta)
                    ,
                    BigInteger.Add(
                        p1.XCoordinate, 
                        p2.XCoordinate)
                    );
        }

        public override bool IsOnCurve(Point point)
        {
            var left = BigInteger.Add(
                BigInteger.Pow(point.YCoordinate, 2),
                BigInteger.Multiply(
                    point.XCoordinate,
                    point.YCoordinate));

            var right = BigInteger.Add(
                BigInteger.Pow(point.XCoordinate, 3),
                BigInteger.Add(
                    BigInteger.Multiply(
                        BigInteger.Pow(point.XCoordinate, 2),
                        this.Coefficient),
                    this.Constant));

            return BigInteger.Compare(left, right) == 0;
        }
    }
}
