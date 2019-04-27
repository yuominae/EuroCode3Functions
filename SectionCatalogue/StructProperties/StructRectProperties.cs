using System;
namespace SectionCatalogue.StructProperties
{
    public class StructRectProperties : StructBaseProperties
    {
        public double h;
        public double b;
        public StructRectProperties(double depth, double width, string denom)
        {
            this.Denomination = denom;
            this.h = depth;
            this.b = width;
        }
        public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
        {
            return new EN1993LTBBucklingCurve
            {
                LTBBucklingCurve = EN1993LTBBucklingCurves.d
            };
        }
    }
}
