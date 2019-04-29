using System;
namespace SectionCatalogue.SectionProperties
{
    public class StructWeldedBoxProperties : SectionBase
    {
        public double h;
        public double b;
        public double tw;
        public double tf;
        public double outstand;
        public double a;
        public SectionFabrication fabrication = SectionFabrication.Welded;
        public StructWeldedBoxProperties(double depth, double width, double webThick, double FlangeThick, double outstandLength, double weldLeg, string denom)
        {
            this.h = depth;
            this.b = width;
            this.tw = webThick;
            this.tf = FlangeThick;
            this.outstand = outstandLength;
            this.a = weldLeg;
            this.Denomination = denom;
            this.A = 2.0 * this.h * this.tw + 2.0 * ((this.b - 2.0 * this.tw) * this.tf);
            this.Iyy = 2.0 * (this.tw * Math.Pow(this.h, 3.0) / 12.0 + (this.b - 2.0 * this.tw) * Math.Pow(this.tf, 3.0) / 12.0 + (this.b - 2.0 * this.tw) * this.tf * Math.Pow(this.h / 2.0 - (this.outstand + this.tf / 2.0), 2.0));
            this.Welyy = this.Iyy / (this.h / 2.0);
            this.Wplyy = (Math.Pow(this.h, 2.0) * this.tw / 4.0 + (this.b - 2.0 * this.tw) * this.tf * (this.h / 2.0 - (this.outstand + this.tf / 2.0))) * 2.0;
            this.iy = Math.Sqrt(this.Iyy / this.A);
            this.Izz = 2.0 * (this.tf * Math.Pow(this.b - 2.0 * this.tw, 3.0) / 12.0) + 2.0 * (this.h * Math.Pow(this.tw, 3.0) / 12.0 + this.h * this.tw * Math.Pow(this.b / 2.0 - this.tw / 2.0, 2.0));
            this.Welzz = this.Izz / (this.b / 2.0);
            this.Wplzz = 2.0 * (this.h * this.tw * (this.b / 2.0 - this.tw / 2.0) + (this.b - 2.0 * this.tw) * this.tf * (this.b - 2.0 * this.tw) / 4.0);
            this.iz = Math.Sqrt(this.Izz / this.A);
            this.It = 4.0 * Math.Pow((this.h - (2.0 * this.outstand + this.tf)) * (this.b - this.tw), 2.0) / (2.0 * ((this.h - (2.0 * this.outstand + this.tf)) / this.tw + (this.b - this.tw) / this.tf));
            this.Iw = 0.0;
            this.Symmetry = Symmetry.DoublySymmetric;
        }
        public override Av GetEN1993ShearArea(double fy)
        {
            Av av = new Av();
            double num = this.h - 2.0 * this.tf;
            if (this.fabrication == SectionFabrication.Welded)
            {
                av.Avyy = this.A - 2.0 * num * this.tw;
                double num2 = 1.2;
                if (fy > 460.0)
                {
                    num2 = 1.0;
                }
                av.Avzz = num2 * 2.0 * num * this.tw;
                return av;
            }
            throw new NotImplementedException();
        }
        public override int GetEN1993CompressionClass(double fy)
        {
            int val = 4;
            int val2 = 4;
            double num = base.CalcEpsilon(fy);
            double num2 = this.b - 2.0 * this.tw;
            double num3 = this.tf;
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
            num2 = this.h - this.outstand * 2.0 - this.tf * 2.0;
            num3 = this.tw;
            if (num2 / num3 <= 42.0 * num)
            {
                val2 = 3;
            }
            if (num2 / num3 <= 38.0 * num)
            {
                val2 = 2;
            }
            if (num2 / num3 <= 33.0 * num)
            {
                val2 = 1;
            }
            val = Math.Max(val, val2);
            num2 = this.outstand;
            num3 = this.tw;
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
        public override int GetEN1993FlexureClass(double fy)
        {
            int val = 4;
            int val2 = 4;
            double num = base.CalcEpsilon(fy);
            double num2 = this.h - 2.0 * this.outstand - 2.0 * this.tf;
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
            num2 = this.b - 2.0 * this.tw;
            num3 = this.tf;
            if (num2 / num3 <= 124.0 * num)
            {
                val2 = 3;
            }
            if (num2 / num3 <= 83.0 * num)
            {
                val2 = 2;
            }
            if (num2 / num3 <= 72.0 * num)
            {
                val2 = 1;
            }
            val = Math.Max(val, val2);
            num2 = this.outstand;
            num3 = this.tw;
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
        public override EN1993CompressionBucklingCurve GetEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
        {
            EN1993CompressionBucklingCurve eN1993CompressionBucklingCurves = new EN1993CompressionBucklingCurve();
            if (this.a > 0.5 * this.tf || this.b / this.tf < 30.0 || this.h / this.tw < 30.0)
            {
                eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.c;
                eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.c;
                return eN1993CompressionBucklingCurves;
            }
            eN1993CompressionBucklingCurves.majorAxis = EN1993CompressionBucklingCurves.b;
            eN1993CompressionBucklingCurves.minorAxis = EN1993CompressionBucklingCurves.b;
            return eN1993CompressionBucklingCurves;
        }
        public override EN1993LTBBucklingCurve GetEN1993LTBBucklingCurve()
        {
            return new EN1993LTBBucklingCurve
            {
                LTBBucklingCurve = EN1993LTBBucklingCurves.d
            };
        }
        public string genGWADef()
        {
            string text = "GEO P(m) ";
            double num = -this.b / 2.0;
            double num2 = this.h / 2.0;
            object obj = text;
            text = string.Concat(new object[]
            {
                obj,
                "M(",
                num,
                "|",
                num2,
                ") "
            });
            num = -this.b / 2.0 + this.tw;
            object obj2 = text;
            text = string.Concat(new object[]
            {
                obj2,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = this.h / 2.0 - this.outstand;
            object obj3 = text;
            text = string.Concat(new object[]
            {
                obj3,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = this.b / 2.0 - this.tw;
            object obj4 = text;
            text = string.Concat(new object[]
            {
                obj4,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = this.h / 2.0;
            object obj5 = text;
            text = string.Concat(new object[]
            {
                obj5,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = this.b / 2.0;
            object obj6 = text;
            text = string.Concat(new object[]
            {
                obj6,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = -this.h / 2.0;
            object obj7 = text;
            text = string.Concat(new object[]
            {
                obj7,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = this.b / 2.0 - this.tw;
            object obj8 = text;
            text = string.Concat(new object[]
            {
                obj8,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = -this.h / 2.0 + this.outstand;
            object obj9 = text;
            text = string.Concat(new object[]
            {
                obj9,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = -this.b / 2.0 + this.tw;
            object obj10 = text;
            text = string.Concat(new object[]
            {
                obj10,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = -this.h / 2.0;
            object obj11 = text;
            text = string.Concat(new object[]
            {
                obj11,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = -this.b / 2.0;
            object obj12 = text;
            text = string.Concat(new object[]
            {
                obj12,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = -this.b / 2.0 + this.tw;
            num2 = this.h / 2.0 - this.outstand - this.tf;
            object obj13 = text;
            text = string.Concat(new object[]
            {
                obj13,
                "M(",
                num,
                "|",
                num2,
                ") "
            });
            num = -num;
            object obj14 = text;
            text = string.Concat(new object[]
            {
                obj14,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num2 = -num2;
            object obj15 = text;
            text = string.Concat(new object[]
            {
                obj15,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
            num = -num;
            object obj16 = text;
            return string.Concat(new object[]
            {
                obj16,
                "L(",
                num,
                "|",
                num2,
                ") "
            });
        }
    }
}
