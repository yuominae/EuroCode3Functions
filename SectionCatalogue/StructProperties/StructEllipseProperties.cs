using System;
namespace SectionCatalogue.StructProperties
{
    public class StructEllipseProperties : StructBaseProperties
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
