using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructRectProperties : SectionBase
    {
        public double h;
        public double b;
        public StructRectProperties(double depth, double width, string denom)
        {
            this.Denomination = denom;
            this.h = depth;
            this.b = width;
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
