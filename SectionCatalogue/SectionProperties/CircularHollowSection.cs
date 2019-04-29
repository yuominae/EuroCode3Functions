using System;
namespace SectionCatalogue.SectionProperties
{
    public class CircularHollowSection : SectionBase
    {
        public double d { get; set; }

        public double t { get; set; }

        public SectionFabrication fabrication = SectionFabrication.HotFinished;

        public override Symmetry Symmetry
        {
            get
            {
                return Symmetry.DoublySymmetric;
            }
        }

        public CircularHollowSection(double Diametre, double Thickness, string Denomination)
        {
            this.Denomination = Denomination;
            this.d = Diametre;
            this.t = Thickness;
            this.A = Math.PI * (Math.Pow(this.d, 2.0) - Math.Pow(this.d - 2.0 * this.t, 2.0)) / 4.0;
            this.Iyy = Math.PI * (Math.Pow(this.d, 4.0) - Math.Pow(this.d - 2.0 * this.t, 4.0)) / 64.0;
            this.Welyy = this.Iyy / (this.d / 2.0);
            this.Wplyy = (Math.Pow(this.d, 3.0) - Math.Pow(this.d - this.t, 3.0)) / 6.0 - Math.Pow(this.d, 3.0) / 6.0 * (1.0 - Math.Pow(1.0 - 2.0 * this.t / this.d, 3.0));
            this.iy = Math.Sqrt(this.Iyy / this.A);
            this.Izz = this.Iyy;
            this.Welzz = this.Welyy;
            this.Wplzz = (Math.Pow(this.d, 3.0) - Math.Pow(this.d - this.t, 3.0)) / 6.0 - Math.Pow(this.d, 3.0) / 6.0 * (1.0 - Math.Pow(1.0 - 2.0 * this.t / this.d, 3.0));
            this.iz = Math.Sqrt(this.Izz / this.A);
            this.It = 2.0 * this.Iyy;
            this.Iw = 0.0;
        }

        public CircularHollowSection(string[] Properties)
        {
            this.Denomination = Properties[0];
            this.Weight = double.Parse(Properties[1]);
            this.d = double.Parse(Properties[2]);
            this.t = double.Parse(Properties[3]);
            this.A = double.Parse(Properties[4]);
            this.Iyy = double.Parse(Properties[5]);
            this.Welyy = double.Parse(Properties[6]);
            this.Wplyy = double.Parse(Properties[7]);
            this.iy = double.Parse(Properties[8]);
            this.Izz = this.Iyy;
            this.Welzz = this.Welyy;
            this.Wplzz = this.Wplyy;
            this.iz = this.iy;
            this.It = double.Parse(Properties[9]);
            this.Iw = double.Parse(Properties[10]);
        }

        public override int GetEN1993CompressionClass(double fy)
        {
            double num = base.CalcEpsilon(fy);
            if (this.d / this.t <= 50.0 * num * num)
            {
                return 1;
            }
            if (this.d / this.t <= 70.0 * num * num)
            {
                return 2;
            }
            if (this.d / this.t <= 90.0 * num * num)
            {
                return 3;
            }
            return 4;
        }
        public override int GetEN1993FlexureClass(double fy)
        {
            return this.GetEN1993CompressionClass(fy);
        }
        public override EN1993CompressionBucklingCurve GetEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
        {
            EN1993CompressionBucklingCurve eN1993CompressionBucklingCurves = new EN1993CompressionBucklingCurve();
            if (this.fabrication == SectionFabrication.ColdFormed)
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
            av.Avyy = 2.0 * this.A / 3.1415926535897931;
            av.Avzz = av.Avyy;
            return av;
        }
    }
}
