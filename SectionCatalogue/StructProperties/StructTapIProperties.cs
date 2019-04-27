using System;
namespace SectionCatalogue.StructProperties
{
    public class StructTapIProperties : StructBaseProperties
    {
        public double h;
        public double bt;
        public double bb;
        public double twt;
        public double twb;
        public double tfb;
        public double tft;
        public StructTapIProperties(double depth, double widthTop, double widthBot, double webThickTop, double webThickBot, double FlangeThickTop, double FlangeThickBot, string denom)
        {
            this.h = depth;
            this.bt = widthTop;
            this.bb = widthBot;
            this.twt = webThickTop;
            this.twb = webThickBot;
            this.tft = FlangeThickTop;
            this.tfb = FlangeThickBot;
            this.Denomination = denom;
        }
    }
}
