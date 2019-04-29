using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructEllipseProperties : SectionBase
    {
        public double h;
        public double b;
        public StructEllipseProperties(double depth, double width, string denom)
        {
            this.h = depth;
            this.b = width;
            this.Denomination = denom;
        }
    }
}
