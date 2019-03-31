using Crypto.Primitives;
using System;
using System.Numerics;

namespace Crypto.Entities
{
    public class Party
    {
        private BigInteger PrivateKey { get; set; }
        public CurvePoint Generator { get; private set; }
        public CurvePoint PublicKey { get; private set; }
        public bool IsLocal => PrivateKey != null;
        public bool IsParticipant => PublicKey != null;
        public Party(CurvePoint generator)
        {
            Generator = generator;
            PrivateKey =  Core.RandomService.Generate(Configurations.MathConfiguration.KeyBitLength);
            PublicKey = generator.Multiply(PrivateKey);
        }

        public Party(CurvePoint generator, CurvePoint publicKey)
        {
            Generator = generator;
            PublicKey = publicKey;
        }

        public Party ToPublic()
        {
            return new Party(Generator, PublicKey);
        }

        internal CurvePoint GenerateKey(CurvePoint generator)
        {
            return new CurvePoint(generator, PrivateKey);
        }

        public Point SignMessage(string message)
        {
            return Generator.SignMessage(message, PrivateKey);
        }

        public bool VerifySignature(string message, Point signature)
        { 
            return Generator.VerifySignature(message, signature, PublicKey);
        }

        public bool VerifySignature(string message, Point signature, Party signer)
        {
            return Generator.VerifySignature(message, signature, signer.PublicKey);
        }

        public bool VerifySignature(string message, Point signature, CurvePoint publicKey)
        {
            return Generator.VerifySignature(message, signature, publicKey);
        }

        public override string ToString()
        {
            var years = BigInteger.Divide(PrivateKey, new BigInteger(26542800000));

            return IsLocal && IsParticipant ? $"Private: {PrivateKey} ({years} years to crack), Public: {PublicKey}" : $"Public: {PublicKey}";
        }
    }
}
