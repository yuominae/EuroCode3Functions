using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructUserProperties : SectionBase
    {
        public double scaleFactor = 1.0;
        public StructUserProperties(string denom, double ScaleFactor)
        {
            this.Denomination = denom;
            this.scaleFactor = ScaleFactor;
        }
    }
}
