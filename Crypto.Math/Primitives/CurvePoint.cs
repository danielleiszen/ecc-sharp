using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Crypto.Primitives
{
    public class CurvePoint
    {
        public EllypticCurve Curve { get; private set; }
        public Point Point { get; private set; }
        public BigInteger Order { get; private set; }

        public CurvePoint(EllypticCurve curve, Point point, BigInteger? orderOfGroup = default(BigInteger?))
        {
            if (curve.IsOnCurve(point))
            {
                Console.WriteLine($"Point {point} is on curve {curve.Coefficient}, {curve.Constant}");
            }
            else 
            {
//                throw new ArgumentException($"Point {point} is not on curve {curve.Coefficient}, {curve.Constant}");
                Console.WriteLine($"Point {point} is not on curve {curve.Coefficient}, {curve.Constant}");
            }

            Curve = curve;
            Point = point;

            if(orderOfGroup.HasValue)
            {
                Order = orderOfGroup.Value;
            }
            else
            {
                Order = FindOrderOfGroup();
                Console.WriteLine($"Order of group is calculated {Order}");
            }
        }

        public CurvePoint(CurvePoint generation, BigInteger multiplier)
        {
            Curve = generation.Curve;
            Order = generation.Order;
            Point = Curve.CalculateMultiplication(generation.Point, multiplier);
        }

        public BigInteger FindOrderOfGroup()
        {
            var ret = BigInteger.Zero;

            var q = Curve.CalculateMultiplication(Point, BigInteger.Add(Curve.Modulo, BigInteger.One));
            var m = BigInteger.Add(Curve.Modulo.SquareRoot().SquareRoot(), BigInteger.One);

            for(var j = BigInteger.One; BigInteger.Compare(j, m) < 1; j = BigInteger.Add(j, BigInteger.One))
            {
                var jp = Curve.CalculateMultiplication(Point, j);

                for(var k = BigInteger.Multiply(m, BigInteger.MinusOne); BigInteger.Compare(k, m) < 1; k = BigInteger.Add(k, BigInteger.One))
                {
                    var twoMK = BigInteger.Multiply(BigInteger.Multiply(new BigInteger(2), m), k);
                    var cp = Curve.CalculateMultiplication(Point, twoMK);
                    cp = Curve.CalculateAddition(cp, q);

                    if(BigInteger.Compare(cp.XCoordinate, jp.XCoordinate) == 0)
                    {
                        ret = BigInteger.Add(Curve.Modulo, BigInteger.Add(BigInteger.One, twoMK));

//                        Console.WriteLine($"Order of group: {ret} +- {j}");

                        var ret1 = BigInteger.Add(ret, j);
                        var ret2 = BigInteger.Subtract(ret, j);

                        if (CheckInfinity(ret1))
                        {
                            return ret1;
                        }
                        else if (CheckInfinity(ret2))
                        {
                            return ret2;
                        }
                    }
                }
            }

            return ret;
        }

        private bool CheckInfinity(BigInteger multiplier)
        {
            var mo = Curve.CalculateMultiplication(Point, BigInteger.Subtract(multiplier, BigInteger.One));
            var m = Curve.CalculateMultiplication(Point, multiplier);

            var po = Curve.CalculateMultiplication(Point, BigInteger.Add(multiplier, BigInteger.One));
            var po2 = Curve.CalculateAddition(m, Point);

//            Console.WriteLine($"Checking infinity of {multiplier}, Before {mo}, Point: {m}, After {po}, After add {po2}");

            return Point.Equals(po2, po) == false && Point.Equals(po2, mo);
        }

        public CurvePoint Multiply(BigInteger scalar)
        {
            return new CurvePoint(this, scalar);
        }

        public Point SignMessage(string message, BigInteger privateKey, BigInteger? randomKey = default(BigInteger?))
        {
            var hash = BigIntegerExtensions.ComputeSHA1(message);
            /// Console.WriteLine($"Hash of {message} is {hash}");
            // Console.WriteLine($"Private key: {privateKey}");

            if (randomKey.HasValue == false)
            {
                randomKey = BigInteger.Remainder(Core.RandomService.Generate(), Order);
            }

            var randomPoint = Multiply(randomKey.Value);
            // Console.WriteLine($"Random key: {randomKey}, Random point: {randomPoint}");

            var rmodinv = randomKey.Value.FindModularInverse(Order);

            if (rmodinv.HasValue)
            {
                var r = BigInteger.Remainder(
                    randomPoint.Point.XCoordinate,
                    randomPoint.Order);
                var s = BigInteger.Remainder(
                    BigInteger.Multiply(
                        BigInteger.Add(
                            hash,
                            BigInteger.Multiply(r, privateKey)),
                        rmodinv.Value),
                    randomPoint.Order);

                var ret = new Point(r, s);
                // Console.WriteLine($"Signature {ret}");

                return ret;
            }
            else
            {
                throw new InvalidOperationException($"Multiplicative modular inverse of {randomKey} cannot be calculated on mod {Order}");
            }
        }

        public bool VerifySignature(string message, Point signature, CurvePoint publicKey)
        {
            var hash = BigIntegerExtensions.ComputeSHA1(message);
            // Console.WriteLine($"Hash of {message} is {hash}");

            var w = signature.YCoordinate.FindModularInverse(Order);
            /// Console.WriteLine($"W: {w}");

            if (w.HasValue)
            {
                var u1 = Multiply(
                    BigInteger.Remainder(
                        BigInteger.Multiply(
                            hash,
                            w.Value),
                        Order));

                var u2 = publicKey.Multiply(
                    BigInteger.Remainder(
                        BigInteger.Multiply(
                            signature.XCoordinate,
                            w.Value),
                        publicKey.Order));

                // Console.WriteLine($"u1: {u1}, u2: {u2}");

                var check = Curve.CalculateAddition(u1.Point, u2.Point);
                var ret = BigInteger.Equals(check.XCoordinate, signature.XCoordinate);
                // Console.WriteLine($"Checkpoint {check} X == signature {signature} X: {ret}");

                return ret;
            }
            else
            {
                throw new InvalidOperationException($"Multiplicative modular inverse of {signature.YCoordinate} cannot be calculated on mod {Order}");
            }
        }

        public override string ToString()
        {
            return $"{Point}";
        }

        public static CurvePoint BitCoinGenerator()
        {
            var curve = EllypticCurve.CreateWeierstrass(0, 7, Configurations.MathConfiguration.GenerateSECP256K1());

            var g = new Point(
                BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"),
                BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424"));

            return new CurvePoint(curve, g,
                BigIntegerExtensions.FromBigEndianHexString("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141"));
        }

        public static CurvePoint SimpleGenerator()
        {
            var curve = EllypticCurve.CreateWeierstrass(0, 7, new BigInteger(199));
            var g = new Point(
                new BigInteger(2),
                new BigInteger(24));

            return new CurvePoint(curve, g);
        }
    }
}
