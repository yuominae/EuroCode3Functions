using System;
namespace SectionCatalogue
{
    public enum EN1993LTBBucklingCurves
    {
        a,
        b,
        c,
        d
    }

    public class EN1993LTBBucklingCurve
    {
        public EN1993LTBBucklingCurves LTBBucklingCurve { get; set; } 
        
        public EN1993LTBBucklingCurve()
        {
            this.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
        }
    }
}
