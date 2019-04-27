using System;
namespace SectionCatalogue.StructProperties
{
    public class StructCircProperties : StructBaseProperties
    {
        public double d;
        public StructCircProperties(double Diameter, string denom)
        {
            this.Denomination = denom;
            this.d = Diameter;
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
