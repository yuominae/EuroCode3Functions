using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructDiamProperties : SectionBase
    {
        public double h;
        public double b;
        public StructDiamProperties(double depth, double width, string denom)
        {
            this.h = depth;
            this.b = width;
            this.Denomination = denom;
        }
    }
}
