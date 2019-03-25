using Deveel.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto.Primitives
{
    public class Point
    {
        public Point(BigDecimal x, BigDecimal y)
        {
            XCoordinate = x;
            YCoordinate = y;
        }

        public Point(double x, double y)
        {
            XCoordinate = BigDecimal.Parse(x.ToString(), Configurations.MathConfiguration.CreateContext());
            YCoordinate = BigDecimal.Parse(y.ToString(), Configurations.MathConfiguration.CreateContext());
        }

        public Point(int x, int y)
        {
            XCoordinate = BigDecimal.Parse(x.ToString());
            YCoordinate = BigDecimal.Parse(y.ToString());
        }

        public BigDecimal XCoordinate { get; private set; }
        public BigDecimal YCoordinate { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                XCoordinate.Equals(point.XCoordinate) &&
                YCoordinate.Equals(point.YCoordinate);
        }

        public override int GetHashCode()
        {
            var hashCode = -1219734581;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigDecimal>.Default.GetHashCode(XCoordinate);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigDecimal>.Default.GetHashCode(YCoordinate);
            return hashCode;
        }

        public override string ToString()
        {
            return $"(x: {XCoordinate:# ##0.############}; y: {YCoordinate:# ##0.############})";
        }

        public void ReScale()
        {
            var xsd = XCoordinate.Precision - XCoordinate.Scale;
            var ysd = YCoordinate.Precision - YCoordinate.Scale;

            var xns = Configurations.MathConfiguration.Scale - xsd;
            var yns = Configurations.MathConfiguration.Scale - ysd;
            var ix = BigInteger.Parse($"{XCoordinate.UnscaledValue}".Substring(0, Configurations.MathConfiguration.Scale));
            var iy = BigInteger.Parse($"{YCoordinate.UnscaledValue}".Substring(0, Configurations.MathConfiguration.Scale));

            XCoordinate = new BigDecimal(ix, xns, Configurations.MathConfiguration.CreateContext());
            YCoordinate = new BigDecimal(iy, yns, Configurations.MathConfiguration.CreateContext());
        }

        public int EqualDigits(Point other)
        {
            var tx = XCoordinate.ToPlainString();
            var ox = other.XCoordinate.ToPlainString();
            var ty = YCoordinate.ToPlainString();
            var oy = other.YCoordinate.ToPlainString();

            var xi = 0;
            var yi = 0;

            for (; xi < tx.Length && xi < ox.Length && tx[xi] == ox[xi]; xi++) ;

            for (; yi < ty.Length && yi < oy.Length && ty[yi] == oy[yi]; yi++) ;

            return Math.Min(xi, yi);
        }

        public string DumpInfo()
        {
            return $"BitLength: {XCoordinate.UnscaledValue.BitLength}, BitCount: {XCoordinate.UnscaledValue.BitCount}, Length: {XCoordinate.ToPlainString().Length}";
        }
    }
}
