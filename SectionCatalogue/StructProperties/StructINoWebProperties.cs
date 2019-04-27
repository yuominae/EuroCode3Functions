using System;
namespace SectionCatalogue.StructProperties
{
    public class StructINoWebProperties : StructBaseProperties
    {
        public double h;
        public double b;
        public double tw;
        public double tf;
        public double r;
        public double Afull;
        public SectionFabrication fabrication = SectionFabrication.WELDED;

        public StructINoWebProperties(double depth, double width, double webThick, double flangeThick, double rootRadius, string denom)
        {
            this.Denomination = denom;
            this.h = depth;
            this.b = width;
            this.tw = webThick;
            this.tf = flangeThick;
            this.r = rootRadius;
            this.A = 2.0 * this.b * this.tf;
            this.Iyy = this.b * (Math.Pow(this.h, 3.0) - Math.Pow(this.h - 2.0 * this.tf, 3.0)) / 12.0;
            this.Welyy = this.Iyy / (this.h / 2.0);
            this.Wplyy = 2.0 * (this.b * this.tf * (this.h / 2.0 - this.tf / 2.0));
            this.iy = Math.Sqrt(this.Iyy / this.A);
            this.Izz = Math.Pow(this.b, 3.0) * (this.h - (this.h - 2.0 * this.tf)) / 12.0;
            this.Welzz = this.Izz / (this.b / 2.0);
            this.Wplzz = Math.Pow(this.b, 2.0) * this.tf / 2.0;
            this.iz = Math.Sqrt(this.Izz / this.A);
            this.It = (2.0 * this.b * Math.Pow(this.tf, 3.0) - (this.h - this.tf) * Math.Pow(this.tw, 3.0)) / 3.0;
            this.Iw = Math.Pow(this.h - this.tf, 2.0) * Math.Pow(this.b, 3.0) * this.tf / 24.0;
            this.Symmetry = Symmetry.DOUBLYSYMMETRIC;
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
            if (this.fabrication == SectionFabrication.ROLLED)
            {
                av.Avyy = 0.0;
                av.Avzz = this.A;
                return av;
            }
            if (this.fabrication == SectionFabrication.WELDED)
            {
                av.Avyy = 0.0;
                av.Avzz = this.A;
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
