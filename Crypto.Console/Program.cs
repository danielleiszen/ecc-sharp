using Crypto.Entities;
using Crypto.Primitives;
using Deveel.Math;
using System;
using System.Diagnostics;

namespace Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            Configurations.MathConfiguration.Precision = 596;
            Configurations.MathConfiguration.KeyBitLength = 481;
            Configurations.MathConfiguration.Scale = 300;

            var curve = EllypticCurve.CreateWeierstrass(-4, 4);
            var g = new Point(-2, -2);

            var generator = new CurvePoint(curve, g);
            DiffieHelman(generator);
        }

        public static void DiffieHelman(CurvePoint generator)
        {
            var alice = new Party();
            var bob = new Party();

            var a = Protocols.DiffieHelmanKeyExchange.CreateClient(generator, alice);
            var b = Protocols.DiffieHelmanKeyExchange.CreateClient(generator, bob);

            a.SendPublicKeyTo(b);
            b.SendPublicKeyTo(a);

            alice.PublicKey.Point.ReScale();
            bob.PublicKey.Point.ReScale();
            a.ReScale();
            b.ReScale();

            Console.WriteLine($"Alice: {a}");
            Console.WriteLine($"Bob: {b}");

            a.CheckEquality(b);

            Console.WriteLine($"Alice: {alice}");
            Console.WriteLine($"Bob: {bob}");

            //var aprivate = FindPoint(generator, alice.PublicKey.Point);
            //var bprivate = FindPoint(generator, bob.PublicKey.Point);

            //Console.WriteLine($"Alice refactored private key {aprivate}");
            //Console.WriteLine($"Bob refactored private key {bprivate}");
        }

        public static BigInteger FindPoint(CurvePoint generator, Point target)
        { 
            Console.WriteLine($"Generation: {generator}");

            var watch = new Stopwatch();
            watch.Start();

            var i = BigInteger.Parse("1");
            var p = new Point(generator.Point.XCoordinate, generator.Point.YCoordinate);
            while(target.Equals(p))
            {
                p = generator.Curve.CalculateAddition(p, generator.Point);
                i += 1;
            }

            watch.Stop();
            Console.WriteLine($"Brute force result: {p} took {watch.ElapsedMilliseconds} ms");
            watch.Reset();

            return i;
        }
    }
}
