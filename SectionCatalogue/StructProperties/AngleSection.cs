using System;
namespace SectionCatalogue.StructProperties
{
    public class AngleSection : StructBaseProperties
    {
        public double h { get; set; }

        public double b { get; set; }

        public double t { get; set; }

        public double r1 { get; set; }

        public double r2 { get; set; }

        public SectionFabrication Fabrication = SectionFabrication.WELDED;

        public override Symmetry Symmetry
        {
            get
            {
                return Symmetry.ASYMMETRIC;
            }
        }

        public AngleSection(double Depth, double Width, double Thickness, string denom)
        {
            this.h = Depth;
            this.b = Width;
            this.t = Thickness;
            this.Denomination = denom;
        }

        public AngleSection(string[] Properties)
        {
            this.Denomination = Properties[0];
            this.weight = double.Parse(Properties[1]);
            this.h = double.Parse(Properties[2]);
            this.b = double.Parse(Properties[3]);
            this.t = double.Parse(Properties[4]);
            this.r1 = double.Parse(Properties[5]);
            this.r2 = double.Parse(Properties[6]);
            this.A = double.Parse(Properties[7]);
            this.Iyy = double.Parse(Properties[8]);
            this.Welyy = double.Parse(Properties[9]);
            this.Wplyy = double.Parse(Properties[10]);
            this.iy = double.Parse(Properties[11]);
            this.Izz = double.Parse(Properties[12]);
            this.Welzz = double.Parse(Properties[13]);
            this.Wplzz = double.Parse(Properties[14]);
            this.iz = double.Parse(Properties[15]);
            this.It = double.Parse(Properties[16]);
            this.Iw = double.Parse(Properties[17]);
        }

        public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
        {
            return new EN1993LTBBucklingCurve
            {
                LTBBucklingCurve = EN1993LTBBucklingCurves.d
            };
        }
    }
}
