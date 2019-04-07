using Crypto.Entities;
using Crypto.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Crypto.Protocols
{
    public interface IEncryptionDecryption
    {
        IEnumerable<Tuple<CurvePoint, CurvePoint>> EncryptWith(string message, BigInteger secretKey);
        string DecryptWith(IEnumerable<Tuple<CurvePoint, CurvePoint>> cypher, BigInteger secretKey);
    }

    public interface IEncryptionClient
    { 
        IEnumerable<Tuple<CurvePoint, CurvePoint>> EncryptTo(string message, Party recipient);
        string Decrypt(IEnumerable<Tuple<CurvePoint, CurvePoint>> cypher);
    }

    public class EncryptionDecryption : Process, IEncryptionDecryption, IEncryptionClient
    {
        public IMessageMap Mapping { get; private set; }

        protected EncryptionDecryption(CurvePoint generator, Party actor)
            : base(generator, actor)
        {
            Mapping = new Sec1702Mapping(generator);
        }

        public static IEncryptionClient CreateClient(CurvePoint generator, Party owner)
        {
            return new EncryptionDecryption(generator, owner);
        }

        protected Tuple<CurvePoint, CurvePoint> Encrypt(Point message, CurvePoint publicKey)
        {
            var randomKey = BigInteger.Remainder(Core.RandomService.Generate(), Generator.Order);
            var c1 = Generator.Multiply(randomKey);
            var c2 = publicKey.Multiply(randomKey);
            c2.Add(message);

            return new Tuple<CurvePoint, CurvePoint>(c1, c2);
        }

        protected Point Decrypt(CurvePoint d, CurvePoint c2)
        {
            var dinv = new Point(d.Point.XCoordinate, BigInteger.Multiply(d.Point.YCoordinate, BigInteger.MinusOne));

            c2.Add(dinv);

            return c2.Point;
        }

        IEnumerable<Tuple<CurvePoint, CurvePoint>> IEncryptionDecryption.EncryptWith(string message, BigInteger secretKey)
        {
            var publicKey = Generator.Multiply(secretKey);

            var points = Mapping.Map(message);
            return points.Select(p => Encrypt(p, publicKey)).ToList();
        }

        IEnumerable<Tuple<CurvePoint, CurvePoint>> IEncryptionClient.EncryptTo(string message, Party recipient)
        {
            var points = Mapping.Map(message);
            return points.Select(p => Encrypt(p, recipient.PublicKey)).ToList();
        }

        string IEncryptionDecryption.DecryptWith(IEnumerable<Tuple<CurvePoint, CurvePoint>> cypher, BigInteger secretKey)
        {
            var messages = cypher.Select(c => Decrypt(c.Item1.Multiply(secretKey), c.Item2));

            return Mapping.Extract(messages);
        }

        string IEncryptionClient.Decrypt(IEnumerable<Tuple<CurvePoint, CurvePoint>> cypher)
        {
            var messages = cypher.Select(c => Decrypt(Actor.Multiply(c.Item1), c.Item2));

            return Mapping.Extract(messages);
        }
    }
}
