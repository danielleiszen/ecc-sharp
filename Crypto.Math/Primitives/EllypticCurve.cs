using System;
using System.Diagnostics;
using System.Numerics;

namespace Crypto.Primitives
{
    public abstract class EllypticCurve
    {
        public BigInteger Coefficient { get; private set; }
        public BigInteger Constant { get; private set; }
        public BigInteger Modulo { get; private set; }
        public EllypticCurve(BigInteger aCoefficient, BigInteger bConstant, BigInteger modulo)
        {
            Modulo = modulo;
            Coefficient = aCoefficient;
            Constant = bConstant;
        }

        public EllypticCurve(BigInteger aCoefficient, BigInteger bConstant)
            : this(aCoefficient, bConstant, Configurations.MathConfiguration.Modulo)
        {
        }

        public EllypticCurve(int aCoefficient, int bConstant)
            : this(new BigInteger(aCoefficient), new BigInteger(bConstant))
        {

        }

        public static EllypticCurve CreateWeierstrass(int aCoefficient, int bConstant)
        {
            return new WeierstrassCurve(aCoefficient, bConstant);
        }

        public static EllypticCurve CreateWeierstrass(int aCoefficient, int bConstant, BigInteger modulo)
        {
            return new WeierstrassCurve(aCoefficient, bConstant, modulo);
        }

        public static EllypticCurve CreateKoblitz(int aCoefficient, int bConstant)
        {
            return new KoblitzCurve(aCoefficient, bConstant);
        }

        internal abstract BigInteger CalculateBeta(Point p);
        internal abstract BigInteger CalculateAdditionXCoordinate(BigInteger beta, Point p1, Point p2);
        public abstract bool IsOnCurve(Point point);

        internal virtual BigInteger CalculateBeta(Point p1, Point p2)
        {
            return BigInteger.Multiply(
                BigInteger.Subtract(p2.YCoordinate, p1.YCoordinate),
                FindModularInverse(BigInteger.Subtract(p2.XCoordinate, p1.XCoordinate))
                );
        }

        protected BigInteger FindModularInverse(BigInteger number)
        {
            number = number.ToPositiveMod(Modulo);
            var ret = number.FindModularInverse(Modulo);

            if(ret.HasValue == false)
            {
                throw new InvalidProgramException($"Modular inverse of {number} on F{Modulo} cannot be calculated");
            }

            return ret.Value;
        }

        internal virtual BigInteger CalculateAdditionYCoordinate(BigInteger beta, Point p1, BigInteger x)
        {
            return BigInteger.Subtract(
                BigInteger.Multiply(
                    beta,
                    BigInteger.Subtract(p1.XCoordinate, x)),
                p1.YCoordinate); 
        }
    }
}
