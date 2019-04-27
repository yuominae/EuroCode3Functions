using System;
namespace SectionCatalogue.StructProperties
{
    public class StructTaperProperties : StructBaseProperties
    {
        public double h;
        public double bt;
        public double bb;
        public StructTaperProperties(double depth, double widthTop, double widthBot, string denom)
        {
            this.h = depth;
            this.bt = widthTop;
            this.bb = widthBot;
            this.Denomination = denom;
        }
    }
}
