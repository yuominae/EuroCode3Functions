using System;
using System.Collections.Generic;
using System.Text;
#if(EXCELDNA)
using ExcelDna.Integration;
#endif
using SSI.SectionCatalogue.StructProperties;
using SSI.SectionCatalogue;

namespace EC3Functions
{
    public static partial class EC3Functions
    {

        //Exception definitions
        private static class CustomExceptions
        {
            public class GeneralException : Exception
            {
                public GeneralException(string message)
                    : base(message)
                {
                }
            }

            public class SectionNotFoundException : Exception
            {
                public SectionNotFoundException()
                    : base("Section not found")
                {
                }
            }

            public class Class4SectionException : Exception
            {
                public Class4SectionException()
                    : base("Section is class 4")
                {
                }
            }

            public class SectionNotYetHandledException : Exception
            {
                public SectionNotYetHandledException()
                    : base("Section not yet handled")
                {
                }
            }

            public class InvalidSectionClassException : Exception
            {
                public InvalidSectionClassException()
                    : base("Invalid section class")
                {
                }
            }
        }

        //Enumerations definitions
        enum SectionTypes { I, RHS, CHS, BOX, INOWEB, ICS, BOXNOWEB, WBFBOX, ANGLE, CHANNEL, USERDEFINED, OTHER };
        enum ShearAxis { Y, Z };
        enum BendingAxis { YY, ZZ };

		/*
         * 
         * Design strength calculations to BS:EN10025, variant 2:
         * Determines teh steel strength for catalogue sections and custom sections handled by the plugin (box girders etc.).
         * This function calls variant 1 of the design strength calculation function and passes it the steel grade and the
         * maximum thickness for of the catalogue section
         * Not all sections have been implemented yet.
         * 
        */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Section_Design_Strength_BSEN10025", Description = "Section design strength calculations in N/sq.mm (BS:EN10025)")]
		public static object SectionDesignStrength([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")]string SteelGrade)
 
#else
		public static object SectionDesignStrength(string SectionDenomination,
            string SteelGrade)
 
#endif
		{
            try
            {
                SectionTypes ThisSection = DetermineType(SectionDenomination);
                double MaxThickness = 0;
                switch (ThisSection)
                {
                    case SectionTypes.I:
                        StructIProp ThisISection = (StructIProp)RetrieveISectionProps(SectionDenomination);
                        MaxThickness = Math.Max(ThisISection.tf, ThisISection.tw) * 1e3;
                        return DesignStrength(SteelGrade, MaxThickness);
                    case SectionTypes.CHS:
                        StructCHSProp ThisCHSSection = (StructCHSProp)RetrieveCHSProps(SectionDenomination);
                        return DesignStrength(SteelGrade, ThisCHSSection.t * 1e3);
                    case SectionTypes.RHS:
                        StructRHSProp ThisRHSSection = (StructRHSProp)RetrieveRHSProps(SectionDenomination);
                        return DesignStrength(SteelGrade, ThisRHSSection.t * 1e3);
                    case SectionTypes.BOX:
                        StructWeldedBoxProp ThisBOXSection = (StructWeldedBoxProp)RetrieveWELDEDBOXProps(SectionDenomination);
                        MaxThickness = Math.Max(ThisBOXSection.tf, ThisBOXSection.tw) * 1e3;
                        return DesignStrength(SteelGrade, MaxThickness);
                    case SectionTypes.INOWEB:
                        StructINoWebProp ThisINoWebSection = (StructINoWebProp)RetrieveINoWebSectionProps(SectionDenomination);
						MaxThickness = Math.Max(ThisINoWebSection.tf, ThisINoWebSection.tw) * 1e3;
                        return DesignStrength(SteelGrade, MaxThickness);
					case SectionTypes.ICS:
						StructIStiffWebProp ThisIStiffWebSection = (StructIStiffWebProp)RetrieveIStiffWebSectionProps(SectionDenomination);
						MaxThickness =  Math.Max(ThisIStiffWebSection.tf, Math.Max(ThisIStiffWebSection.tw,ThisIStiffWebSection.tStiff)) * 1e3;
						return DesignStrength(SteelGrade, MaxThickness);
                    case SectionTypes.BOXNOWEB:
                        StructWeldedBoxNoWebsProp ThisBoxNoWebsSection = (StructWeldedBoxNoWebsProp)RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
						MaxThickness = Math.Max(ThisBoxNoWebsSection.tf, ThisBoxNoWebsSection.tw) * 1e3;
                        return DesignStrength(SteelGrade, MaxThickness);
                    default:
                        throw new CustomExceptions.SectionNotFoundException();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
		}

        /*
         *
         * These functions retrieve the the detailed section properties (dimensions + Areas etc.)
         * for the catalogue sections
         * 
        */
        private static StructBaseProperties RetrieveSectionProps(string Denomination)
        {
			StructBaseProperties Section = (StructBaseProperties)parseGWA.parseGWASectStr(Denomination);
            if (Section == null)
                throw (new CustomExceptions.SectionNotFoundException());
            else
                return Section;
        }
        private static StructIProp RetrieveISectionProps(string Denomination)
        {
            StructIProp Section = (StructIProp)parseGWA.parseGWASectStr(Denomination);
            if(Section == null)
                throw (new CustomExceptions.SectionNotFoundException());
            else
                return Section;
        }
        private static StructRHSProp RetrieveRHSProps(string Denomination)
        {
            StructRHSProp Section = (StructRHSProp)parseGWA.parseGWASectStr(Denomination);
            if (Section == null)
                throw (new CustomExceptions.SectionNotFoundException());
            else
                return Section;
        }
        private static StructCHSProp RetrieveCHSProps(string Denomination)
        {
            StructCHSProp Section = (StructCHSProp)parseGWA.parseGWASectStr(Denomination);
            if (Section == null)
                throw new CustomExceptions.SectionNotFoundException();
            else
                return Section;
        }
        private static StructWeldedBoxProp RetrieveWELDEDBOXProps(string Denomination)
        {
            StructWeldedBoxProp Section = (StructWeldedBoxProp)parseGWA.parseGWASectStr(Denomination);
            if(Section == null)
                throw new CustomExceptions.SectionNotFoundException();
            else 
                return Section;
        }
        private static StructINoWebProp RetrieveINoWebSectionProps(string Denomination)
        {
            StructINoWebProp Section = (StructINoWebProp)parseGWA.parseGWASectStr(Denomination);
            if (Section == null)
                throw (new CustomExceptions.SectionNotFoundException());
            else
                return Section;
        }
		private static StructIStiffWebProp RetrieveIStiffWebSectionProps(string Denomination)
		{
			StructIStiffWebProp Section = (StructIStiffWebProp)parseGWA.parseGWASectStr(Denomination);
			if (Section == null)
				throw (new CustomExceptions.SectionNotFoundException());
			else
				return Section;
		}
        private static StructWeldedBoxNoWebsProp RetrieveWELDEDBOXNoWebsSectionProps(string Denomination)
        {
            StructWeldedBoxNoWebsProp Section = (StructWeldedBoxNoWebsProp)parseGWA.parseGWASectStr(Denomination);
            if (Section == null)
                throw (new CustomExceptions.SectionNotFoundException());
            else
                return Section;
        }
		private static StructWBFWeldedBoxProp RetrieveWBFBOXSectionProps(string Denomination)
		{
			StructWBFWeldedBoxProp Section = (StructWBFWeldedBoxProp)parseGWA.parseGWASectStr(Denomination);
			if (Section == null)
				throw (new CustomExceptions.SectionNotFoundException());
			else
				return Section;
		}
        /*
         * 
         * Determines the type of section from the catalogue
         * Tries to cast the section to each section type in turn until a match is found.
         * 
        */
        private static SectionTypes DetermineType(string Denomination)
        {
            try
            {
                StructBaseProperties BaseSection = (StructBaseProperties)parseGWA.parseGWASectStr(Denomination);
                if (BaseSection == null)
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch{}
            try
            {
                    StructIProperties Section = (StructIProperties)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.I;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch{}
            try
            {
                StructRHSProperties Section = (StructRHSProperties)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.RHS;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch{}
            try
            {
                StructCHSProp Section = (StructCHSProp)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.CHS;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch{}
            try
            {
                StructWeldedBoxProp Section = (StructWeldedBoxProp)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.BOX;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch{}
            try
            {
                StructINoWebProp Section = (StructINoWebProp)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.INOWEB;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch {}
			try
			{
				StructIStiffWebProp Section = (StructIStiffWebProp)parseGWA.parseGWASectStr(Denomination);
				if (Section != null)
					return SectionTypes.ICS;
				else
					throw new CustomExceptions.SectionNotFoundException();
			}
			catch { }
            try
            {
                StructWeldedBoxNoWebsProp Section = (StructWeldedBoxNoWebsProp)parseGWA.parseGWASectStr(Denomination);
                if (Section != null)
                    return SectionTypes.BOXNOWEB;
                else
                    throw new CustomExceptions.SectionNotFoundException();
            }
            catch {}
            return SectionTypes.OTHER;
        }
        /*
         * Determines the max steel strength according to the steel grade.
        */
        private static object YieldStrength(string SteelGrade)
        {
            switch ((EN1993SteelGrade)Enum.Parse(typeof(EN1993SteelGrade), SteelGrade.ToUpper()))
            {
                case EN1993SteelGrade.S235:
                    return 235;
                case EN1993SteelGrade.S275:
                    return 275;
                case EN1993SteelGrade.S355:
                    return 355;
                case EN1993SteelGrade.S420:
                    return 420;
                case EN1993SteelGrade.S450:
                    return 450;
                case EN1993SteelGrade.S460:
                    return 460;
                default:
                    throw new CustomExceptions.GeneralException("Steel grade not found");
            }
        }
		#if(EXCELDNA)
        /*
         * Version and help functions
        */

        [ExcelFunction(Category = "EC3 Functions Help", Name = "Version",Description = "EXP functions version information")]
        public static string ExpeditionEC3Version()
        {
            return "Expedition EC3 functions version 1.42. Last updated 20/03/2009";
        }

        /*
         * 
         * Design strength calculations to BS:EN10025, variant 1:
         * Determines the design strength given the steel grade and the thickness of the steel plate
         * according to BS:EN10025-2 tab. 7 and the Turin design criteria.
         * Note that the S460 grade is handled according to the Turin design criteria and might need to be revised.
         * 
        */

        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Steel_Design_Strength_BSEN10025", Description = "Design strength calculations in N/sq.mm according to BS:EN10025")]
        public static object DesignStrength([ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")]string SteelGrade,
            [ExcelArgument(Name = "Thickness", Description = "Plate thickness in mm")]double Thickness)
#else
	
        public static object DesignStrength(string SteelGrade,double Thickness)
#endif
		{
            try
            {
                //All values from BS EN 10025:2 table 7
                //Steel minimum yield stengths for S235J grade steel 
                if (Thickness <= 0)
                {
                    throw new CustomExceptions.GeneralException("Thickness less than or equal to 0");
                }
                else
                {
                    switch ((EN1993SteelGrade)Enum.Parse(typeof(EN1993SteelGrade), SteelGrade.ToUpper()))
                    {
                        case EN1993SteelGrade.S235:
                            if (Thickness <= 16)
                                return 235;
                            else
                            {
                                if (Thickness <= 40)
                                    return 225;
                                else
                                {
                                    if (Thickness <= 63)
                                        return 215;
                                    else
                                    {
                                        if (Thickness <= 80)
                                            return 215;
                                        else
                                        {
                                            if (Thickness <= 100)
                                                return 215;
                                            else
                                            {
                                                if (Thickness <= 150)
                                                    return 195;
                                                else
                                                {
                                                    if (Thickness <= 200)
                                                        return 185;
                                                    else
                                                    {
                                                        if (Thickness <= 250)
                                                            return 175;
                                                        else
                                                        {
                                                            if (Thickness <= 400)
                                                                return 165;
                                                            else
                                                                throw new CustomExceptions.GeneralException("Thickness exceeds code limits");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        //Steel minimum yield stengths for S275J grade steel 
                        case EN1993SteelGrade.S275:
                            if (Thickness <= 16)
                                return 275;
                            else
                            {
                                if (Thickness <= 40)
                                    return 265;
                                else
                                {
                                    if (Thickness <= 63)
                                        return 255;
                                    else
                                    {
                                        if (Thickness <= 80)
                                            return 245;
                                        else
                                        {
                                            if (Thickness <= 100)
                                                return 235;
                                            else
                                            {
                                                if (Thickness <= 150)
                                                    return 225;
                                                else
                                                {
                                                    if (Thickness <= 200)
                                                        return 215;
                                                    else
                                                    {
                                                        if (Thickness <= 250)
                                                            return 205;
                                                        else
                                                        {
                                                            if (Thickness <= 400)
                                                                return 195;
                                                            else
                                                                throw new CustomExceptions.GeneralException("Thickness exceeds code limits");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        //Steel minimum yield stengths for S355J grade steel
                        case EN1993SteelGrade.S355:
                            if (Thickness <= 16)
                                return 355;
                            else
                            {
                                if (Thickness <= 40)
                                    return 345;
                                else
                                {
                                    if (Thickness <= 63)
                                        return 335;
                                    else
                                    {
                                        if (Thickness <= 80)
                                            return 325;
                                        else
                                        {
                                            if (Thickness <= 100)
                                                return 315;
                                            else
                                            {
                                                if (Thickness <= 150)
                                                    return 295;
                                                else
                                                {
                                                    if (Thickness <= 200)
                                                        return 285;
                                                    else
                                                    {
                                                        if (Thickness <= 250)
                                                            return 275;
                                                        else
                                                        {
                                                            if (Thickness <= 400)
                                                                return 265;
                                                            else
                                                                throw new CustomExceptions.GeneralException("Thickness exceeds code limits");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        //Steel minimum yield stengths for S450J grade steel
                        case EN1993SteelGrade.S450:
                            if (Thickness <= 16)
                                return 450;
                            else
                            {
                                if (Thickness <= 40)
                                    return 430;
                                else
                                {
                                    if (Thickness <= 63)
                                        return 410;
                                    else
                                    {
                                        if (Thickness <= 80)
                                            return 390;
                                        else
                                        {
                                            if (Thickness <= 100)
                                                return 380;
                                            else
                                            {
                                                if (Thickness <= 150)
                                                    return 380;
                                                else
                                                    throw new CustomExceptions.GeneralException("Thickness exceeds code limits");
                                            }
                                        }
                                    }
                                }
                            }
                        //Steel minimum yield stengths for S460J grade steel
                        case EN1993SteelGrade.S460:
                            if (Thickness <= 16)
                                return 460;
                            else
                            {
                                if (Thickness <= 40)
                                    return 440;
                                else
                                {
                                    if (Thickness <= 63)
                                        return 430;
                                    else
                                    {
                                        if (Thickness <= 80)
                                            return 410;
                                        else
                                        {
                                            if (Thickness <= 100)
                                                return 400;
                                            else
                                            {
                                                if (Thickness <= 150)
                                                    return 385;
                                                else
                                                    throw new CustomExceptions.GeneralException("Thickness exceeds code limits");
                                            }
                                        }
                                    }
                                }
                            }
                        default:
                            throw new CustomExceptions.GeneralException("Steel grade not covered");
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
#if(EXCELDNA)
        /*
         * Determines the compression or bending class for catalogues section given the section denomination
         * by calling the methods associated with the catalogue sections
        */
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_class_EC3", Description = "Section class for bending or compression (EC3 cl 5.5)")]
        public static object SectionClass([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")]string SteelGrade,
            [ExcelArgument(Name = "Type", Description = "(Optional) Classification type: ''BENDING'' or ''COMPRESSION''. Input as string")]string ClassificationType)
#else
         public static object SectionClass(string SectionDenomination,
            string SteelGrade,
            string ClassificationType)

#endif
        {
            try
            {
				IBaseProp ThisSectionBaseProps = (IBaseProp)RetrieveSectionProps(SectionDenomination);

                double Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());

					switch (ClassificationType.ToUpper())
					{
						case "BENDING":
							return ThisSectionBaseProps.getEN1993FlexureClass(Designfy);
						case "COMPRESSION":
							return ThisSectionBaseProps.getEN1993CompressionClass(Designfy);
						default:
							return Math.Max(ThisSectionBaseProps.getEN1993FlexureClass(Designfy), ThisSectionBaseProps.getEN1993CompressionClass(Designfy));
				}
            }
            catch (Exception e)
            {
                throw e; 
            }
        }
        //
         /*Section properties
          * Returns all or some section properties for a given section from the section catalogue
         */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Section_properties", Description = "Outputs the section properties: A, Iyy, Izz, iy, iz, Welyy, Welzz, Wplyy, Wplzz, It, Iw. Base unit: mm")]
        public static object SectionProperties([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Property index", Description = "Section property index: 1.A, 2.Iyy, 3.Izz, 4.iy, 5.iz, 6.Welyy, 7.Welzz, 8.Wplyy, 9.Wplzz, 10.It, 11.Iw")] int PropIndex)
#else
        public static object SectionProperties(string SectionDenomination,
            int PropIndex)
#endif
        {
            try
            {
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
                
                double[] SectionPropsArray = new double[11];
                SectionPropsArray[0] = ThisSectionBaseProps.A * 1e6;
                SectionPropsArray[1] = ThisSectionBaseProps.Iyy * 1e12;
                SectionPropsArray[2] = ThisSectionBaseProps.Izz * 1e12;
                SectionPropsArray[3] = ThisSectionBaseProps.iy * 1e3;
                SectionPropsArray[4] = ThisSectionBaseProps.iz * 1e3;
                SectionPropsArray[5] = ThisSectionBaseProps.Welyy * 1e9;
                SectionPropsArray[6] = ThisSectionBaseProps.Welzz * 1e9;
                SectionPropsArray[7] = ThisSectionBaseProps.Wplyy * 1e9;
                SectionPropsArray[8] = ThisSectionBaseProps.Wplzz * 1e9;
                SectionPropsArray[9] = ThisSectionBaseProps.It * 1e12;
                SectionPropsArray[10] = ThisSectionBaseProps.Iw * 1e18;

                if (PropIndex < 0 || PropIndex > 11)
                    throw new CustomExceptions.GeneralException("Invalid property index");
                else if (PropIndex == 0)
                    return SectionPropsArray;
                else
                    return SectionPropsArray[PropIndex - 1];
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        //
#if(EXCELDNA)
		[ExcelFunction(Category = "EXP Engineering Functions", Name = "Generate_Box_Section_GWA", Description = "Outputs the GSA walkaround code for a box section")]
        public static object GENBoxSectionGwa([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination)
#else
      public static object GENBoxSectionGwa(string SectionDenomination)
#endif
    {
            try
            {
                    SectionTypes ThisSectionType = (SectionTypes)DetermineType(SectionDenomination);
					switch (ThisSectionType)
					{
						case SectionTypes.BOX:
							StructWeldedBoxProp ThisBoxSectionProps = RetrieveWELDEDBOXProps(SectionDenomination);
							return ThisBoxSectionProps.genGWADef();
						case SectionTypes.BOXNOWEB:
							StructWeldedBoxNoWebsProp ThisBoxNoWebsSectionProps = RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
							return ThisBoxNoWebsSectionProps.genGWADef();
						default:
							throw new CustomExceptions.SectionNotYetHandledException();
					}
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        //
        //
        /*
         * Cross section resistance calculations
         * Note that this section is not quite correct yet: 
         * *The cross section capacities are worked out correctly, but the interaction checks still need to be coded properly.
         * *Shear checks need to be coded still.
         * *Class 4 sections can not be dealt with yet.
         *
         */
      /*
       * Cross section tension resistance calculated for according to BS:EN1993-1 cl. 6.2.4
       * NOT YET READY TO USE
       */
#if(EXCELDNA)
        [ExcelFunction(Category = "EC3 Test functions", Name = "Cross_Section_Tension_Resistance", Description = "Cross-section tension resistance in kN, see EC3 cl. 6.2.3")]
        private static object CrossSectionTensionResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
			[ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "Area reduction", Description = "(Optional) Ratio of the net cross section area over the gross cross section area. (Default = 1)")] double AreaRed,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "GammaM2", Description = "(Optional) Partial safety factor for net cross-section resistance. (Default = 1.25)")] double GammaM2)
#else
            private static object CrossSectionTensionResistance(string SectionDenomination,
			object SteelGrade,
            double AreaRed,
            double GammaM0,
            double GammaM2)
#endif
        {
            try
            {
                //Default for GammaM0
                if (GammaM0 == 0)
                    GammaM0 = 1;

                if (GammaM2 == 0)
                    GammaM2 = 1.25;

                if (AreaRed == 0)
                    AreaRed = 1;
                else if (AreaRed > 1)
                    throw new CustomExceptions.GeneralException("Net area can not exceed gross area");

				//Determine design yield stress
				double Designfy;
				if (!double.TryParse(SteelGrade.ToString(), out Designfy))
					Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
                double A = ThisSectionBaseProps.A * 1e6;
                double GrossTensionCapacity = A * Designfy / GammaM0;

                if (AreaRed < 1)
                    throw new CustomExceptions.GeneralException("Net cross section areas not dealt with yet");

                return GrossTensionCapacity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
            /*
             * Cross section compression resistance calculated for according to BS:EN1993-1 cl. 6.2.4
             */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Compression_Resistance", Description = "Section compression resistance in kN (EC3 cl. 6.2.4)")]
        public static object CrossSectionCompressionResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
#else
          public static object CrossSectionCompressionResistance(string SectionDenomination,
           object SteelGrade,
             double GammaM0,
            int CompClass)  
#endif
    {
            try
            {
				StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)RetrieveSectionProps(SectionDenomination);
                //Default for GammaM0
                if (GammaM0 == 0)
                    GammaM0 = 1;

				//Determine design yield stress
				double Designfy;
				if (!double.TryParse(SteelGrade.ToString(),out Designfy))
					Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());

                //Checks the section class
				if (CompClass < 0 || CompClass > 4)
				{
					throw new CustomExceptions.InvalidSectionClassException();
				}
				else
				{
					if (CompClass == 0)
						CompClass = baseProp.getEN1993CompressionClass(Designfy);
					if (CompClass == 4)
						throw new CustomExceptions.Class4SectionException();
				}

                double A = ThisSectionBaseProps.A * 1e6; // Area in sq.mm
                    return (A * Designfy / GammaM0) / 1e3;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
          /*
           * Cross section bending resistance calculated according to BS:EN1993-1 cl. 6.2.5
           * This check now includes checks for the shear force according to BS:EN1993-1 cl. 6.2.8, but only for I, RHS and welded box sections
           */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Bending_Resistance", Description = "Section bending resistance in kNm (EC3 cl. 6.2.5)")]
        public static object CrossSectionBendingResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
			[ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "Bending axis", Description = "Axis of bending, either ''YY'' or ''ZZ''")] string Axis,
            [ExcelArgument(Name = "VEd", Description = "(Optional) Design shear force perpendicular to the axis of bending in kN. (Default = 0kN)")] double DesignShear,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
#else
          public static object CrossSectionBendingResistance(string SectionDenomination,
			object SteelGrade,
            string Axis,
            double DesignShear,
            double GammaM0,
            int BendClass)
#endif
		{
            try
            {
                double W = 0;

				//Determine design yield stress
				double Designfy;
				if (!double.TryParse(SteelGrade.ToString(), out Designfy))
					Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());

                //Default for GammaM0
                if (GammaM0 == 0)
                    GammaM0 = 1;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
                //Checks the section bending class
                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
                        BendClass = baseProp.getEN1993FlexureClass(Designfy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                double dblShearCap = 0;

                //Retrieve the plastic or elastic modulus of the section
                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        W = ThisSectionBaseProps.Wplyy;
                        if (BendClass == 3)
                            W = ThisSectionBaseProps.Welyy;
                        dblShearCap = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade,"Z", GammaM0).ToString());
                        break;
                    case BendingAxis.ZZ:
                        W = ThisSectionBaseProps.Wplzz;
                        if (BendClass == 3)
                            W = ThisSectionBaseProps.Welzz;
                        dblShearCap = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Y", GammaM0).ToString());
                        break;
                    default:
                        return "Please specify the bending axis";
                }

                W *= 1e9; //Modulus in mm cubed

                //Check if the design strength needs to be reduced for high shear
                double rho = 0;
                double WAv = 0;
                if (DesignShear > dblShearCap / 2 && DesignShear <= dblShearCap)
                {
                    rho = Math.Pow((2 * DesignShear / dblShearCap - 1), 2);
                    //Determine the bending modulus of the shear area
                    SectionTypes ThisSectionType = (SectionTypes)DetermineType(SectionDenomination);
                    switch (ThisSectionType)
                    {
                        case SectionTypes.I: //Dealing with I and H sections
                            StructIProp ThisISectionProps = (StructIProp)RetrieveISectionProps(SectionDenomination);
                            switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                            {
                                case BendingAxis.YY:
                                    double hw = ThisISectionProps.h - 2 * ThisISectionProps.tf;
                                    double tw = ThisISectionProps.tw;
                                    WAv = Math.Pow(hw, 2) * tw / 4;
                                    if (BendClass == 3)
                                        WAv = (Math.Pow(hw, 3) * tw / 12) / (hw / 2);
                                    break;
                                case BendingAxis.ZZ:
                                    double bf = ThisISectionProps.b;
                                    double tf = ThisISectionProps.tf;
                                    WAv = Math.Pow(bf, 2) * tf / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(bf, 3) * tf / 12) / (bf / 2));
                                    break;
                            }
                            break;
                        case SectionTypes.RHS:
                            StructRHSProp ThisRHSSectionProps = (StructRHSProp)RetrieveRHSProps(SectionDenomination);
                            double t = ThisRHSSectionProps.t;
                            switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                            {
                                case BendingAxis.YY:
                                    double hw = ThisRHSSectionProps.h - 2 * t;
                                    WAv = Math.Pow(hw, 2) * t / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(hw, 3) * t / 12) / (hw / 2));
                                    break;
                                case BendingAxis.ZZ:
                                    double bf = ThisRHSSectionProps.b - 2 * t;
                                    WAv = Math.Pow(bf, 2) * t / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(bf, 3) * t / 12) / (bf / 2));
                                    break;
                            }
                            break;
						case SectionTypes.CHS:
							StructCHSProp ThisCHSSectionProps = (StructCHSProp)RetrieveCHSProps(SectionDenomination);
							WAv = 2 * ThisCHSSectionProps.A * Math.PI;
							break;
                        case SectionTypes.BOX:
                            StructWeldedBoxProp ThisBOXSectionProps = (StructWeldedBoxProp)RetrieveWELDEDBOXProps(SectionDenomination);
                            switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                            {
                                case BendingAxis.YY:
                                    double hw = ThisBOXSectionProps.h;
                                    double tw = ThisBOXSectionProps.tw;
                                    WAv = Math.Pow(hw, 2) * tw / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(hw, 3) * tw / 12) / (hw / 2));
                                    break;
                                case BendingAxis.ZZ:
                                    double bf = ThisBOXSectionProps.b - 2 * ThisBOXSectionProps.tw;
                                    double tf = ThisBOXSectionProps.tf;
                                    WAv = Math.Pow(bf, 2) * tf / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(bf, 3) * tf / 12) / (bf / 2));
                                    break;
                            }
                            break;
                        case SectionTypes.INOWEB: //Dealing with I and H sections ignoring the web
                            StructINoWebProp ThisINoWebSectionProps = (StructINoWebProp)RetrieveINoWebSectionProps(SectionDenomination);
                            switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                            {
                                case BendingAxis.YY:
                                    return "Section can not carry shear in this direction";
                                case BendingAxis.ZZ:
                                    double bf = ThisINoWebSectionProps.b;
                                    double tf = ThisINoWebSectionProps.tf;
                                    WAv = Math.Pow(bf, 2) * tf / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(bf, 3) * tf / 12) / (bf / 2));
                                    break;
                            }
                            break;
						case SectionTypes.ICS: //Dealing with I and H sections ignoring the web
							StructIStiffWebProp ThisIStiffWebSectionProps = (StructIStiffWebProp)RetrieveIStiffWebSectionProps(SectionDenomination);
							switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
							{
								case BendingAxis.YY:
									double hw = ThisIStiffWebSectionProps.h - 2 * ThisIStiffWebSectionProps.tf;
									double tw = ThisIStiffWebSectionProps.tw;
									WAv = Math.Pow(hw, 2) * tw / 4;
									if (BendClass == 3)
										WAv = (Math.Pow(hw, 3) * tw / 12) / (hw / 2);
									break;
								case BendingAxis.ZZ:
									double bf = ThisIStiffWebSectionProps.b;
									double tf = ThisIStiffWebSectionProps.tf;
									double wStiff = ThisIStiffWebSectionProps.wStiff;
									double tStiff = ThisIStiffWebSectionProps.tStiff;
									WAv = Math.Pow(bf, 2) * tf / 2 + Math.Pow(wStiff,2) * tStiff / 4;
									if (BendClass == 3)
										WAv = ((2 * Math.Pow(bf, 3) * tf + Math.Pow(wStiff, 3) * tStiff) / 12) / (Math.Max(bf, wStiff) / 2);
									break;
							}
							break;
                        case SectionTypes.BOXNOWEB:
                            StructWeldedBoxNoWebsProp ThisBOXNoWebsSectionProps = (StructWeldedBoxNoWebsProp)RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
                            switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                            {
                                case BendingAxis.YY:
                                    double hw = ThisBOXNoWebsSectionProps.h;
                                    double tw = ThisBOXNoWebsSectionProps.tw;
                                    WAv = Math.Pow(hw, 2) * tw / 2;
                                    if (BendClass == 3)
                                        WAv = 2 * ((Math.Pow(hw, 3) * tw / 12) / (hw / 2));
                                    break;
                                case BendingAxis.ZZ:
                                    return "Section can not carry shear in this direction"; 
                            }
                            break;
                        default:
                            throw new CustomExceptions.SectionNotYetHandledException();
                    }
                }
                else if (DesignShear > dblShearCap)
                    return "Shear failure";

                WAv *= 1e9; //Modulus in mm cubed

                double dblMomentCapacity = ((W - rho * WAv) * Designfy / GammaM0) / 1e6; //Bending capacity in kNm
                if (dblMomentCapacity > (W * Designfy / GammaM0) / 1e6)
                    dblMomentCapacity = (W * Designfy / GammaM0) / 1e6;

                return dblMomentCapacity;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
#if(EXCELDNA)
        /*
         * Plastic shear resistance calculations according to BS:EN1993-1 cl. 6.2.6
         * Does not include the effects of torsion yet
         */
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Shear_Resistance", Description = "Section shear resistance in kN (EC3 cl. 6.2.6)")]
        public static object CrossSectionPlasticShearResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
			[ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "Axis", Description = "Axis parallel to the shear force, ''Y'' (major axis) or ''Z'' (minor axis)")] string ShearForceParallelAxis,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0)
# else
        
        public static object CrossSectionPlasticShearResistance(string SectionDenomination,
			 object SteelGrade,
             string ShearForceParallelAxis,
             double GammaM0)

#endif
        {
            try
            {
                IBaseProp ThisSectionBaseProps = (IBaseProp)RetrieveSectionProps(SectionDenomination);

                //Checks GammaM0
                if (GammaM0 == 0)
                    GammaM0 = 1;

				//Determine design yield stress
				double Designfy;
				if (!double.TryParse(SteelGrade.ToString(), out Designfy))
					Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());

                switch ((ShearAxis)Enum.Parse(typeof(ShearAxis), ShearForceParallelAxis.ToUpper()))
                {
                    case ShearAxis.Y:
						double Av = ThisSectionBaseProps.getEN1993ShearArea(Designfy).Avzz * 1e6; //in mm
                        return (Av * (Designfy / Math.Sqrt(3)) / GammaM0) / 1e3;
                    case ShearAxis.Z:
						Av = ThisSectionBaseProps.getEN1993ShearArea(Designfy).Avyy * 1e6; //in mm
                        return (Av * (Designfy / Math.Sqrt(3)) / GammaM0) / 1e3;
                    default:
                        return "Please specify bending axis";
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /*
         * Torsion resistance calculations according to BS:EN1993-1 cl. 6.2.7
         * Coming soon to an Excel computer near you. Not implemented yet.
         */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Torsion_Resistance", Description = "Section torsion resistance kNm (EC3 cl. 6.2.7)")]
        public static object CrossSectionTorsionResitance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "Bending axis", Description = "Axis of bending, ''YY'' or ''ZZ''")] string Axis,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. Taken as 1 by default")] double GammaM0)
#else
    public static object CrossSectionTorsionResitance(string SectionDenomination,
            object SteelGrade,
             string Axis,
             double GammaM0)

    
#endif
    {
            try
            {
                return "Not implemented yet";
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /*
         * Combined bending and axial checks to BS:EN1993-1 cl. 6.2.9
         * These are the cross-section capacity checks. Currently only implemented for class 1 and 2 I sections as these are handled explicitly in the code
         * and are simpler. Class 3 and 4 sections have to undergo different checks.
         * The checks depend on whether the sections work under high shear or not.(see end of code)
		 * There are two versions of the function, one verbose and the other not.
         */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Combined_Axial_And_Bending_Check", Description = "Section utilisation under combined axial load and bending (EC3 cl. 6.2.9)")]
        public static object CombinedAxialAndBendingChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
			[ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
            [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd,
            [ExcelArgument(Name = "VyEd", Description = "(Optional) Shear force parallel to the major axis in kN. (Default = 0kN)")] double VyEd,
            [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double VzEd,
            [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MyEd,
            [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MzEd,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. Calculated according to EC3 cl. 5.6 by default")] int CompClass,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. Calculated according to EC3 cl. 5.6 by default")] int BendClass,
#else
            
        public static object CombinedAxialAndBendingChecks(string SectionDenomination,
			object SteelGrade,
            double NEd,
            double VyEd,
            double VzEd,
            double MyEd,
            double MzEd,
            double GammaM0,
            int CompClass,
            int BendClass,
            string strCheckType)
#endif

        {
			return (CombinedAxialAndBendingChecks(false,SectionDenomination,SteelGrade,NEd,VyEd,VzEd,MyEd,MzEd,GammaM0,
            CompClass,BendClass,strCheckType));

        }

#if(EXCELDNA)
		[ExcelFunction(Category = "EXP EC3 Functions Verbose", Name = "vSection_Combined_Axial_And_Bending_Check", Description = "Section utilisation under combined axial load and bending (EC3 cl. 6.2.9)")]
		public static object vCombinedAxialAndBendingChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
			[ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade,
			[ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd,
			[ExcelArgument(Name = "VyEd", Description = "(Optional) Shear force parallel to the major axis in kN. (Default = 0kN)")] double VyEd,
			[ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double VzEd,
			[ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MyEd,
			[ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MzEd,
			[ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0,
			[ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. Calculated according to EC3 cl. 5.6 by default")] int CompClass,
			[ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. Calculated according to EC3 cl. 5.6 by default")] int BendClass,
			[ExcelArgument(Name = "Check type", Description = "(Optional) ''ADVANCED'' or ''CONSERVATIVE''. ''ADVANCED'' carries out non-linear checks even for high shear. (Default = ''CONSERVATIVE'')")] string strCheckType)
#else
           public static object vCombinedAxialAndBendingChecks(string SectionDenomination,
			object SteelGrade,
			double NEd,
			double VyEd,
			double VzEd,
			double MyEd,
			double MzEd,
			double GammaM0,
			int CompClass,
			int BendClass,
			string strCheckType)


#endif
        {
			return (CombinedAxialAndBendingChecks(true, SectionDenomination, SteelGrade, NEd, VyEd, VzEd, MyEd, MzEd, GammaM0,
			CompClass, BendClass, strCheckType));

		}
        /*
         * 
         * Buckling resistance checks of members. First compression then torsion then combined checks
         * Note this is coded for catalogue and GWA sections so far. Needs to be updated so that the functions
         * can be used for custom sections, i.e. with the section properties being input manually.
         * 
         * Compression buckling resistance cl.6.3.1
         * 
         */
        //Compression buckling curve
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Buckling_Curve", Description = "Buckling curve for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingCurve([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
#else
           public static object CompressionBucklingCurve(string SectionDenomination,
               string SteelGrade,
               string Axis)
#endif
        {
            try
            {
                StructBaseProperties ThisSectionProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionProps;
                EN1993SteelGrade enSteelGrade = (EN1993SteelGrade)Enum.Parse(typeof(EN1993SteelGrade), SteelGrade.ToUpper());
                EN1993CompressionBucklingCurves BucklingCurves = baseProp.getEN1993CompressionBucklingCurves(enSteelGrade);

                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        return BucklingCurves.majorAxis.ToString();
                    case BendingAxis.ZZ:
                        return BucklingCurves.minorAxis.ToString();
                }
                throw new CustomExceptions.SectionNotYetHandledException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Buckling imperfection factor
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Imperfection_Factor", Description = "Imperfection factor for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingImperfectionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
#else
           public static object CompressionBucklingImperfectionFactor(string SectionDenomination,
               string SteelGrade,
                string Axis)
#endif
        {
            try
            {
                StructBaseProperties ThisSectionProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
                string strBucklingCurve = CompressionBucklingCurve(SectionDenomination, SteelGrade, Axis).ToString(); 
                switch((EN1993CompressionBucklingCurve)Enum.Parse(typeof(EN1993CompressionBucklingCurve),strBucklingCurve.ToLower()))
                {
                    case EN1993CompressionBucklingCurve.a0:
                        return 0.13;
                    case EN1993CompressionBucklingCurve.a:
                        return 0.21;
                    case EN1993CompressionBucklingCurve.b:
                        return 0.34;
                    case EN1993CompressionBucklingCurve.c:
                        return 0.49;
                    case EN1993CompressionBucklingCurve.d:
                        return 0.76;
                }
                throw new CustomExceptions.SectionNotYetHandledException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Critical elastic buckling force
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Elastic_Critical_Force", Description = "Elastic critical compression force for compression buckling in kN (EC 3 cl. 6.3.1.2)")]
        public static object CompressionBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E)
#else
           public static object CompressionBucklingCriticalElasticForce(string SectionDenomination,
           double Lcr,
           string Axis,
           double E)
    
    
#endif
    {
            try
            {
                if(E == 0)
                    E = 210000;

                Lcr *= 1000;
                double I = 0;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);

                switch((BendingAxis)Enum.Parse(typeof(BendingAxis),Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        I = ThisSectionBaseProps.Iyy * 1e12;
                        return (Math.Pow(Math.PI, 2) * E * I / Math.Pow(Lcr, 2)) / 1e3;
                    case BendingAxis.ZZ:
                        I = ThisSectionBaseProps.Izz * 1e12;
                        return (Math.Pow(Math.PI, 2) * E * I / Math.Pow(Lcr, 2)) / 1e3;
                }

                throw new CustomExceptions.GeneralException("Elastic critical buckling force error");

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Non-dimensional slenderness
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_NonDimensional_Slenderness", Description = "Non-dimensional slenderness for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingNonDimensionalSlenderness([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string.")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis,
            [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
#else
           public static object CompressionBucklingNonDimensionalSlenderness(string SectionDenomination,
            string SteelGrade,
            string Axis,
            double Lcr,
            double E,
            int CompClass)
    
#endif
    {
            try
            {
                if (E ==0)
                    E = 210000;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
                double Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());

                double Ncr = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, Lcr, Axis, E).ToString()) * 1e3;

                    if (CompClass < 0 || CompClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (CompClass == 0)
						CompClass = baseProp.getEN1993CompressionClass(Designfy);
                    if (CompClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }


                
                return Math.Sqrt(ThisSectionBaseProps.A * 1e6 * Designfy / Ncr);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Compression buckling reduction factor
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Reduction_Factor", Description = "Compression buckling resistance reduction factor (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingReductionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis,
            [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")]int CompClass)
#else
           public static object CompressionBucklingReductionFactor(string SectionDenomination,
               string SteelGrade,
               string Axis,
               double Lcr,
               double E,
               int CompClass)
#endif
        {
            try
            {
                if(E == 0)
                    E = 210000;

                double alpha = double.Parse(CompressionBucklingImperfectionFactor(SectionDenomination, SteelGrade, Axis).ToString());

                double LambdaBar = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, Axis, Lcr, E,CompClass).ToString());

                double Fi = 0.5 * (1 + alpha * (LambdaBar - 0.2) + Math.Pow(LambdaBar, 2));

                double Chi = 1 / (Fi + Math.Sqrt(Math.Pow(Fi, 2) - Math.Pow(LambdaBar, 2)));
                if (Chi > 1)
                    return 1;
                return Chi;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Compression buckling design resistance
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Design_Resistance", Description = "Design compression buckling resistance in kN (EC3 cl. 6.3.1.1)")]
        public static object CompressionBucklingDesignResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis,
            [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")]int CompClass)
#else
           public static object CompressionBucklingDesignResistance(string SectionDenomination,
              string SteelGrade,
              string Axis,
              double Lcr,
              double E,
              double GammaM1,
              int CompClass)
#endif
        {
            try
            {
                if(E == 0)
                    E = 210000;
                if (GammaM1 == 0)
                    GammaM1 = 1;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);

                double Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());

                double Chi = double.Parse(CompressionBucklingReductionFactor(SectionDenomination, SteelGrade,Axis, Lcr, E,CompClass).ToString());
                
                return ((Chi * ThisSectionBaseProps.A * 1e6 * Designfy) / GammaM1) / 1e3;                        

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /*
         *
         * Lateral torsional buckling resistance cl.6.3.2
         * 
        */
        //LTB Buckling curve
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_buckling_curve", Description = "Lateral torsional buckling buckling curve (EC3 cl. 6.3.2.2)")]
        public static object LTBBucklingCurve([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination)
#else
        public static object LTBBucklingCurve(string SectionDenomination)
#endif
        {
            try
            {
                IBaseProp ThisSectionBaseProps = (IBaseProp)RetrieveSectionProps(SectionDenomination);
                EN1993LTBBucklingCurve LTBBucklingCurve = ThisSectionBaseProps.getEN1993LTBBucklingCurve();
                return LTBBucklingCurve.LTBBucklingCurve.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //LTB Buckling imperfection factor
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Buckling_Imperfection_Factor", Description = "Imperfection factor for lateral torsional buckling (EC3 cl. 6.3.2.2)")]
        public static object LTBBucklingImperfectionFactor([ExcelArgument(Name = "Section name", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade)
#else
        public static object LTBBucklingImperfectionFactor(string SectionDenomination,
            string SteelGrade)
#endif
        {
            try
            {
                SectionTypes ThisSection = DetermineType(SectionDenomination);
                string strBucklingCurve = LTBBucklingCurve(SectionDenomination).ToString();
                switch ((EN1993LTBBucklingCurves)Enum.Parse(typeof(EN1993LTBBucklingCurves), strBucklingCurve.ToLower()))
                {
                    case EN1993LTBBucklingCurves.a:
                        return 0.21;
                    case EN1993LTBBucklingCurves.b:
                        return 0.34;
                    case EN1993LTBBucklingCurves.c:
                        return 0.49;
                    case EN1993LTBBucklingCurves.d:
                        return 0.76;
                    default:
                        throw new CustomExceptions.GeneralException("Buckling factor could not be determined");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //LTB buckling critical elastic moment
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Critical_Elastic_Moment", Description = "Elastic critical moment for LTB in kNm (cl. 6.3.2.2)")]
        public static object LTBCriticalMoment([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu)
#else
        public static object LTBCriticalMoment(string SectionDenomination,
            double Lcr,
            double C1,
            double E,
            double nu)
#endif
        {

            try
            {

                if (E == 0)
                    E = 210000;
                if (nu == 0)
                    nu = 0.297;

                Lcr = Lcr * 1000;
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section is not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }

                double G = E / (2 * (1 + nu));
                double Izz = ThisSectionBaseProps.Izz * 1e12;
                double Iw = ThisSectionBaseProps.Iw * 1e18;
                double J = ThisSectionBaseProps.It * 1e12;

                /*
                 * Note this is not the most complete expression as it does not take into account eccentricity between the shear
                 * centre of the section and the point of application of the load. For the complete expression of the critical moment
                 * see access steel document SN030a-EN-EU
                 */
                double Mcr1 = (Math.Pow(Math.PI, 2) * E * Izz / Math.Pow(Lcr, 2));
                double Mcr2 = Math.Sqrt(Iw / Izz + (Math.Pow(Lcr, 2) * G * J) / (Math.Pow(Math.PI, 2) * E * Izz));

                return C1 * Mcr1 * Mcr2 / 1e6;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //LTB Non-dimensional slenderness
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_NonDimensional_Slenderness", Description = "Determines the non-dimensional slenderness (cl. 6.3.2)")]
        public static object LTBNonDimensionalSlenderness([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
#else
        public static object LTBNonDimensionalSlenderness(string SectionDenomination,
             string SteelGrade,
            double Lcr,
            double C1,
            double E,
            double nu,
            int BendClass)
#endif
        {
            try
            {

                if (E == 0)
                    E = 210000;
                if (nu == 0)
                    nu = 0.297;

                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                double Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;

				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }

                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
						BendClass = baseProp.getEN1993FlexureClass(fy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }
                
                double Wyy = ThisSectionBaseProps.Wplyy * 1e9;
                if (BendClass == 3)
                    Wyy = ThisSectionBaseProps.Welyy * 1e9;
                
                double Mcr = double.Parse(LTBCriticalMoment(SectionDenomination,Lcr,C1,E,nu).ToString()) * 1e6;
                return Math.Sqrt(Wyy * Designfy / Mcr);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //LTB reduction factor
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Reduction_Factor", Description = "Determines the LTB buckling reduction factor")]
        public static object LTBReductionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1s,
            [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
#else
        public static object LTBReductionFactor(string SectionDenomination,
            string SteelGrade,
            double Lcr,
            double C1s,
            double GammaM1,
            double E,
            double nu,
            int BendClass)
#endif
        {
            try
            {

                if (GammaM1 == 0)
                    GammaM1 = 1;
                if (E == 0)
                    E = 210000;
                if (nu == 0)
                    nu = 0.297;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }

                double AlphaLT = double.Parse(LTBBucklingImperfectionFactor(SectionDenomination, SteelGrade).ToString());
                double LambdaLT = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination, SteelGrade, Lcr, C1s, E, nu, BendClass).ToString());
                double FiLT = 0.5 * (1 + AlphaLT * (LambdaLT - 0.2) + Math.Pow(LambdaLT, 2));

                double ChiLT = 1 / (FiLT + Math.Sqrt(Math.Pow(FiLT, 2) - Math.Pow(LambdaLT, 2)));
                if (ChiLT > 1)
                    return 1;
                return ChiLT;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Design LTB buckling resistance
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Design_Buckling_Resistance", Description = "Determines the LTB buckling design resistance Mb,rd in kNm (EC3 cl.6.3.2 eq.6.55)")]
        public static object LTBDesignBucklingResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1,
            [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu,
            [ExcelArgument(Name = "Bend. class", Description = "((Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
#else
        public static object LTBDesignBucklingResistance(string SectionDenomination,
            string SteelGrade,
            double Lcr,
            double C1,
            double GammaM1,
            double E,
            double nu,
            int BendClass)
#endif
        {
            try
            {
                if (GammaM1 == 0)
                    GammaM1 = 1;
                if (E == 0)
                    E = 210000;
                if (nu == 0)
                    nu = 0.297;

                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                double Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }

                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
						BendClass = baseProp.getEN1993FlexureClass(fy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                double ChiLT = double.Parse(LTBReductionFactor(SectionDenomination,SteelGrade,Lcr,C1,GammaM1,E,nu,BendClass).ToString());

                double Wyy = ThisSectionBaseProps.Wplyy * 1e9;
                if (BendClass == 3)
                    Wyy = ThisSectionBaseProps.Welyy * 1e9;
                
                return (ChiLT * Wyy * Designfy / GammaM1) / 1e6;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /*
         * 
         * Interaction checks cl. 6.3.3
         * Is there a way to program in the C factors?
         * 
        */
        //K factors
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Buckling_Interaction_K_Factors", Description = "Determines the K factors used in the combined buckling and axial bending checks")]
        public static object OutputKfactors([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd,
            [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double DesignShear,
            [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MEdY,
            [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")]double MEdZ,
            [ExcelArgument(Name = "LcrY (m)", Description = "Buckling length about YY axis in m. (See BS5950 for reference)")] double LcrY,
            [ExcelArgument(Name = "LcrZ (m)", Description = "Buckling length about ZZ axis in m. (See BS5950 for reference)")] double LcrZ,
            [ExcelArgument(Name = "LcrLT (m)", Description = "Lateral torsion buckling length in m. (See BS5950 for reference)")] double LcrLT,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1,
            [ExcelArgument(Name = "CmY", Description = "Factor CmY depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmY,
            [ExcelArgument(Name = "CmZ", Description = "Factor CmZ depending on the moment distribution along the beam. See blue book or Access Steel for reference")]double CmZ,
            [ExcelArgument(Name = "CmLT", Description = "Factor CmLT depending on the moment distribution along the beam. See blue book or Access Steel for reference")]double CmLT,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) GammaM0 safety factor. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")]int CompClass,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass,
            [ExcelArgument(Name = "Kfactor", Description = "K factor to output: ''YY'', ''ZZ'', ''YZ'' or ''ZY''")] string KFactor)
#else
        public static object OutputKfactors(string SectionDenomination,
            string SteelGrade,
            double NEd,
            double DesignShear,
            double MEdY,
            double MEdZ,
            double LcrY,
            double LcrZ,
            double LcrLT,
            double C1,
            double CmY,
            double CmZ,
            double CmLT,
            double GammaM0,
            double GammaM1,
            double E,
            double nu,
            int CompClass,
            int BendClass,
            string KFactor)
#endif
        {
            try
            {
                SectionTypes ThisSectionType = (SectionTypes)DetermineType(SectionDenomination);
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
                if (GammaM0 == 0)
                    GammaM0 = 1;
                if (GammaM1 == 0)
                    GammaM1 = 1;

                double fy = double.Parse(YieldStrength(SteelGrade).ToString());

                if (CompClass < 0 || CompClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (CompClass == 0)
						CompClass = baseProp.getEN1993CompressionClass(fy);
                    if (CompClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
						BendClass = baseProp.getEN1993FlexureClass(fy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                double NbRdy = double.Parse(CompressionBucklingDesignResistance(SectionDenomination,
                    SteelGrade, "YY", LcrY, E, GammaM1, CompClass).ToString());
                double NbRdz = double.Parse(CompressionBucklingDesignResistance(SectionDenomination,
                    SteelGrade, "ZZ", LcrZ,E,GammaM1,CompClass).ToString());
                double MbRdy = double.Parse(LTBDesignBucklingResistance(SectionDenomination,SteelGrade,LcrLT,C1,GammaM1,E,nu,BendClass).ToString());
                double MzRk = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", DesignShear, GammaM0, BendClass).ToString());
               
                double LambdaBarY = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, 
                    SteelGrade, "YY", LcrY, E, CompClass).ToString());
                double LambdaBarZ = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, 
                    SteelGrade, "ZZ", LcrZ, E,CompClass).ToString());
                double LambdaLT = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination,
                    SteelGrade,LcrLT,C1,E,nu,BendClass).ToString());

                switch(KFactor.ToUpper())
                {
                    case "YY":
                        return double.Parse(FactorKyy(NEd, NbRdy, CmY, LambdaBarY, GammaM1, BendClass).ToString());
                    case "ZZ":
                        return double.Parse(FactorKzz_AppBB(NEd, NbRdz, CmZ, LambdaBarZ, GammaM1, BendClass, ThisSectionType).ToString());
                    case "YZ":
                        return double.Parse(FactorKyz_AppBB(NEd, NbRdz, CmZ, LambdaBarZ, GammaM1, BendClass, ThisSectionType).ToString());
                    case "ZY":
                        double Ncrz = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination,LcrZ,"ZZ",E).ToString());
                        double NcrTF = double.Parse(TorsionFlexuralBucklingCriticalElasticForce(SectionDenomination, LcrZ,LcrLT,E,nu,"ZZ").ToString());
                        double LambdaLTLim = 0.2 * Math.Sqrt(C1) * Math.Pow((1 - NEd / Ncrz) * (1 - NEd / NcrTF), 1 / 4);
						if (LambdaLTLim < Math.Sqrt(C1) * LambdaLT)
							return double.Parse(FactorKzyTorsion_AppBB(NEd, NbRdy, CmLT, LambdaBarZ, GammaM1, baseProp.getEN1993FlexureClass(fy)).ToString());
                        return double.Parse(FactorKzy_AppBB(NEd, NbRdy, CmY, LambdaBarY, GammaM1, baseProp.getEN1993FlexureClass(fy)).ToString());
                    default:
                        throw new CustomExceptions.GeneralException("Unrecognised K factor");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        private static double FactorKyy(double Ned, double NbRdy, double Cmy, double LambdaBarY,
            double GammaM1, int SectionClass)
        {
            double Kyy = 0;
            if (SectionClass == 1 || SectionClass == 2)
            {
                Kyy = Cmy * (1 + (LambdaBarY - 0.2) * Ned / NbRdy);
                if (Kyy > Cmy * (1 + 0.8 * Ned / NbRdy))
                    return Cmy * (1 + 0.8 * Ned / NbRdy);
                return Kyy;
            }
            Kyy = Cmy * (1 + 0.6 * LambdaBarY * Ned / NbRdy);
            if (Kyy > Cmy * (1 + 0.6 * Ned / NbRdy))
                return Cmy * (1 + 0.6 * Ned / NbRdy);
            return Kyy;
        }
        private static double FactorKyz_AppBB(double Ned, double NbRdz, double CmZ, double LambdaBarZ,
            double GammaM1, int SectionClass, SectionTypes SectionType)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                return 0.6 * FactorKzz_AppBB(Ned, NbRdz, CmZ, LambdaBarZ, GammaM1, SectionClass, SectionType);
            }
            return FactorKzz_AppBB(Ned, NbRdz, CmZ, LambdaBarZ, GammaM1, SectionClass, SectionType);
        }
        private static double FactorKzy_AppBB(double Ned, double NbRdy, double Cmy, double LambdaBarY,
            double GammaM1, int SectionClass)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                return 0.6 * FactorKyy(Ned, NbRdy, Cmy, LambdaBarY, GammaM1, SectionClass);
            }
            return 0.8 * FactorKyy(Ned, NbRdy, Cmy, LambdaBarY, GammaM1, SectionClass);
        }
        private static double FactorKzyTorsion_AppBB(double Ned, double NbRdz, double CmLT, double LambdaBarZ,
            double GammaM1, int SectionClass)
        {
            double Kzy = 1 - 0.05 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
            if (SectionClass == 1 || SectionClass == 2)
            {
                Kzy = 1 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
				if (LambdaBarZ < 0.4)
				{
					Kzy = 0.6 + LambdaBarZ;
					if (Kzy > 1 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz))
						Kzy = 1 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
					return Kzy;
				}
                if (Kzy < 1 - 0.1 * Ned / ((CmLT - 0.25) * NbRdz))
                    return 1 - 0.1 * Ned / ((CmLT - 0.25) * NbRdz);
                return Kzy;
            }
            if (Kzy < 1 - 0.05 * Ned / ((CmLT - 0.25) * NbRdz))
                return 1 - 0.05 * Ned / ((CmLT - 0.25) * NbRdz);
            return Kzy;

        }
        private static double FactorKzz_AppBB(double Ned, double NbRdz, double CmZ, double LambdaBarZ,
            double GammaM1, int SectionClass, SectionTypes SectionType)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                double KzzI = CmZ * (1 + (2 * LambdaBarZ - 0.6) * Ned / NbRdz);
                if (KzzI > CmZ * (1 + 1.4 * Ned / NbRdz))
                    KzzI = CmZ * (1 + 1.4 * Ned / NbRdz);
                double KzzBox = CmZ * (1 + (LambdaBarZ - 0.2) * Ned / NbRdz);
                if (KzzBox > CmZ * (1 + 0.8 * Ned / NbRdz))
                    KzzBox = CmZ * (1 + 0.8 * Ned / NbRdz);
                switch(SectionType)
                {
                    case SectionTypes.I:
					case SectionTypes.INOWEB:
					case SectionTypes.ICS:
                        return KzzI;
                    case SectionTypes.RHS:
					case SectionTypes.BOX:
					case SectionTypes.BOXNOWEB:
                        return KzzBox;
                    default:
                        return Math.Max(KzzI, KzzBox);
                }
            }
            double Kzz = CmZ * (1 + 0.6 * LambdaBarZ * Ned / NbRdz);
            if (Kzz > CmZ * (1 + 0.6 * Ned / NbRdz))
                return CmZ * (1 + 0.6 * Ned / NbRdz);
            return Kzz;
        }
        /*
         * Interaction checks
        */
#if(EXCELDNA)
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Buckling_Combined_Axial_And_Bending_Checks", Description = "Section utilisation with regards to buckling under combined bending and axial loads")]
        public static object BendingAxialInteractionChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade,
            [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd,
            [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double DesignShear,
            [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MEdY,
            [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")]double MEdZ,
            [ExcelArgument(Name = "LcrY (m)", Description = "Buckling length about YY axis in m. (See BS5950 for reference)")] double LcrY,
            [ExcelArgument(Name = "LcrZ (m)", Description = "Buckling length about ZZ axis in m. (See BS5950 for reference)")] double LcrZ,
            [ExcelArgument(Name = "LcrLT (m)", Description = "Lateral torsion buckling length in m. (See BS5950 for reference)")] double LcrLT,
            [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1,
            [ExcelArgument(Name = "CmY", Description = "Factor CmY depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmY,
            [ExcelArgument(Name = "CmZ", Description = "Factor CmZ depending on the moment distribution along the beam. See blue book or Access Steel for reference")]double CmZ,
            [ExcelArgument(Name = "CmLT", Description = "Factor CmLT depending on the moment distribution along the beam. See blue book or Access Steel for reference")]double CmLT,
            [ExcelArgument(Name = "GammaM0", Description = "(Optional) GammaM0 safety factor. (Default = 1)")] double GammaM0,
            [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1,
            [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E,
            [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu,
            [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")]int CompClass,
            [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass,
            [ExcelArgument(Name = "Check", Description = "Check to carry out: 1 for eq. (6.61), 2 for eq. (6.62)")] int Check1Or2)
#else
        public static object BendingAxialInteractionChecks(string SectionDenomination,
           string SteelGrade,
            double NEd,
            double DesignShear,
            double MEdY,
            double MEdZ,
            double LcrY,
            double LcrZ,
            double LcrLT,
            double C1,
            double CmY,
            double CmZ,
            double CmLT,
            double GammaM0,
            double GammaM1,
            double E,
            double nu,
            int CompClass,
            int BendClass,
            int Check1Or2)
#endif
        {
            try
            {
                SectionTypes ThisSectionType = (SectionTypes)DetermineType(SectionDenomination);
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
                if (GammaM0 == 0)
                    GammaM0 = 1;
                if (GammaM1 == 0)
                    GammaM1 = 1;

                double fy = double.Parse(YieldStrength(SteelGrade).ToString());

                if (CompClass < 0 || CompClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (CompClass == 0)
						CompClass = baseProp.getEN1993CompressionClass(fy);
                    if (CompClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
						BendClass = baseProp.getEN1993FlexureClass(fy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                double NbRdy = double.Parse(CompressionBucklingDesignResistance(SectionDenomination,
                    SteelGrade, "YY", LcrY, E, GammaM1, CompClass).ToString());
                double NbRdz = double.Parse(CompressionBucklingDesignResistance(SectionDenomination,
                    SteelGrade, "ZZ", LcrZ,E,GammaM1,CompClass).ToString());
                double MbRdy = double.Parse(LTBDesignBucklingResistance(SectionDenomination,SteelGrade,LcrLT,C1,GammaM1,E,nu,BendClass).ToString());
                double MzRk = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", DesignShear, GammaM0, BendClass).ToString());
               
                double LambdaBarY = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, 
                    SteelGrade, "YY", LcrY, E, CompClass).ToString());
                double LambdaBarZ = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, 
                    SteelGrade, "ZZ", LcrZ, E,CompClass).ToString());
                double LambdaLT = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination,
                    SteelGrade,LcrLT,C1,E,nu,BendClass).ToString());

                double kyy = double.Parse(FactorKyy(NEd, NbRdy, CmY, LambdaBarY, GammaM1, BendClass).ToString());
                double kzz = double.Parse(FactorKzz_AppBB(NEd, NbRdz, CmZ, LambdaBarZ, GammaM1, BendClass, ThisSectionType).ToString());
                double kyz = double.Parse(FactorKyz_AppBB(NEd, NbRdz, CmZ, LambdaBarZ, GammaM1, BendClass, ThisSectionType).ToString());

                double Ncrz = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination,LcrZ, "ZZ",E).ToString());
                double NcrTF = double.Parse(TorsionFlexuralBucklingCriticalElasticForce(SectionDenomination, LcrZ,LcrLT,E,nu,"ZZ").ToString());
                double LambdaLTLim = 0.2 * Math.Sqrt(C1) * Math.Pow((1 - NEd / Ncrz) * (1 - NEd / NcrTF), 1 / 4);

                double kzy = double.Parse(FactorKzy_AppBB(NEd, NbRdy, CmY, LambdaBarY, GammaM1, baseProp.getEN1993FlexureClass(fy)).ToString());
				if (LambdaLTLim < Math.Sqrt(C1) * LambdaLT)
					kzy = double.Parse(FactorKzyTorsion_AppBB(NEd, NbRdy, CmLT, LambdaBarZ, GammaM1, baseProp.getEN1993FlexureClass(fy)).ToString());


                if (Check1Or2 == 1)
                    return NEd / NbRdy + kyy * MEdY / MbRdy + kyz * MEdZ / (MzRk / GammaM1);
                else
                    return NEd / NbRdz + kzy * MEdY / MbRdy + kzz * MEdZ / (MzRk / GammaM1);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //
        //Torsion buckling checks
        //
        //Critical elastic torsion buckling force
#if(EXCELDNA)
        [ExcelFunction(Category = "EC3 test functions", Name = "Torsion_Buckling_Elastic_Critical_Force", Description = "Determines the elastic critical torsion buckling load Ncr,T in kN (cl. 6.3.1)")]
        public static object TorsionBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "LcrT (m)", Description = "Buckling length in m. Generally taken as the member length")] double LcrT,
            [ExcelArgument(Name = "E (N/sq.mm)", Description = "Elastic modulus in N/sq.mm (210000N/sq.mm recommended in EC3)")] double E,
            [ExcelArgument(Name = "v", Description = "Poisson's ratio")] double nu)
#else
        public static object TorsionBucklingCriticalElasticForce(string SectionDenomination,
            double LcrT,
            double E,
            double nu)
#endif
        {
            try
            {
                LcrT *= 1000;

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }
                double iy = ThisSectionBaseProps.iy * 1e3;
                double iz = ThisSectionBaseProps.iz * 1e3;
                double i0 = Math.Pow(iy, 2) + Math.Pow(iz, 2);
                double G = E / (2 * (1 + nu));
                double Iw = ThisSectionBaseProps.Iw * 1e18;
                double IT = ThisSectionBaseProps.It * 1e12;
                double A = ThisSectionBaseProps.A * 1e6;
                return ((G * IT + Math.Pow(Math.PI, 2) * E * Iw / Math.Pow(LcrT, 2)) / i0) / 1e3;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //
        //Critical elastic torsion flexural buckling force
#if(EXCELDNA)
        [ExcelFunction(Category = "EC3 test functions", Name = "Torsion_Flexural_Buckling_Elastic_Critical_Force", Description = "Determines the elastic critical torsion-flexural buckling load Ncr,TF in kN (cl. 6.3.1)")]
        public static object TorsionFlexuralBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")]string SectionDenomination,
            [ExcelArgument(Name = "Lcr (m)", Description = "Compression buckling length in m. See BS5950 for reference")] double Lcr,
            [ExcelArgument(Name = "LcrT (m)", Description = "Torsional buckling length in m. See EN1993-1-3 for guidance")] double LcrT,
            [ExcelArgument(Name = "E (N/sq.mm)", Description = "Elastic modulus in N/sq.mm (210000N/sq.mm recommended in EC3)")] double E,
            [ExcelArgument(Name = "v", Description = "Poisson's ratio")] double nu,
            [ExcelArgument(Name = "Bending axis", Description = "Bending Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
#else
        public static object TorsionFlexuralBucklingCriticalElasticForce(string SectionDenomination,
            double Lcr,
            double LcrT,
            double E,
            double nu,
            string Axis)
#endif
        {
            try
            {

                
                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				IBaseProp baseProp = (IBaseProp)ThisSectionBaseProps;
				if (baseProp.SecSymmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new CustomExceptions.GeneralException("Section not doubly-symmetric");  //todo, work out how to calculate for non symmetric sections
                }
                
                double iy = ThisSectionBaseProps.iy * 1e3;
                double iz = ThisSectionBaseProps.iz * 1e3;
                double i0 = Math.Pow(iy, 2) + Math.Pow(iz, 2);
                double y0 = 0;
                double Beta = 1 - y0/i0;

                double Ncr = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, Lcr, Axis, E).ToString()) * 1e3;
                double NcrT = double.Parse(TorsionBucklingCriticalElasticForce(SectionDenomination, LcrT, E, nu).ToString()) * 1e3;

                return (Ncr / (2 * Beta) * (1 + NcrT / Ncr - Math.Sqrt(Math.Pow(1 - NcrT / Ncr, 2)
                    + 4 * Math.Pow(y0 / i0, 2) * NcrT / Ncr))) / 1e3;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}
