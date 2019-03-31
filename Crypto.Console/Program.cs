using Crypto.Entities;
using Crypto.Primitives;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            Configurations.MathConfiguration.KeyBitLength = 32;

            var generator = CurvePoint.BitCoinGenerator();

            var alice = new Party(generator);
            var bob = new Party(generator);

            SignMessage(alice, bob);

            DiffieHelman(generator, alice, bob);
            //AttackDH(generator, alice, bob);

            //BruteForce(generator, 211);
        }

        public static void SignMessage(Party alice, Party bob)
        {
            var signature = alice.SignMessage("ECC beats RSA");
            var signed = bob.VerifySignature("ECC beats RSA", signature, alice.ToPublic());
            var manipulated = bob.VerifySignature("RSA beats ECC", signature, alice.ToPublic());

            Console.WriteLine($"Message signature is valid: {signed}, Manupulated message signature is valid: {manipulated}");
        }

        public static void DiffieHelman(CurvePoint generator, Party alice, Party bob)
        {
            var a = Protocols.DiffieHelmanKeyExchange.CreateClient(generator, alice);
            var b = Protocols.DiffieHelmanKeyExchange.CreateClient(generator, bob);

            a.SendPublicKeyTo(b);
            b.SendPublicKeyTo(a);

            Console.WriteLine($"Alice: {a}");
            Console.WriteLine($"Bob: {b}");

            a.CheckEquality(b);

            Console.WriteLine($"Alice: {alice}");
            Console.WriteLine($"Bob: {bob}");
        }

        public static void AttackDH(CurvePoint generator, Party alice, Party bob)
        {
            var aprivate = FindPoint(generator, alice.PublicKey.Point);
            var bprivate = FindPoint(generator, bob.PublicKey.Point);

            Console.WriteLine($"Alice refactored private key {aprivate}");
            Console.WriteLine($"Bob refactored private key {bprivate}");
        }

        public static BigInteger FindPoint(CurvePoint generator, Point target)
        { 
            Console.WriteLine($"Generation: {generator}");

            var watch = new Stopwatch();
            watch.Start();

            var i = BigInteger.Parse("1");
            var p = new Point(generator.Point.XCoordinate, generator.Point.YCoordinate);

            Console.Write($"Calculating {target}: {i:0000000000#}");
            while(target.Equals(p) == false)
            {
                p = generator.Curve.CalculateAddition(p, generator.Point);
                i += 1;
                Console.Write($"\b\b\b\b\b\b\b\b\b\b          \b\b\b\b\b\b\b\b\b\b\b{i:0000000000#}");
            }

            watch.Stop();
            Console.WriteLine();
            Console.WriteLine($"Brute force result: {p} took {watch.ElapsedMilliseconds} ms");
            watch.Reset();

            return i;
        }

        public static void BruteForce(CurvePoint g, BigInteger iterations)
        {
            Console.WriteLine($"G: {g.Point}");

            var p = new Point(g.Point.XCoordinate, g.Point.YCoordinate);
            var m = g.Curve.CalculateMultiplication(g.Point, iterations);

            for (var i = new BigInteger(2); i <= iterations; i++)
            {
                p = g.Curve.CalculateAddition(p, g.Point);
                Console.WriteLine($"P{i}: {p}");
            }

            Console.WriteLine($"Multiplication: {m}");
        }
    }
}
