using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructChannelProperties : SectionBase
    {
        public double h;
        public double b;
        public double tw;
        public double tf;
        public StructChannelProperties(double depth, double width, double webThick, double flangeThick, string denom)
        {
            this.Denomination = denom;
            this.h = depth;
            this.b = width;
            this.tw = webThick;
            this.tf = flangeThick;
        }
        public StructChannelProperties(string def)
        {
            this.Denomination = def;
            string[] array = def.Substring(2, def.Length - 2).Split("x".ToCharArray());
            this.h = double.Parse(array[0]) / 1000.0;
            this.b = double.Parse(array[1]) / 1000.0;
            this.tw = double.Parse(array[2]) / 1000.0;
            this.tf = double.Parse(array[3]) / 1000.0;
        }
        public override EN1993LTBBucklingCurve GetEN1993LTBBucklingCurve()
        {
            return new EN1993LTBBucklingCurve
            {
                LTBBucklingCurve = EN1993LTBBucklingCurves.d
            };
        }
    }
}
