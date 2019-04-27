using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SectionCatalogue
{
   

	public enum SectionFabrication { ROLLED, WELDED, HOTFINISHED, COLDFORMED };
	public enum EN1993CompressionBucklingCurve { a0, a, b, c, d };
    public enum EN1993LTBBucklingCurves {a, b, c, d };
	public enum EN1993SteelGrade { S235, S275, S355, S420, S450, S460 };
    public enum Symmetry {ASYMMETRIC, SINGLYSYMMETRIC, DOUBLYSYMMETRIC};

    public class Av
    {
        public double Avyy = 0;
        public double Avzz = 0;
    }

	public class EN1993CompressionBucklingCurves
	{
		public EN1993CompressionBucklingCurve majorAxis = EN1993CompressionBucklingCurve.d;
		public EN1993CompressionBucklingCurve minorAxis = EN1993CompressionBucklingCurve.d;
	}

    public class EN1993LTBBucklingCurve
    {
        public EN1993LTBBucklingCurves LTBBucklingCurve = EN1993LTBBucklingCurves.d;
    }

	public class NotCodedException : Exception
	{
		public NotCodedException(string message)
			: base(message)
		{

		}
	}

	 namespace StructProperties
    {
         //
         //Base properties
		public class StructBaseProperties
		{
			internal string strTitle;
			public double weight, A, Iyy, Welyy, Wplyy, iy, Avz, Izz, Welzz, Wplzz, iz, ss, It, Iw;
            public Symmetry SecSymmetry = Symmetry.ASYMMETRIC;
			public string Title
			{
				get
				{
					return strTitle;
				}
				set
				{
					strTitle = value;
				}
			}
			public virtual EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
			{
				throw new NotCodedException("Class not Coded for this section!");
			}
            public virtual EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                throw new NotCodedException("Class not Coded for this section!");
            }
			public virtual int getEN1993CompressionClass(double fy)
			{
				throw new NotCodedException("Class not Coded for this section!");
			}
			public virtual int getEN1993FlexureClass(double fy)
			{
				throw new NotCodedException("Class not Coded for this section!");
			}
            public virtual Av getEN1993ShearArea(double fy)
            {
                throw new NotCodedException("Class not Coded for this section!");
            }
			protected internal double calcEpsilon(double fy)
			{
				return Math.Sqrt(235 / fy);
			}

		}
         //
         //
		public class StructRectProperties : StructBaseProperties
		{
			public double h, b;

			public StructRectProperties(double depth, double width, string denom)
			{
				strTitle = denom;
				h = depth;
				b = width;
			}

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }


		}

		public class StructCircProperties : StructBaseProperties
		{
			public double d;
			public StructCircProperties(double Diameter, string denom)
			{
				strTitle = denom;
				d = Diameter;
			}

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }


		}

		public class StructRHSProperties : StructBaseProperties // todo consider different flange/web thick
		{
			public double h, b, t;

            public SectionFabrication fabrication = SectionFabrication.ROLLED;
                
			public StructRHSProperties(double depth, double width, double thickness, string denom)
			{
				h = depth;
				b = width;
				t = thickness;
                A = h * b - (h - 2 * t) * (b - 2 * t);
                Iyy = (b * Math.Pow(h, 3) - (b - 2 * t) * Math.Pow((h - 2 * t), 3)) / 12;
                Welyy = Iyy / (h / 2);
                Wplyy = 2 * (b * t * (h / 2 - t / 2) + t * Math.Pow(h / 2 - t, 2)); ;
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = (h * Math.Pow(b, 3) - (h - 2 * t) * Math.Pow((b - 2 * t), 3)) / 12;
                Welzz = Izz / (b / 2);
                Wplzz = 2 * (h * t * (b / 2 - t / 2) + t * Math.Pow(b / 2 - t, 2));
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = 4 * Math.Pow((b - t) * (h - t), 2) * t / (2 * (b + h));
                Iw = 0;
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
			}

			public StructRHSProperties(string def)
			{
				strTitle = def;
				string[] arrDef = def.Substring(3, def.Length - 3).Split("x".ToCharArray());
				h = double.Parse(arrDef[0]) / 1000;
				b = double.Parse(arrDef[1]) / 1000;
				t = double.Parse(arrDef[2]) / 1000;
                A = h * b - (h - 2 * t) * (b - 2 * t);
                Iyy = (b * Math.Pow(h, 3) - (b - 2 * t) * Math.Pow((h - 2 * t), 3)) / 12;
                Welyy = Iyy / (h / 2);
                Wplyy = 2 * (b * t * (h / 2 - t / 2) + t * Math.Pow(h / 2 - t, 2)); ;
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = (h * Math.Pow(b, 3) - (h - 2 * t) * Math.Pow((b - 2 * t), 3)) / 12;
                Welzz = Izz / (b / 2);
                Wplzz = 2 * (h * t * (b / 2 - t / 2) + t * Math.Pow(b / 2 - t, 2));
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = 4 * Math.Pow((b - t) * (h - t), 2) * t / (2 * (b + h));
                Iw = 0;
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
			}

            public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
            {
                double epsilon = calcEpsilon(fy);
                double c = h - 2 * t;
                if (b - 2 * t > c)
                    c = b - 2 * t;
                if (c / t <= 33 * epsilon)
                    return  1;
                if (c / t <= 38 * epsilon)
                    return 2;
                if (c / t <= 42 * epsilon)
                    return 3;
                return 4;
            }

            public override int getEN1993FlexureClass(double fy)
            {
                double epsilon = calcEpsilon(fy);
                double c = h - 2 * t;
                if (b - 2 * t > c)
                    c = b - 2 * t;
                if (c / t <= 72 * epsilon)
                    return 1;
                if (c / t <= 83 * epsilon)
                    return 2;
                if (c / t <= 124 * epsilon)
                    return 3;
                return 4;
            }

            public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
            {

                EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();

                if (fabrication == SectionFabrication.COLDFORMED)
                {
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                    buckCurves.minorAxis = buckCurves.majorAxis;
                    return buckCurves;
                }

                buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                if (grade == EN1993SteelGrade.S460)
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.a0;
                buckCurves.minorAxis = buckCurves.majorAxis;
                return buckCurves;

            }

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }

            public override Av getEN1993ShearArea(double fy)
            {
                Av ShearArea = new Av();
                if (fabrication == SectionFabrication.ROLLED)
                {
                    ShearArea.Avyy = A * h / (b + h);
                    ShearArea.Avzz = A * b / (b + h);
                    return ShearArea;
                }
                if (fabrication == SectionFabrication.WELDED)
                {
                    ShearArea.Avyy = A - 2 * h * t;
                    double etah = 1.2; //see EN1993-1-5 cl.5.1
                    if (fy > 460)
                        etah = 1.0;
                    ShearArea.Avzz = etah * 2 * h * t;
                    return ShearArea;
                }
                throw new NotCodedException("Case not coded");
            }

		}

		public class StructCHSProperties : StructBaseProperties
		{
			public double d, t;

            public SectionFabrication fabrication = SectionFabrication.HOTFINISHED;

			public StructCHSProperties(double diam, double thickness, string denom)
			{
				strTitle = denom;
				d = diam;
				t = thickness;
                A = (Math.PI * (Math.Pow(d, 2) - Math.Pow(d - 2 * t, 2))) / 4;
                Iyy = (Math.PI * (Math.Pow(d, 4) - Math.Pow(d - 2 * t, 4))) / 64;
                Welyy = Iyy / (d / 2);
                Wplyy = (Math.Pow(d, 3) - Math.Pow(d - t, 3)) / 6 - Math.Pow(d, 3) / 6 * (1 - Math.Pow(1 - 2 * t / d, 3));
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = Iyy;
                Welzz = Welyy;
                Wplzz = (Math.Pow(d, 3) - Math.Pow(d - t, 3)) / 6 - Math.Pow(d, 3) / 6 * (1 - Math.Pow(1 - 2 * t / d, 3));
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = 2 * Iyy;
                Iw = 0;
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
			}
			public StructCHSProperties(string def)
			{
				strTitle = def;
				string[] arrDef = def.Substring(3, def.Length - 3).Split("x".ToCharArray());
				d = double.Parse(arrDef[0]) / 1000;
				t = double.Parse(arrDef[1]) / 1000;
                A = (Math.PI * (Math.Pow(d, 2) - Math.Pow(d - 2 * t, 2))) / 4;
                Iyy = (Math.PI * (Math.Pow(d, 4) - Math.Pow(d - 2 * t, 4))) / 64;
                Welyy = Iyy / (d / 2);
                Wplyy = Math.Pow(d, 3) / 6 * (1 - Math.Pow(1 - 2 * t / d, 3));
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = Iyy;
                Welzz = Welyy;
                Wplzz = Math.Pow(d, 3) / 6 * (1 - Math.Pow(1 - 2 * t / d, 3));
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = 2 * Iyy;
                Iw = 0;
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
			}

			public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
			{
				double epsilon = calcEpsilon(fy);
				if (d / t <= 50 * epsilon * epsilon)
					return 1;
				if (d / t <= 70 * epsilon * epsilon)
					return 2;
				if (d / t <= 90 * epsilon * epsilon)
					return 3;
				return 4;
			}
			public override int getEN1993FlexureClass(double fy)
			{
				return getEN1993CompressionClass(fy);
			}

            public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
            {

                EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();

                if (fabrication == SectionFabrication.COLDFORMED)
                {
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                    buckCurves.minorAxis = buckCurves.majorAxis;
                    return buckCurves;
                }

                buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                if (grade == EN1993SteelGrade.S460)
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.a0;
                buckCurves.minorAxis = buckCurves.majorAxis;
                return buckCurves;

            }

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }

            public override Av getEN1993ShearArea(double fy)
            {
                Av ShearArea = new Av();
                ShearArea.Avyy = 2 * A / Math.PI;
                ShearArea.Avzz = ShearArea.Avyy;
                return ShearArea;
            }

		}

		public class StructChannelProperties : StructBaseProperties
		{
			public double h, b, tw, tf;

			public StructChannelProperties(double depth, double width, double webThick, double flangeThick, string denom)
			{
				strTitle = denom;
				h = depth;
				b = width;
				tw = webThick;
				tf = flangeThick;
			}
			public StructChannelProperties(string def) //todo  create properly
			{
				strTitle = def;
				string[] arrDef = def.Substring(2, def.Length - 2).Split("x".ToCharArray());
				h = double.Parse(arrDef[0]) / 1000;
				b = double.Parse(arrDef[1]) / 1000;
				tw = double.Parse(arrDef[2]) / 1000;
				tf = double.Parse(arrDef[3]) / 1000;
			}

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }

		}

		public class StructAngleProperties : StructBaseProperties
		{
			public double h, b, tw, tf;

			public StructAngleProperties(double depth, double width, double webThick, double flangeThick, string denom)
			{
				h = depth;
				b = width;
				tw = webThick;
				tf = flangeThick;
				strTitle = denom;
			}

			public StructAngleProperties(string def)
			{
				strTitle = def;
				string[] arrDef = def.Substring(2, def.Length - 2).Split("x".ToCharArray());
				h = b = double.Parse(arrDef[0]) / 1000;
				tw = double.Parse(arrDef[2]) / 1000;
				tf = tw;
			}

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }

		}

        public class StructIProperties : StructBaseProperties
        {
            public double h, b, tw, tf, r;
            public SectionFabrication fabrication = SectionFabrication.WELDED;

            public StructIProperties(double depth, double width, double webThick, double flangeThick, double rootRadius, string denom)
            {
                strTitle = denom;
                h = depth;
                b = width;
                tw = webThick;
                tf = flangeThick;
                r = rootRadius;
                A = 2 * b * tf + tw * (h - 2 * tf);
                Iyy = (b * Math.Pow(h, 3) - (b - tw) * Math.Pow((h - 2 * tf), 3)) / 12;
                Welyy = Iyy / (h / 2);
                Wplyy = (b * tf * (h / 2 - tf / 2) + ((h - 2 * tf) / 2) * tw * ((h - 2 * tf) / 4)) * 2;
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = (2 * tf * Math.Pow(b, 3) + (h - 2 * tf) * Math.Pow(tw, 3)) / 12;
                Welzz = Izz / (b / 2);
                Wplzz = ((b / 2 * tf * b / 4) * 2 + ((h - 2 * tf) * tw / 2 * tw / 4)) * 2;
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = (2 * b * Math.Pow(tf, 3) - (h - tf) * Math.Pow(tw, 3)) / 3;
                Iw = (Math.Pow((h - tf),2) * Math.Pow(b,3) * tf) / 24;
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
            }

            public StructIProperties(string[] strDef)
            {
                strTitle = strDef[0];
                weight = double.Parse(strDef[1]);
                h = double.Parse(strDef[2]);
                b = double.Parse(strDef[3]);
                tw = double.Parse(strDef[4]);
                tf = double.Parse(strDef[5]);
                r = double.Parse(strDef[6]);
                A = double.Parse(strDef[7]);
                Iyy = double.Parse(strDef[8]);
                Welyy = double.Parse(strDef[9]);
                Wplyy = double.Parse(strDef[10]);
                iy = double.Parse(strDef[11]);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = double.Parse(strDef[13]);
                Welzz = double.Parse(strDef[14]);
                Wplzz = double.Parse(strDef[15]);
                iz = double.Parse(strDef[16]);
                ss = double.Parse(strDef[17]);
                It = double.Parse(strDef[18]);
                Iw = double.Parse(strDef[19]);
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
            }

            public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
            {
                int sectclass = 4;
                double epsilon = calcEpsilon(fy);
                double c = h - 2 * (tf + r);
                double t = tw;

                if (c / t <= 42 * epsilon)  //don't reverse order of check
                    sectclass = 3;
                if (c / t <= 38 * epsilon)
                    sectclass = 2;
                if (c / t <= 33 * epsilon)
                    sectclass = 1;

                c = b / 2 - t / 2 - r;
                t = tf;

                if (c / t <= 9 * epsilon)
                    return Math.Max(sectclass, 1);
                if (c / t <= 10 * epsilon)
                    return Math.Max(sectclass, 2);
                if (c / t <= 14 * epsilon)
                    return Math.Max(sectclass, 3);

                return 4;
            }

            public override int getEN1993FlexureClass(double fy)  //table 5.2 of EN 1993-1-1
            {
                int sectclass = 4;
                double epsilon = calcEpsilon(fy);
                double c = h - 2 * (tf + r);
                double t = tw;


                if (c / t <= 124 * epsilon) //don't reverse order of check
                    sectclass = 3;
                if (c / t <= 83 * epsilon)
                    sectclass = 2;
                if (c / t <= 72 * epsilon)
                    sectclass = 1;



                c = b / 2 - t / 2 - r;
                t = tf;

                if (c / t <= 9 * epsilon)
                    return Math.Max(sectclass, 1);
                if (c / t <= 10 * epsilon)
                    return Math.Max(sectclass, 2);
                if (c / t <= 14 * epsilon)
                    return Math.Max(sectclass, 3);

                return 4;
            }

            public override Av getEN1993ShearArea(double fy)
            {
                Av ShearArea = new Av();
                double hw = h - 2 * tf;
                if (fabrication == SectionFabrication.ROLLED)
                {
                    ShearArea.Avyy = A - 2 * b * tf + (tw + 2 * r) * tf;
                    double etah = 1.2;
                    if (fy > 460)
                        etah = 1.0;
                    if (ShearArea.Avyy < etah * hw * tw)
                        ShearArea.Avyy = etah * hw * tw;
                    ShearArea.Avzz = A - hw * tw;
                    return ShearArea;
                }
                else if (fabrication == SectionFabrication.WELDED)
                {
                    double etah = 1.2;
                    if (fy > 460)
                        etah = 1.0;
                    if (ShearArea.Avyy < etah * hw * tw)
                        ShearArea.Avyy = etah * hw * tw;
                    ShearArea.Avzz = A - hw * tw;
                    return ShearArea;
                }
                else
                {
                    throw new NotCodedException("Case not coded");
                }
            }

            public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
            {
                EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();
                if (fabrication == SectionFabrication.ROLLED)
                {
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                    buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;

                    if (h / b > 1.2)
                    {
                        if (tf * 1e3 <= 40)
                        {
                            if (grade == EN1993SteelGrade.S460)
                            {
                                buckCurves.majorAxis = EN1993CompressionBucklingCurve.a0;
                                buckCurves.minorAxis = EN1993CompressionBucklingCurve.a0;
                                return buckCurves;
                            }
                            buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                            buckCurves.minorAxis = EN1993CompressionBucklingCurve.b;
                            return buckCurves;
                        }
                        else
                        {
                            if (tf * 1e3 > 100)
                            {
                                buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                                buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                                return buckCurves;
                            }
                            if (grade == EN1993SteelGrade.S460)
                            {
                                buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                                buckCurves.minorAxis = EN1993CompressionBucklingCurve.a;
                                return buckCurves;
                            }
                            buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                            buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                            return buckCurves;
                        }
                    }
                    else
                    {
                        if (tf * 1e3 <= 100)
                        {
                            if (grade == EN1993SteelGrade.S460)
                            {
                                buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                                buckCurves.minorAxis = EN1993CompressionBucklingCurve.a;
                                return buckCurves;
                            }
                            buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                            buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                            return buckCurves;
                        }
                        else
                        {
                            if (grade == EN1993SteelGrade.S460)
                            {
                                buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                                buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                                return buckCurves;
                            }
                            buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                            buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                            return buckCurves;
                        }
                    }
                }
                //Welded sections
                if (tf * 1e3 <= 40)
                {
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                    buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                    return buckCurves;
                }
                else
                {
                    buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                    buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                    return buckCurves;
                }
            }

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                if (fabrication == SectionFabrication.ROLLED)
                {
                    if (h / b > 2)
                    {
                        LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.b;
                        return LTBbuckCurve;
                    }
                    LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.a;
                    return LTBbuckCurve;
                }
                else
                {
                    if (h / b > 2)
                    {
                        LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                        return LTBbuckCurve;
                    }
                    LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.c;
                    return LTBbuckCurve;
                }
            }

        }

        public class StructINoWebProperties : StructBaseProperties
         {
             public double h, b, tw, tf, r, Afull;
             public SectionFabrication fabrication = SectionFabrication.WELDED;

             public StructINoWebProperties(double depth, double width, double webThick, double flangeThick, double rootRadius, string denom)
             {
                 strTitle = denom;
                 h = depth;
                 b = width;
                 tw = webThick;
                 tf = flangeThick;
                 r = rootRadius;
                 A = 2 * b * tf;
                 Iyy = (b * (Math.Pow(h, 3) - Math.Pow(h - 2 * tf, 3))) / 12;
                 Welyy = Iyy / (h / 2);
                 Wplyy = 2 * (b * tf * (h / 2 - tf / 2));
                 iy = Math.Sqrt(Iyy / A);
                 Izz = (Math.Pow(b, 3) * (h - (h - 2 * tf))) / 12;
                 Welzz = Izz / (b / 2);
                 Wplzz = Math.Pow(b, 2) * tf / 2;
                 iz = Math.Sqrt(Izz / A);
                 ss = 0;
                 It = (2 * b * Math.Pow(tf, 3) - (h - tf) * Math.Pow(tw, 3)) / 3;
                 Iw = (Math.Pow((h - tf), 2) * Math.Pow(b, 3) * tf) / 24;
                 SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
             }

             public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
             {
                 int sectclass = 4;
                 double epsilon = calcEpsilon(fy);
                 double c = h - 2 * (tf + r);
                 double t = tw;

                 if (c / t <= 42 * epsilon)  //don't reverse order of check
                     sectclass = 3;
                 if (c / t <= 38 * epsilon)
                     sectclass = 2;
                 if (c / t <= 33 * epsilon)
                     sectclass = 1;

                 c = b / 2 - t / 2 - r;
                 t = tf;

                 if (c / t <= 9 * epsilon)
                     return Math.Max(sectclass, 1);
                 if (c / t <= 10 * epsilon)
                     return Math.Max(sectclass, 2);
                 if (c / t <= 14 * epsilon)
                     return Math.Max(sectclass, 3);

                 return 4;
             }

             public override int getEN1993FlexureClass(double fy)  //table 5.2 of EN 1993-1-1
             {
                 int sectclass = 4;
                 double epsilon = calcEpsilon(fy);
                 double c = h - 2 * (tf + r);
                 double t = tw;


                 if (c / t <= 124 * epsilon) //don't reverse order of check
                     sectclass = 3;
                 if (c / t <= 83 * epsilon)
                     sectclass = 2;
                 if (c / t <= 72 * epsilon)
                     sectclass = 1;



                 c = b / 2 - t / 2 - r;
                 t = tf;

                 if (c / t <= 9 * epsilon)
                     return Math.Max(sectclass, 1);
                 if (c / t <= 10 * epsilon)
                     return Math.Max(sectclass, 2);
                 if (c / t <= 14 * epsilon)
                     return Math.Max(sectclass, 3);

                 return 4;
             }

             public override Av getEN1993ShearArea(double fy)
             {
                 Av ShearArea = new Av();
                 double hw = h - 2 * tf;
                 if (fabrication == SectionFabrication.ROLLED)
                 {
                     ShearArea.Avyy = 0;
                     ShearArea.Avzz = A;
                     return ShearArea;
                 }
                 else if (fabrication == SectionFabrication.WELDED)
                 {
                     ShearArea.Avyy = 0;
                     ShearArea.Avzz = A;
                     return ShearArea;
                 }
                 else
                 {
                     throw new NotCodedException("Case not coded");
                 }
             }

             public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
             {
                 EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();
                 if (fabrication == SectionFabrication.ROLLED)
                 {
                     buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                     buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;

                     if (h / b > 1.2)
                     {
                         if (tf * 1e3 <= 40)
                         {
                             if (grade == EN1993SteelGrade.S460)
                             {
                                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.a0;
                                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.a0;
                                 return buckCurves;
                             }
                             buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                             buckCurves.minorAxis = EN1993CompressionBucklingCurve.b;
                             return buckCurves;
                         }
                         else
                         {
                             if (tf * 1e3 > 100)
                             {
                                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                                 return buckCurves;
                             }
                             if (grade == EN1993SteelGrade.S460)
                             {
                                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.a;
                                 return buckCurves;
                             }
                             buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                             buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                             return buckCurves;
                         }
                     }
                     else
                     {
                         if (tf * 1e3 <= 100)
                         {
                             if (grade == EN1993SteelGrade.S460)
                             {
                                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.a;
                                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.a;
                                 return buckCurves;
                             }
                             buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                             buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                             return buckCurves;
                         }
                         else
                         {
                             if (grade == EN1993SteelGrade.S460)
                             {
                                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                                 return buckCurves;
                             }
                             buckCurves.majorAxis = EN1993CompressionBucklingCurve.d;
                             buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                             return buckCurves;
                         }
                     }
                 }
                 //Welded sections
                 if (tf * 1e3 <= 40)
                 {
                     buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                     buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                     return buckCurves;
                 }
                 else
                 {
                     buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                     buckCurves.minorAxis = EN1993CompressionBucklingCurve.d;
                     return buckCurves;
                 }
             }

             public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
             {
                 EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                 if (fabrication == SectionFabrication.ROLLED)
                 {
                     if (h / b > 2)
                     {
                         LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.b;
                         return LTBbuckCurve;
                     }
                     LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.a;
                     return LTBbuckCurve;
                 }
                 else
                 {
                     if (h / b > 2)
                     {
                         LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                         return LTBbuckCurve;
                     }
                     LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.c;
                     return LTBbuckCurve;
                 }
             }

         }

		public class StructWeldedBoxProperties : StructBaseProperties
		{
			public double h, b, tw, tf, outstand, a;
            public SectionFabrication fabrication = SectionFabrication.WELDED;

			public StructWeldedBoxProperties(double depth, double width, double webThick, double FlangeThick, double outstandLength, double weldLeg, string denom)
			{
                h = depth;
                b = width;
                tw = webThick;
                tf = FlangeThick;
                outstand = outstandLength;
                a = weldLeg;
                strTitle = denom;
                A = 2 * h * tw + 2 * ((b - 2 * tw) * tf);
                Iyy = 2 * (tw * Math.Pow(h, 3) / 12 + (b - 2 * tw) * Math.Pow(tf, 3) / 12 + (b - 2 * tw) * tf * Math.Pow(h / 2 - (outstand + tf / 2), 2));
                Welyy = Iyy / (h / 2);
                Wplyy = (Math.Pow(h,2) * tw / 4 + (b - 2 * tw) * tf * (h / 2 - (outstand + tf / 2))) * 2;
                iy = Math.Sqrt(Iyy / A);
                //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                Izz = 2 * (tf * Math.Pow((b - 2 * tw), 3) / 12) + 2 * (h * Math.Pow(tw, 3) / 12 + h * tw * Math.Pow((b / 2 - tw / 2),2));
                Welzz = Izz / (b / 2);
                Wplzz = 2 * (h * tw * (b / 2 - tw / 2) + (b - 2 * tw) * tf * (b - 2 * tw) / 4);
                iz = Math.Sqrt(Izz / A);
                ss = 0;
                It = 4 * Math.Pow(((h - (2 * outstand + tf)) * (b - tw)), 2) / (2 * ((h - (2 * outstand + tf)) / tw + (b - tw) / tf));
                Iw = 0; //Seems that that warping constant is usually taken as 0 for box sections
                SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
			}

            public override Av getEN1993ShearArea(double fy)
            {
                Av ShearArea = new Av();
                double hw = h - 2 * tf;
                if (fabrication == SectionFabrication.WELDED)
                {
                    ShearArea.Avyy = A - 2 * hw * tw;
                    double etah = 1.2; //see EN1993-1-5 cl.5.1
                    if (fy > 460)
                        etah = 1.0;
                    ShearArea.Avzz = etah * 2 * hw * tw;
                    return ShearArea;
                }
                throw new NotCodedException("Case not coded");
            }

			public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
			{
                int sectclass = 4, sectclass2 = 4;
				double epsilon = calcEpsilon(fy);

                //Flange
				double c = b - 2 * tw;
				double t = tf;

				if (c / t <=  42 * epsilon)  //don't reverse order of check
					sectclass = 3;
                if (c / t <= 38 * epsilon)
					sectclass = 2;
                if (c / t <=  33 * epsilon)
					sectclass = 1;

                //Inner web
				c = h - outstand * 2 - tf * 2;
				t = tw;

                if (c / t <=  42 * epsilon) //don't reverse order of check
					sectclass2 = 3;
                if (c / t <=  38 * epsilon)
					sectclass2 = 2;
                if (c / t <=  33 * epsilon)
					sectclass2 = 1;

                sectclass = Math.Max(sectclass, sectclass2);

                //Outstand
				c = outstand;
				t = tw;

				if (c / t <= 9 * epsilon)
					return Math.Max(sectclass, 1);
				if (c / t <= 10 * epsilon)
					return Math.Max(sectclass, 2);
				if (c / t <= 14 * epsilon)
					return Math.Max(sectclass, 3);

				return 4;
			}

			public override int getEN1993FlexureClass(double fy)  //table 5.2 of EN 1993-1-1
			{
				int sectclass = 4;
                int sectclass2 = 4;
				double epsilon = calcEpsilon(fy);

                //Inner web
                double c = h - 2 * outstand - 2 * tf;
				double t = tw;

				if (c / t <= 124 * epsilon) //don't reverse order of check
					sectclass = 3;
				if (c / t <= 83 * epsilon)
					sectclass = 2;
				if (c / t <= 72 * epsilon)
					sectclass = 1;

                //Flange
                c = b - 2 * tw;
				t = tf;

				if (c / t <= 124 * epsilon) //don't reverse order of check
					sectclass2 = 3;
				if (c / t <= 83 * epsilon)
					sectclass2 = 2;
				if (c / t <= 72 * epsilon)
					sectclass2 = 1;

                sectclass = Math.Max(sectclass, sectclass2);

                //Outstant
				c = outstand;
				t = tw;

				if (c / t <= 9 * epsilon)
					return Math.Max(sectclass, 1);
				if (c / t <= 10 * epsilon)
					return Math.Max(sectclass, 2);
				if (c / t <= 14 * epsilon)
					return Math.Max(sectclass, 3);

				return 4;
			}

            public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade) //table 6.2 of EN 1993-1-1
			{
				EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();
				if (a > 0.5 * tf || b / tf < 30 || h / tw < 30)
				{

					buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
					buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
					return buckCurves;
				}

				buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
				buckCurves.minorAxis = EN1993CompressionBucklingCurve.b;
				return buckCurves;

			}

            public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
            {
                EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                return LTBbuckCurve;
            }

			public string genGWADef()
			{
				string gwaDef = "GEO P(m) ";
				double x = -b / 2, y = h / 2;
				gwaDef += "M(" + x + "|" + y + ") ";
				x = -b / 2 + tw;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = h / 2 - outstand;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = b / 2 - tw;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = h / 2;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = b / 2;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = -h / 2;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = b / 2 - tw;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = -h / 2 + outstand;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = -b / 2 + tw;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = -h / 2;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = -b / 2;
				gwaDef += "L(" + x + "|" + y + ") ";

				x = -b / 2 + tw;
				y = h / 2 - outstand - tf;
				gwaDef += "M(" + x + "|" + y + ") ";
				x = -x;
				gwaDef += "L(" + x + "|" + y + ") ";
				y = -y;
				gwaDef += "L(" + x + "|" + y + ") ";
				x = -x;
				gwaDef += "L(" + x + "|" + y + ") ";

				return gwaDef;
			}
		}

        public class StructWeldedBoxNoWebsProperties : StructBaseProperties
         {
             public double h, b, tw, tf, outstand, a;
             public SectionFabrication fabrication = SectionFabrication.WELDED;

             public StructWeldedBoxNoWebsProperties(double depth, double width, double webThick, double FlangeThick, double outstandLength, double weldLeg, string denom)
             {
                 h = depth;
                 b = width;
                 tw = webThick;
                 tf = FlangeThick;
                 outstand = outstandLength;
                 a = weldLeg;
                 strTitle = denom;
                 A = 2 * h * tw;
                 Iyy = 2 * (tw * Math.Pow(h, 3) / 12);
                 Welyy = Iyy / (h / 2);
                 Wplyy = Math.Pow(h, 2) * tw / 2;
                 iy = Math.Sqrt(Iyy / A);
                 //Avz = double.Parse(strDef[12]); Note: Removed from here and calculated separately below.
                 Izz = 2 * (h * Math.Pow(tw, 3) / 12 + h * tw * Math.Pow((b / 2 - tw / 2), 2));
                 Welzz = Izz / (b / 2);
                 Wplzz = 2 * (h * tw * (b / 2 - tw / 2));
                 iz = Math.Sqrt(Izz / A);
                 ss = 0;
                 It = 4 * Math.Pow(((h - (2 * outstand + tf)) * (b - tw)), 2) / (2 * ((h - (2 * outstand + tf)) / tw + (b - tw) / tf));
                 Iw = 0; //Seems that that warping constant is usually taken as 0 for box sections
                 SecSymmetry = Symmetry.DOUBLYSYMMETRIC;
             }

             public override Av getEN1993ShearArea(double fy)
             {
                 Av ShearArea = new Av();
                 if (fabrication == SectionFabrication.WELDED)
                 {
                     ShearArea.Avyy = A;
                     ShearArea.Avzz = 0;
                     return ShearArea;
                 }
                 throw new NotCodedException("Case not coded");
             }

             public override int getEN1993CompressionClass(double fy)  //table 5.2 of EN 1993-1-1
             {
                 int sectclass = 4, sectclass2 = 4;
                 double epsilon = calcEpsilon(fy);

                 //Flange
                 double c = b - 2 * tw;
                 double t = tf;

                 if (c / t <= 42 * epsilon)  //don't reverse order of check
                     sectclass = 3;
                 if (c / t <= 38 * epsilon)
                     sectclass = 2;
                 if (c / t <= 33 * epsilon)
                     sectclass = 1;

                 //Inner web
                 c = h - outstand * 2 - tf * 2;
                 t = tw;

                 if (c / t <= 42 * epsilon) //don't reverse order of check
                     sectclass2 = 3;
                 if (c / t <= 38 * epsilon)
                     sectclass2 = 2;
                 if (c / t <= 33 * epsilon)
                     sectclass2 = 1;

                 sectclass = Math.Max(sectclass, sectclass2);

                 //Outstand
                 c = outstand;
                 t = tw;

                 if (c / t <= 9 * epsilon)
                     return Math.Max(sectclass, 1);
                 if (c / t <= 10 * epsilon)
                     return Math.Max(sectclass, 2);
                 if (c / t <= 14 * epsilon)
                     return Math.Max(sectclass, 3);

                 return 4;
             }

             public override int getEN1993FlexureClass(double fy)  //table 5.2 of EN 1993-1-1
             {
                 int sectclass = 4;
                 int sectclass2 = 4;
                 double epsilon = calcEpsilon(fy);

                 //Inner web
                 double c = h - 2 * outstand - 2 * tf;
                 double t = tw;

                 if (c / t <= 124 * epsilon) //don't reverse order of check
                     sectclass = 3;
                 if (c / t <= 83 * epsilon)
                     sectclass = 2;
                 if (c / t <= 72 * epsilon)
                     sectclass = 1;

                 //Flange
                 c = b - 2 * tw;
                 t = tf;

                 if (c / t <= 124 * epsilon) //don't reverse order of check
                     sectclass2 = 3;
                 if (c / t <= 83 * epsilon)
                     sectclass2 = 2;
                 if (c / t <= 72 * epsilon)
                     sectclass2 = 1;

                 sectclass = Math.Max(sectclass, sectclass2);

                 //Outstant
                 c = outstand;
                 t = tw;

                 if (c / t <= 9 * epsilon)
                     return Math.Max(sectclass, 1);
                 if (c / t <= 10 * epsilon)
                     return Math.Max(sectclass, 2);
                 if (c / t <= 14 * epsilon)
                     return Math.Max(sectclass, 3);

                 return 4;
             }

             public override EN1993CompressionBucklingCurves getEN1993CompressionBucklingCurves(EN1993SteelGrade grade) //table 6.2 of EN 1993-1-1
             {
                 EN1993CompressionBucklingCurves buckCurves = new EN1993CompressionBucklingCurves();
                 if (a > 0.5 * tf || b / tf < 30 || h / tw < 30)
                 {

                     buckCurves.majorAxis = EN1993CompressionBucklingCurve.c;
                     buckCurves.minorAxis = EN1993CompressionBucklingCurve.c;
                     return buckCurves;
                 }

                 buckCurves.majorAxis = EN1993CompressionBucklingCurve.b;
                 buckCurves.minorAxis = EN1993CompressionBucklingCurve.b;
                 return buckCurves;

             }

             public override EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
             {
                 EN1993LTBBucklingCurve LTBbuckCurve = new EN1993LTBBucklingCurve();
                 LTBbuckCurve.LTBBucklingCurve = EN1993LTBBucklingCurves.d;
                 return LTBbuckCurve;
             }

             public string genGWADef()
             {
                 string gwaDef = "GEO P(m) ";
                 double x = -b / 2, y = h / 2;
                 gwaDef += "M(" + x + "|" + y + ") ";
                 x = -b / 2 + tw;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = h / 2 - outstand;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = b / 2 - tw;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = h / 2;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = b / 2;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = -h / 2;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = b / 2 - tw;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = -h / 2 + outstand;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = -b / 2 + tw;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = -h / 2;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = -b / 2;
                 gwaDef += "L(" + x + "|" + y + ") ";

                 x = -b / 2 + tw;
                 y = h / 2 - outstand - tf;
                 gwaDef += "M(" + x + "|" + y + ") ";
                 x = -x;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 y = -y;
                 gwaDef += "L(" + x + "|" + y + ") ";
                 x = -x;
                 gwaDef += "L(" + x + "|" + y + ") ";

                 return gwaDef;
             }
         }
	}
}
