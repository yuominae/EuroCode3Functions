using System;
namespace SectionCatalogue.StructProperties
{
    /// <summary>
    /// I sections
    /// </summary>
    public class ISection : StructBaseProperties
    {
        public double h { get; set; }

        public double b { get; set; }

        public double tw { get; set; }

        public double tf { get; set; }

        public double r { get; set; }

        public SectionFabrication fabrication = SectionFabrication.WELDED;

        public override Symmetry Symmetry
        {
            get
            {
                return Symmetry.DOUBLYSYMMETRIC;
            }
        }

        public ISection(double Depth, double Width, double WebThick, double FlangeThick, double RootRadius, string Denomination)
        {
            this.Denomination = Denomination;
            this.h = Depth;
            this.b = Width;
            this.tw = WebThick;
            this.tf = FlangeThick;
            this.r = RootRadius;
            this.A = 2.0 * this.b * this.tf + this.tw * (this.h - 2.0 * this.tf);
            this.Iyy = (this.b * Math.Pow(this.h, 3.0) - (this.b - this.tw) * Math.Pow(this.h - 2.0 * this.tf, 3.0)) / 12.0;
            this.Welyy = this.Iyy / (this.h / 2.0);
            this.Wplyy = (this.b * this.tf * (this.h / 2.0 - this.tf / 2.0) + (this.h - 2.0 * this.tf) / 2.0 * this.tw * ((this.h - 2.0 * this.tf) / 4.0)) * 2.0;
            this.iy = Math.Sqrt(this.Iyy / this.A);
            this.Izz = (2.0 * this.tf * Math.Pow(this.b, 3.0) + (this.h - 2.0 * this.tf) * Math.Pow(this.tw, 3.0)) / 12.0;
            this.Welzz = this.Izz / (this.b / 2.0);
            this.Wplzz = (this.b / 2.0 * this.tf * this.b / 4.0 * 2.0 + (this.h - 2.0 * this.tf) * this.tw / 2.0 * this.tw / 4.0) * 2.0;
            this.iz = Math.Sqrt(this.Izz / this.A);
            this.It = (2.0 * this.b * Math.Pow(this.tf, 3.0) - (this.h - this.tf) * Math.Pow(this.tw, 3.0)) / 3.0;
            this.Iw = Math.Pow(this.h - this.tf, 2.0) * Math.Pow(this.b, 3.0) * this.tf / 24.0;
        }

        public ISection(string[] Properties)
        {
            this.Denomination = Properties[0];
            this.weight = double.Parse(Properties[1]);
            this.h = double.Parse(Properties[2]);
            this.b = double.Parse(Properties[3]);
            this.tw = double.Parse(Properties[4]);
            this.tf = double.Parse(Properties[5]);
            this.r = double.Parse(Properties[6]);
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

        public override int getEN1993CompressionClass(double fy)
        {
            int val = 4;
            double num = base.calcEpsilon(fy);
            double num2 = this.h - 2.0 * (this.tf + this.r);
            double num3 = this.tw;
            if (num2 / num3 <= 42.0 * num)
            {
                val = 3;
            }
            if (num2 / num3 <= 38.0 * num)
            {
                val = 2;
            }
            if (num2 / num3 <= 33.0 * num)
            {
                val = 1;
            }
            num2 = this.b / 2.0 - num3 / 2.0 - this.r;
            num3 = this.tf;
            if (num2 / num3 <= 9.0 * num)
            {
                return Math.Max(val, 1);
            }
            if (num2 / num3 <= 10.0 * num)
            {
                return Math.Max(val, 2);
            }
            if (num2 / num3 <= 14.0 * num)
            {
                return Math.Max(val, 3);
            }
            return 4;
        }
        public override int getEN1993FlexureClass(double fy)
        {
            int val = 4;
            double num = base.calcEpsilon(fy);
            double num2 = this.h - 2.0 * (this.tf + this.r);
            double num3 = this.tw;
            if (num2 / num3 <= 124.0 * num)
            {
                val = 3;
            }
            if (num2 / num3 <= 83.0 * num)
            {
                val = 2;
            }
            if (num2 / num3 <= 72.0 * num)
            {
                val = 1;
            }
            num2 = this.b / 2.0 - num3 / 2.0 - this.r;
            num3 = this.tf;
            if (num2 / num3 <= 9.0 * num)
            {
                return Math.Max(val, 1);
            }
            if (num2 / num3 <= 10.0 * num)
            {
                return Math.Max(val, 2);
            }
            if (num2 / num3 <= 14.0 * num)
            {
                return Math.Max(val, 3);
            }
            return 4;
        }
        public override Av getEN1993ShearArea(double fy)
        {
            Av av = new Av();
            double num = this.h - 2.0 * this.tf;
            if (this.fabrication == SectionFabrication.ROLLED)
            {
                av.Avyy = this.A - 2.0 * this.b * this.tf + (this.tw + 2.0 * this.r) * this.tf;
                double num2 = 1.2;
                if (fy > 460.0)
                {
                    num2 = 1.0;
                }
                if (av.Avyy < num2 * num * this.tw)
                {
                    av.Avyy = num2 * num * this.tw;
                }
                av.Avzz = this.A - num * this.tw;
                return av;
            }
            if (this.fabrication == SectionFabrication.WELDED)
            {
                double num3 = 1.2;
                if (fy > 460.0)
                {
                    num3 = 1.0;
                }
                if (av.Avyy < num3 * num * this.tw)
                {
                    av.Avyy = num3 * num * this.tw;
                }
                av.Avzz = this.A - num * this.tw;
                return av;
            }
            throw new NotImplementedException();
        }
        public override EN1993CompressionBucklingCurve getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
        {
            EN1993CompressionBucklingCurve eN1993CompressionBucklingCurves = new EN1993CompressionBucklingCurve();
            if (this.fabrication == SectionFabrication.ROLLED)
            {
                eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.d;
                eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.d;
                if (this.h / this.b > 1.2)
                {
                    if (this.tf * 1000.0 <= 40.0)
                    {
                        if (grade == EN1993SteelGrade.S460)
                        {
                            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a0;
                            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.a0;
                            return eN1993CompressionBucklingCurves;
                        }
                        eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a;
                        eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.b;
                        return eN1993CompressionBucklingCurves;
                    }
                    else
                    {
                        if (this.tf * 1000.0 > 100.0)
                        {
                            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.d;
                            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.d;
                            return eN1993CompressionBucklingCurves;
                        }
                        if (grade == EN1993SteelGrade.S460)
                        {
                            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a;
                            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.a;
                            return eN1993CompressionBucklingCurves;
                        }
                        eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.b;
                        eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.c;
                        return eN1993CompressionBucklingCurves;
                    }
                }
                else
                {
                    if (this.tf * 1000.0 <= 100.0)
                    {
                        if (grade == EN1993SteelGrade.S460)
                        {
                            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.a;
                            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.a;
                            return eN1993CompressionBucklingCurves;
                        }
                        eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.b;
                        eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.c;
                        return eN1993CompressionBucklingCurves;
                    }
                    else
                    {
                        if (grade == EN1993SteelGrade.S460)
                        {
                            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.c;
                            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.c;
                            return eN1993CompressionBucklingCurves;
                        }
                        eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.d;
                        eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.d;
                        return eN1993CompressionBucklingCurves;
                    }
                }
            }
            else
            {
                if (this.tf * 1000.0 <= 40.0)
                {
                    eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.b;
                    eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.c;
                    return eN1993CompressionBucklingCurves;
                }
                eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.c;
                eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.d;
                return eN1993CompressionBucklingCurves;
            }
        }
        public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
        {
            EN1993LTBBucklingCurve eN1993LTBBucklingCurve = new EN1993LTBBucklingCurve();
            if (this.fabrication == SectionFabrication.ROLLED)
            {
                if (this.h / this.b > 2.0)
                {
                    eN1993LTBBucklingCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.b;
                    return eN1993LTBBucklingCurve;
                }
                eN1993LTBBucklingCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.a;
                return eN1993LTBBucklingCurve;
            }
            else
            {
                if (this.h / this.b > 2.0)
                {
                    eN1993LTBBucklingCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                    return eN1993LTBBucklingCurve;
                }
                eN1993LTBBucklingCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.c;
                return eN1993LTBBucklingCurve;
            }
        }
    }
}
