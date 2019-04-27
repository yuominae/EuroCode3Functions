using System;
using System.Collections.Generic;
using System.Text;

namespace SectionCatalogue
{

    namespace StructProperties
    {
		public class StructTaperProperties : StructBaseProperties
		{
			public double h, bt, bb;
			public StructTaperProperties(double depth, double widthTop, double widthBot, string denom)
			{
				h = depth;
				bt = widthTop;
				bb = widthBot;
				strTitle = denom;
			}
			
		}

		public class StructDiamProperties : StructBaseProperties
		{
			public double h, b;
			public StructDiamProperties(double depth, double width, string denom)
			{
				h = depth;
				b = width;
				strTitle = denom;
			}
			
		}

		public class StructEllipseProperties : StructBaseProperties
		{
			public double h, b;
			public StructEllipseProperties(double depth, double width, string denom)
			{
				h = depth;
				b = width;
				strTitle = denom;
			}
			
		}

		public class StructTapIProperties : StructBaseProperties
		{
			public double h, bt, bb, twt, twb, tfb, tft;

			public StructTapIProperties(double depth, double widthTop, double widthBot, double webThickTop, double webThickBot, double FlangeThickTop, double FlangeThickBot, string denom)
			{
				h = depth;
				bt = widthTop;
				bb = widthBot;
				twt = webThickTop;
				twb = webThickBot;
				tft = FlangeThickTop;
				tfb = FlangeThickBot;
				strTitle = denom;
			}
			
		}

		public class StructTapTeeProperties : StructBaseProperties
		{
			public double h, b, twt, tf, twb;

			public StructTapTeeProperties(double depth, double width, double webThickTop, double webThickBot, double flangeThick, string denom)
			{
				strTitle = denom;
				h = depth;
				b = width;
				twt = webThickTop;
				twb = webThickBot;
				tf = flangeThick;
			}
			

		}
		public class StructTapAngProperties : StructBaseProperties
		{
			public double h, b, twt, tf, twb;

			public StructTapAngProperties(double depth, double width, double webThickTop, double webThickBot, double flangeThick, string denom)
			{
				strTitle = denom;
				h = depth;
				b = width;
				twt = webThickTop;
				twb = webThickBot;
				tf = flangeThick;
			}
			

		}
		public class StructRectCircProperties : StructBaseProperties
		{
			public double h, b;
			public StructRectCircProperties(double depth, double width, string denom)
			{
				h = depth;
				b = width;
				strTitle = denom;
			}
			
		}

		public class StructUserProperties : StructBaseProperties
		{
			public double scaleFactor = 1;
			public StructUserProperties(string denom, double ScaleFactor)
			{
				strTitle = denom;
				scaleFactor = ScaleFactor;
			}
		}
	}
}
