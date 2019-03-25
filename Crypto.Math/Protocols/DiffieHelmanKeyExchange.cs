using Crypto.Entities;
using Crypto.Primitives;
using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Protocols
{
    public interface IDiffeHelmanClient
    {
        void SendPublicKeyTo(IDiffeHelmanClient client);
        void GenerateSharedKey(Party remote);
        void CheckEquality(IDiffeHelmanClient other);
        void ReScale();
    }

    public class DiffieHelmanKeyExchange : Process, IDiffeHelmanClient
    {
        public Party Remote { get; private set; }
        public CurvePoint Shared { get; private set; }

        private DiffieHelmanKeyExchange(CurvePoint generation, Party actor)
            : base(generation, actor)
        {
            actor.GeneratePublicKey(generation);
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

        void IDiffeHelmanClient.ReScale()
        {
            Shared.Point.ReScale();
        }

        void IDiffeHelmanClient.CheckEquality(IDiffeHelmanClient other)
        {
            var digist = 0;

            Console.WriteLine($"Info: {Shared.Point.DumpInfo()}");

            if(other is DiffieHelmanKeyExchange dright)
            {
                Console.WriteLine($"Info: {dright.Shared.Point.DumpInfo()}");
                digist = Shared.Point.EqualDigits(dright.Shared.Point);
                Console.WriteLine($"Precision: {Shared.Point.XCoordinate.Precision}, Scale: {Shared.Point.XCoordinate.Scale}");
            }

            Console.WriteLine($"Two keys have {digist} equal digits");
        }

    }
}
