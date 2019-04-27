using System;
namespace SectionCatalogue.StructProperties
{
    public class StructUserProperties : StructBaseProperties
    {
        public double scaleFactor = 1.0;
        public StructUserProperties(string denom, double ScaleFactor)
        {
            this.Denomination = denom;
            this.scaleFactor = ScaleFactor;
        }
    }
}
