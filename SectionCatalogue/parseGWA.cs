using SectionCatalogue.SectionProperties;
using System;
namespace SectionCatalogue
{
    public static class parseGWA
    {
        private static SectionCatalogueFactory catalogue;
        static parseGWA()
        {
            parseGWA.catalogue = new SectionCatalogueFactory();
        }
        public static SectionBase parseGWASectStr(string GWADef)
        {
            string[] array = GWADef.Split("% ".ToCharArray());
            double num = 0.001;
            if (array.GetUpperBound(0) < 1)
            {
                int num2 = 0;
                int num3;
                while (num2 < GWADef.Length && !int.TryParse(GWADef.Substring(num2, 1), out num3))
                {
                    num2++;
                }
                string type = GWADef.Substring(0, num2);
                return parseGWA.catalogue.GetCatProperties(type, GWADef);
            }
            if (array[1].IndexOf("(m)", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                num = 1.0;
            }
            if (array[1].IndexOf("(cm)", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                num = 0.001;
            }
            if (array[1].IndexOf("(in)", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                num = 0.0254;
            }
            if (array[1].IndexOf("(ft)", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                num = 0.30479999999999996;
            }
            string[] array2 = array[1].Split("(".ToCharArray());
            string strA = array2[0];
            if (string.Compare("STD", array[0], true) == 0)
            {
                if (string.Compare(strA, "R", true) == 0)
                {
                    return new StructRectProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                }
                if (string.Compare(strA, "RHS", true) == 0)
                {
                    return new RectangularHollowSection(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]), GWADef);
                }
                if (string.Compare(strA, "C", true) == 0)
                {
                    return new StructCircProperties(double.Parse(array[2]) * num, GWADef);
                }
                if (string.Compare(strA, "CHS", true) == 0)
                {
                    return new CircularHollowSection(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                }
                if (string.Compare(strA, "I", true) == 0)
                {
                    return new ISection(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, 0.0, GWADef);
                }
                if (string.Compare(strA, "IT", true) == 0)
                {
                    return null;
                }
                if (string.Compare(strA, "T", true) == 0)
                {
                    return new StructTapTeeProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, GWADef);
                }
                if (string.Compare(strA, "CH", true) == 0)
                {
                    return new StructChannelProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, GWADef);
                }
                if (string.Compare(strA, "DCH", true) == 0)
                {
                    return new ISection(double.Parse(array[2]) * num * 2.0, double.Parse(array[3]) * num, double.Parse(array[4]) * num * 2.0, double.Parse(array[5]) * num, 0.0, GWADef);
                }
                //if (string.Compare(strA, "A", true) == 0)
                //{
                //    return new EASection(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, GWADef);
                //}
                if (string.Compare(strA, "IA", true) == 0)
                {
                    return null;
                }
                if (string.Compare(strA, "D", true) == 0)
                {
                    return new StructTapTeeProperties(double.Parse(array[2]) * num * 2.0, double.Parse(array[3]) * num, double.Parse(array[4]) * num * 2.0, double.Parse(array[4]) * num * 2.0, double.Parse(array[5]) * num, GWADef);
                }
                if (string.Compare(strA, "TR", true) == 0)
                {
                    return new StructTaperProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, GWADef);
                }
                if (string.Compare(strA, "E", true) == 0)
                {
                    if (int.Parse(array[4]) == 1)
                    {
                        return new StructDiamProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                    }
                    return new StructEllipseProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                }
                else
                {
                    if (string.Compare(strA, "GI", true) == 0)
                    {
                        return new StructTapIProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, double.Parse(array[7]) * num, double.Parse(array[8]) * num, GWADef);
                    }
                    if (string.Compare(strA, "TT", true) == 0)
                    {
                        return new StructTapTeeProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, GWADef);
                    }
                    if (string.Compare(strA, "TA", true) == 0)
                    {
                        return new StructTapAngProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, GWADef);
                    }
                    if (string.Compare(strA, "TI", true) == 0)
                    {
                        return new StructTapIProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, double.Parse(array[7]) * num, double.Parse(array[8]) * num, GWADef);
                    }
                    if (string.Compare(strA, "RC", true) == 0)
                    {
                        return new StructRectCircProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                    }
                    if (string.Compare(strA, "Oval", true) == 0)
                    {
                        return new StructEllipseProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, GWADef);
                    }
                    if (string.Compare(strA, "BG", true) == 0)
                    {
                        if (array.GetUpperBound(0) == 7)
                        {
                            return new StructWeldedBoxProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, double.Parse(array[7]) * num, GWADef);
                        }
                        return new StructWeldedBoxProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, 0.0, GWADef);
                    }
                    else
                    {
                        if (string.Compare(strA, "INW", true) == 0)
                        {
                            return new StructINoWebProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, 0.0, GWADef);
                        }
                        if (string.Compare(strA, "BGNW", true) == 0)
                        {
                            if (array.GetUpperBound(0) == 7)
                            {
                                return new StructWeldedBoxNoWebsProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, double.Parse(array[7]) * num, GWADef);
                            }
                            return new StructWeldedBoxNoWebsProperties(double.Parse(array[2]) * num, double.Parse(array[3]) * num, double.Parse(array[4]) * num, double.Parse(array[5]) * num, double.Parse(array[6]) * num, 0.0, GWADef);
                        }
                    }
                }
            }
            if (string.Compare("CAT", array[0], true) == 0)
            {
                return parseGWA.catalogue.GetCatProperties(array[1], array[2]);
            }
            if (string.Compare("GEO", array[0], true) == 0)
            {
                return new StructUserProperties(GWADef, num);
            }
            return null;
        }
    }
}
