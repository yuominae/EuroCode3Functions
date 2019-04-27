using System;
namespace SectionCatalogue.StructProperties
{
    public class StructRectCircProperties : StructBaseProperties
    {
        public double h;
        public double b;
        public StructRectCircProperties(double depth, double width, string denom)
        {
            this.h = depth;
            this.b = width;
            this.Denomination = denom;
        }
    }
}
