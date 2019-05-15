using System;
namespace SectionCatalogue.SectionProperties
{
    public class RectangularHollowSection : SectionBase
    {
        public double h { get; set; }

        public double b { get; set; }

        public double t { get; set; }

        public SectionFabrication Fabrication { get; set; }

        public override Symmetry Symmetry
        {
            get
            {
                return Symmetry.DoublySymmetric;
            }
        }

        public RectangularHollowSection(double depth, double width, double thickness, string denom)
        {
            this.h = depth;
            this.b = width;
            this.t = thickness;
            this.A = this.h * this.b - (this.h - 2.0 * this.t) * (this.b - 2.0 * this.t);
            this.Iyy = (this.b * Math.Pow(this.h, 3.0) - (this.b - 2.0 * this.t) * Math.Pow(this.h - 2.0 * this.t, 3.0)) / 12.0;
            this.Welyy = this.Iyy / (this.h / 2.0);
            this.Wplyy = 2.0 * (this.b * this.t * (this.h / 2.0 - this.t / 2.0) + this.t * Math.Pow(this.h / 2.0 - this.t, 2.0));
            this.iy = Math.Sqrt(this.Iyy / this.A);
            this.Izz = (this.h * Math.Pow(this.b, 3.0) - (this.h - 2.0 * this.t) * Math.Pow(this.b - 2.0 * this.t, 3.0)) / 12.0;
            this.Welzz = this.Izz / (this.b / 2.0);
            this.Wplzz = 2.0 * (this.h * this.t * (this.b / 2.0 - this.t / 2.0) + this.t * Math.Pow(this.b / 2.0 - this.t, 2.0));
            this.iz = Math.Sqrt(this.Izz / this.A);
            this.It = 4.0 * Math.Pow((this.b - this.t) * (this.h - this.t), 2.0) * this.t / (2.0 * (this.b + this.h));
            this.Iw = 0.0;
        }

        public RectangularHollowSection(string[] Properties)
        {
            this.Denomination = Properties[0];
            this.Weight = double.Parse(Properties[1]);
            this.h = double.Parse(Properties[2]);
            this.b = double.Parse(Properties[3]);
            this.t = double.Parse(Properties[4]);
            this.A = double.Parse(Properties[5]);
            this.Iyy = double.Parse(Properties[6]);
            this.Welyy = double.Parse(Properties[7]);
            this.Wplyy = double.Parse(Properties[8]);
            this.iy = double.Parse(Properties[9]);
            this.Izz = double.Parse(Properties[10]);
            this.Welzz = double.Parse(Properties[11]);
            this.Wplzz = double.Parse(Properties[12]);
            this.iz = double.Parse(Properties[13]);
            this.It = double.Parse(Properties[14]);
            this.Iw = double.Parse(Properties[15]);
        }

        public override int GetEN1993CompressionClass(double fy)
        {
            double num = base.CalcEpsilon(fy);
            double num2 = this.h - 2.0 * this.t;
            if (this.b - 2.0 * this.t > num2)
            {
                num2 = this.b - 2.0 * this.t;
            }
            if (num2 / this.t <= 33.0 * num)
            {
                return 1;
            }
            if (num2 / this.t <= 38.0 * num)
            {
                return 2;
            }
            if (num2 / this.t <= 42.0 * num)
            {
                return 3;
            }
            return 4;
        }

        public override int GetEN1993FlexureClass(double fy)
        {
            double num = base.CalcEpsilon(fy);
            double num2 = this.h - 2.0 * this.t;
            if (this.b - 2.0 * this.t > num2)
            {
                num2 = this.b - 2.0 * this.t;
            }
            if (num2 / this.t <= 72.0 * num)
            {
                return 1;
            }
            if (num2 / this.t <= 83.0 * num)
            {
                return 2;
            }
            if (num2 / this.t <= 124.0 * num)
            {
                return 3;
            }
            return 4;
        }

        public override EN1993CompressionBucklingCurve GetEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
        {
            EN1993CompressionBucklingCurve eN1993CompressionBucklingCurves = new EN1993CompressionBucklingCurve();
            if (this.Fabrication == SectionFabrication.ColdFormed)
            {
                eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.c;
                eN1993CompressionBucklingCurves.minorAxis = eN1993CompressionBucklingCurves.majorAxis;
                return eN1993CompressionBucklingCurves;
            }
            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a;
            if (grade == EN1993SteelGrade.S460)
            {
                eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a0;
            }
            eN1993CompressionBucklingCurves.minorAxis = eN1993CompressionBucklingCurves.majorAxis;
            return eN1993CompressionBucklingCurves;
        }

        public override EN1993LTBBucklingCurve GetEN1993LTBBucklingCurve()
        {
            return new EN1993LTBBucklingCurve
            {
                LTBBucklingCurve = EN1993LTBBucklingCurves.d
            };
        }

        public override Av GetEN1993ShearArea(double fy)
        {
            Av av = new Av();

            if (this.Fabrication == SectionFabrication.Rolled)
            {
                av.Avyy = this.A * this.h / (this.b + this.h);
                av.Avzz = this.A * this.b / (this.b + this.h);
                return av;
            }
            if (this.Fabrication == SectionFabrication.Welded)
            {
                av.Avyy = this.A - 2.0 * this.h * this.t;
                double num = 1.2;
                if (fy > 460.0)
                {
                    num = 1.0;
                }
                av.Avzz = num * 2.0 * this.h * this.t;
                return av;
            }
            throw new NotImplementedException();
        }
    }
}
