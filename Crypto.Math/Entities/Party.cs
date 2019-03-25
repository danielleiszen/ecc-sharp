using Crypto.Primitives;
using Crypto.Protocols;
using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Entities
{
    public class Party
    {
        private BigInteger PrivateNumber { get; set; }
        public CurvePoint PublicKey { get; private set; }
        public bool IsLocal => PrivateNumber != null;
        public bool IsParticipant => PublicKey != null;
        public Party()
        {
            PrivateNumber =  Core.RandomService.Generate(Configurations.MathConfiguration.KeyBitLength);
        }

        public Party(CurvePoint publicKey)
        {
            PublicKey = publicKey;
        }
        public Party(EllypticCurve curve, Point publicPoint)
        {
            PublicKey = new CurvePoint(curve, publicPoint);
        }

        public Party ToPublic()
        {
            return new Party(PublicKey);
        }

        internal CurvePoint GenerateKey(CurvePoint generator)
        {
            return new CurvePoint(generator, PrivateNumber);
        }

        internal void GeneratePublicKey(CurvePoint generator)
        {
            PublicKey = GenerateKey(generator);
        }

        public override string ToString()
        {
            return IsLocal && IsParticipant ? $"Private: {PrivateNumber}, Public: {PublicKey}" : $"Public: {PublicKey}";
        }
    }
}
