using System;
using System.Text;
using System.Collections;


namespace SectionCatalogue
{
    public static class parseGWA
    {
        private static StructCatalogueCollection catalogue;

        static parseGWA()
        {
            catalogue = new StructCatalogueCollection();

        }


        public static StructProperties.StructBaseProperties parseGWASectStr(string GWADef)
        {
            string[] arrSectDesc, arrStrings;
            string strSecType;
            double scaleSect;

            arrSectDesc = GWADef.Split("% ".ToCharArray());
            // scale section

            scaleSect = 0.001;
            if (arrSectDesc.GetUpperBound(0) < 1)
            {
                int icounter = 0, itemp;

                while (icounter < GWADef.Length && !int.TryParse(GWADef.Substring(icounter, 1), out itemp))
                    icounter++;
                string cat = GWADef.Substring(0, icounter);
                return catalogue.getCatProperties(cat, GWADef);

            }

            if (arrSectDesc[1].IndexOf("(m)", StringComparison.CurrentCultureIgnoreCase) >= 0)
                scaleSect = 1;
            if (arrSectDesc[1].IndexOf("(cm)", StringComparison.CurrentCultureIgnoreCase) >= 0)
                scaleSect = 0.001;
            if (arrSectDesc[1].IndexOf("(in)", StringComparison.CurrentCultureIgnoreCase) >= 0)
                scaleSect = 0.0254;
            if (arrSectDesc[1].IndexOf("(ft)", StringComparison.CurrentCultureIgnoreCase) >= 0)
                scaleSect = 12 * 0.0254;
            arrStrings = arrSectDesc[1].Split("(".ToCharArray());
            strSecType = arrStrings[0];
            if (string.Compare("STD", arrSectDesc[0], true) == 0)
            {
                if (string.Compare(strSecType, "R", true) == 0)
                {
                    return new StructProperties.StructRectProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "RHS", true) == 0)
                {
                    return new StructProperties.StructRHSProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]), GWADef);
                }
                if (string.Compare(strSecType, "C", true) == 0)
                {
                    return new StructProperties.StructCircProperties(double.Parse(arrSectDesc[2]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "CHS", true) == 0)
                {
                    return new StructProperties.StructCHSProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "I", true) == 0)
                {
                    return new StructProperties.StructIProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, 0, GWADef);
                }
                if (string.Compare(strSecType, "IT", true) == 0) // todo inverted section
                {
                    return null;//	return new structTapTeeProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "T", true) == 0)
                {
                    return new StructProperties.StructTapTeeProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "CH", true) == 0)
                {
                    return new StructProperties.StructChannelProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "DCH", true) == 0)
                {
                    return new StructProperties.StructIProperties(double.Parse(arrSectDesc[2]) * scaleSect * 2, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect * 2, double.Parse(arrSectDesc[5]) * scaleSect, 0, GWADef);
                }
                if (string.Compare(strSecType, "A", true) == 0)
                {
                    return new StructProperties.StructAngleProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "IA", true) == 0) // todo inverted section
                {
                    return null;//	return new structTapTeeProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "D", true) == 0)
                {
                    return new StructProperties.StructTapTeeProperties(double.Parse(arrSectDesc[2]) * scaleSect * 2, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect * 2, double.Parse(arrSectDesc[4]) * scaleSect * 2, double.Parse(arrSectDesc[5]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "TR", true) == 0)
                {
                    return new StructProperties.StructTaperProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "E", true) == 0)
                {
                    if (int.Parse(arrSectDesc[4]) == 1)
                        return new StructProperties.StructDiamProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef);

                    return new StructProperties.StructEllipseProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "GI", true) == 0)
                {
                    return new StructProperties.StructTapIProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, double.Parse(arrSectDesc[7]) * scaleSect, double.Parse(arrSectDesc[8]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "TT", true) == 0)
                {
                    return new StructProperties.StructTapTeeProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "TA", true) == 0)
                {
                    return new StructProperties.StructTapAngProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "TI", true) == 0)
                {
                    return new StructProperties.StructTapIProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, double.Parse(arrSectDesc[7]) * scaleSect, double.Parse(arrSectDesc[8]) * scaleSect, GWADef);
                }
                if (string.Compare(strSecType, "RC", true) == 0)//todo update recto-ellipse
                {
                    return new StructProperties.StructRectCircProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef); // todo check recto-ellipse
                }
                if (string.Compare(strSecType, "Oval", true) == 0)//todo oval
                {
                    return new StructProperties.StructEllipseProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, GWADef); // todo check oval
                }
				if (string.Compare(strSecType, "BG", true) == 0) // Expedition format for box girder, not in GSA
				{
                    if (arrSectDesc.GetUpperBound(0) == 7)
					    return new StructProperties.StructWeldedBoxProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, double.Parse(arrSectDesc[7]) * scaleSect, GWADef);
                    return new StructProperties.StructWeldedBoxProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, 0, GWADef);
				}
                if (string.Compare(strSecType, "INW", true) == 0) // Expedition format for I section without web, not in GSA
                {
                    return new StructProperties.StructINoWebProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, 0, GWADef);
                }
                if (string.Compare(strSecType, "BGNW", true) == 0) // Expedition format for box girder without web, not in GSA
                {
                    if (arrSectDesc.GetUpperBound(0) == 7)
                        return new StructProperties.StructWeldedBoxNoWebsProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, double.Parse(arrSectDesc[7]) * scaleSect, GWADef);
                    return new StructProperties.StructWeldedBoxNoWebsProperties(double.Parse(arrSectDesc[2]) * scaleSect, double.Parse(arrSectDesc[3]) * scaleSect, double.Parse(arrSectDesc[4]) * scaleSect, double.Parse(arrSectDesc[5]) * scaleSect, double.Parse(arrSectDesc[6]) * scaleSect, 0, GWADef);
                }
                //todo rhino warning section not recognized
            }
            if (string.Compare("CAT", arrSectDesc[0], true) == 0)
            {
                return catalogue.getCatProperties(arrSectDesc[1], arrSectDesc[2]);
            }



            if (string.Compare("GEO", arrSectDesc[0], true) == 0)
            {
                return new StructProperties.StructUserProperties(GWADef, scaleSect);
            }
            return null;
        }




    }
}
