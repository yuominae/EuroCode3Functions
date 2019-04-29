using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructCircProperties : SectionBase
    {
        public double d;
        public StructCircProperties(double Diameter, string denom)
        {
            this.Denomination = denom;
            this.d = Diameter;
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
