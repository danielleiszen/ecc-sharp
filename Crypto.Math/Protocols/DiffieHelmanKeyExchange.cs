using Crypto.Entities;
using Crypto.Primitives;
using System;

namespace Crypto.Protocols
{
    public interface IDiffeHelmanClient
    {
        void SendPublicKeyTo(IDiffeHelmanClient client);
        void GenerateSharedKey(Party remote);
        void CheckEquality(IDiffeHelmanClient other);
    }

    public class DiffieHelmanKeyExchange : Process, IDiffeHelmanClient
    {
        public Party Remote { get; private set; }
        public CurvePoint Shared { get; private set; }

        private DiffieHelmanKeyExchange(CurvePoint generation, Party actor)
            : base(generation, actor)
        {
        }

        public static IDiffeHelmanClient CreateClient(CurvePoint setup, Party actor)
        {
            return new DiffieHelmanKeyExchange(setup, actor);
        }

        void IDiffeHelmanClient.SendPublicKeyTo(IDiffeHelmanClient client)
        {
            client.GenerateSharedKey(Actor.ToPublic());
        }

        void IDiffeHelmanClient.GenerateSharedKey(Party remote)
        {
            Remote = remote;
            Shared = Actor.GenerateKey(remote.PublicKey);
        }

        public override string ToString()
        {
            return $"Public key: {Actor.PublicKey.Point}, Remote key: {Remote.PublicKey.Point}, Shared key: {Shared.Point}";
        }

        void IDiffeHelmanClient.CheckEquality(IDiffeHelmanClient other)
        {
            Console.WriteLine($"Key: {Shared.Point}");

            if(other is DiffieHelmanKeyExchange dright)
            {
                Console.WriteLine($"Key: {dright.Shared.Point}");
                Console.WriteLine($"The two keys are equal: {Shared.Point.Equals(dright.Shared.Point)}");
            }
        }
    }
}
