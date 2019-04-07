using Crypto.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Crypto.Protocols
{
    public interface IMessageMap
    {
        IEnumerable<Point> Map(string message);
        string Extract(IEnumerable<Point> points);
    }

    public abstract class MessageMapping : IMessageMap
    {
        protected CurvePoint Generator { get; private set; }

        public MessageMapping(CurvePoint generator)
        {
            Generator = generator;
        }

        protected abstract IEnumerable<Point> MapMessage(string message);
        protected abstract string ExtractMessage(IEnumerable<Point> points);

        IEnumerable<Point> IMessageMap.Map(string message)
        {
            return MapMessage(message);
        }

        string IMessageMap.Extract(IEnumerable<Point> points)
        {
            return ExtractMessage(points);
        }
    }

    public class Sec1702Mapping : MessageMapping
    {
        public Sec1702Mapping(CurvePoint generator)
            : base(generator) { }

        protected override string ExtractMessage(IEnumerable<Point> points)
        {
            var bytes = new List<byte>();

            foreach(var point in points)
            {
                var chunk = point.XCoordinate.ToByteArray();
                chunk = chunk.Reverse().ToArray();

                for(var i = 0; i < chunk.Length - 1; i++)
                {
                    bytes.Add(chunk[i]);
                }
            }

            var base64 = Encoding.ASCII.GetString(bytes.ToArray());
            var textbytes = Convert.FromBase64String(base64.TrimEnd('\0'));
            return Encoding.UTF8.GetString(textbytes);
        }

        protected override IEnumerable<Point> MapMessage(string message)
        {
            var orderArray = Generator.Order.ToByteArray();
            /// key size of curve
            var size = (orderArray[orderArray.Length - 1] == 0 ? orderArray.Length - 1 : orderArray.Length) * 8D;
            // maximum size of message chunks mapped to points
            var M = (int) Math.Floor((size - 8D) / 8D);
            // number of zero bits at the end for valid point checking
            var N = 8;

            var textbytes = Encoding.UTF8.GetBytes(message);
            var base64 = Convert.ToBase64String(textbytes);
            var bytes = Encoding.ASCII.GetBytes(base64);

            var index = 0;

            while(index < bytes.Length)
            {
                var chunk = new byte[M + 1];

                // copy the next M bytes into chunk
                do
                {
                    chunk[index % M] = bytes[index];
                    index++;
                }
                while (index % M > 0 && index < bytes.Length);
                
                chunk[M] = 0;

                var x = BigIntegerExtensions.FromBigEndianByteArray(chunk);
                var add = BigInteger.MinusOne;
                var point = default(Point);

                do
                {
                    add = BigInteger.Add(add, BigInteger.One);

                    if(add == 256)
                    {
                        throw new InvalidProgramException("The message mapping algorithm cannot map this message");
                    }

                    var xa = BigInteger.Add(x, add);
                    var right = Generator.Curve.CalculateRightSideOfEquality(new Point(xa, 0));
                    var ys = right.FindModularSquareRoots(Generator.Curve.Modulo);

                    point = new Point(xa, BigInteger.Min(ys.Item1, ys.Item2));
                }
                while (Generator.Curve.IsOnCurve(point) == false);

                yield return point;
            }
        }
    }
}
