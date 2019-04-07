using Crypto.Entities;
using Crypto.Primitives;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            Configurations.MathConfiguration.KeyBitLength = 192;

            var generator = CurvePoint.NIST121P192Generator();
            var alice = new Party(generator);
            var bob = new Party(generator);

            EncryptDecrypt(generator, alice, bob,
                "Elképesztő a trió egyéni GS-statisztikája is, amit ha csak megközelít majd valaki, biztosan a sportág meghatározó egyénisége lesz. Lehet akár Alexander Zverev is, aki 22 éves lesz, de már 27. a pénzkereseti örökranglistán. Tíz tornát nyert pályafutása során, de Grand Slamen egy párizsi negyeddöntő a legjobb eredménye eddig. Sokan azt mondják, hogy az oroszból lett német, agresszív alapvonaljátékos Zverev lehet a fiatalok közül, aki először csúcsra ér.Bombaerős tenyeresei és fonákjai is életveszélyesek, magas testalkata ellenére is alacsony a súlypontja.A 220 kilométer / órás szerváira felkészülni szinte lehetetlen. Federer 22 éves kora előtt három hónappal, Nadal három nappal 19.születésnapja előtt, Djokovics pedig két hónappal 20.születésnapja előtt volt életében először harmadik a világranglistán.Zverevnek ez 19 és fél évesen sikerült.");
            //AttackDH(generator, alice, bob);

            //BruteForce(generator, 1000);
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

        public static void EncryptDecrypt(CurvePoint generator, Party alice, Party bob, string message)
        {
            Console.WriteLine($"Original text: {message}");

            var aenc = Protocols.EncryptionDecryption.CreateClient(generator, alice);
            var benc = Protocols.EncryptionDecryption.CreateClient(generator, bob);

            var cypher = aenc.EncryptTo(message, bob.ToPublic());

            Console.WriteLine($"Cypher encrypted to {cypher.Count()} points");
            var text = benc.Decrypt(cypher);

            Console.WriteLine($"Decrypted text: {text}");
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

                var right = g.Curve.CalculateRightSideOfEquality(p);
                var ys = right.FindModularSquareRoots(g.Curve.Modulo);

                var on1 = g.Curve.IsOnCurve(new Point(p.XCoordinate, ys.Item1));
                var on2 = g.Curve.IsOnCurve(new Point(p.XCoordinate, ys.Item2));

                Console.WriteLine($"Y1: {on1}, Y2: {on2}, Original: {ys.Item1 == p.YCoordinate}, {ys.Item2 == p.YCoordinate}");
            }

            Console.WriteLine($"Multiplication: {m}");
        }
    }
}
