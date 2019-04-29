using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructTapAngProperties : SectionBase
    {
        public double h;
        public double b;
        public double twt;
        public double tf;
        public double twb;
        public StructTapAngProperties(double depth, double width, double webThickTop, double webThickBot, double flangeThick, string denom)
        {
            this.Denomination = denom;
            this.h = depth;
            this.b = width;
            this.twt = webThickTop;
            this.twb = webThickBot;
            this.tf = flangeThick;
        }
    }
}
