using ExcelDna.Integration;
using SectionCatalogue;
using SectionCatalogue.StructProperties;
using System;
namespace EC3Functions
{
    public static class EC3Functions
    {
        
        public static object CombinedAxialAndBendingChecks(bool verbose, string SectionDenomination, string SteelGrade, double NEd, double VyEd, double VzEd, double MyEd, double MzEd, double GammaM0, int CompClass, int BendClass, string strCheckType)
        {
            object result;
            try
            {
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                if (strCheckType.ToUpper() != "ADVANCED")
                {
                    strCheckType = "CONSERVATIVE";
                }
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                if (CompClass < 0 || CompClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (CompClass == 0)
                {
                    CompClass = structBaseProperties.getEN1993CompressionClass(num);
                }
                if (CompClass == 4)
                {
                    throw new Class4SectionException();
                }
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(num);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                int num2 = Math.Max(CompClass, BendClass);
                SectionTypes sectionTypes = DetermineType(SectionDenomination);
                double num3 = double.Parse(CrossSectionCompressionResistance(SectionDenomination, SteelGrade, GammaM0, CompClass).ToString()) * 1000.0;
                double num4 = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "YY", VzEd, GammaM0, BendClass).ToString()) * 1000000.0;
                double num5 = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", VyEd, GammaM0, BendClass).ToString()) * 1000000.0;
                double num6 = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Y", GammaM0).ToString()) * 1000.0;
                double num7 = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Z", GammaM0).ToString()) * 1000.0;
                NEd *= 1000.0;
                VyEd *= 1000.0;
                VzEd *= 1000.0;
                MyEd *= 1000000.0;
                MzEd *= 1000000.0;
                if (num2 < 3)
                {
                    if (NEd != 0.0 && MyEd == 0.0 && MzEd == 0.0)
                    {
                        if (verbose)
                        {
                            result = string.Concat(new object[]
							{
								"Ned / NRd : ",
								NEd,
								" / ",
								num3
							});
                        }
                        else
                        {
                            result = NEd / num3;
                        }
                    }
                    else
                    {
                        if (NEd == 0.0 && MyEd != 0.0 && MzEd == 0.0)
                        {
                            if (verbose)
                            {
                                result = string.Concat(new object[]
								{
									"MyEd / MyRd : ",
									MyEd,
									" / ",
									num4
								});
                            }
                            else
                            {
                                result = MyEd / num4;
                            }
                        }
                        else
                        {
                            if (NEd == 0.0 && MyEd == 0.0 && MzEd != 0.0)
                            {
                                if (verbose)
                                {
                                    result = string.Concat(new object[]
									{
										"MzEd / MzRd : ",
										MzEd,
										" / ",
										num5
									});
                                }
                                else
                                {
                                    result = MzEd / num5;
                                }
                            }
                            else
                            {
                                double num8 = num4;
                                double num9 = num5;
                                double num10 = 0.0;
                                double num11 = NEd / num3;
                                double num12 = structBaseProperties.A * 1000000.0;
                                switch (sectionTypes)
                                {
                                    case SectionTypes.I:
                                    case SectionTypes.INOWEB:
                                        {
                                            ISection structIProperties = RetrieveISectionProps(SectionDenomination);
                                            double num13 = structIProperties.h * 1000.0;
                                            double num14 = structIProperties.b * 1000.0;
                                            num10 = structIProperties.tf * 1000.0;
                                            double num15 = structIProperties.tw * 1000.0;
                                            double num16 = num13 - 2.0 * num10;
                                            double num17 = (num12 - 2.0 * num14 * num10) / num12;
                                            if (num17 > 0.5)
                                            {
                                                num17 = 0.5;
                                            }
                                            if (NEd > 0.25 * num3 || NEd > 0.5 * num16 * num15 * num / GammaM0)
                                            {
                                                num8 = num4 * (1.0 - num11) / (1.0 - 0.5 * num17);
                                            }
                                            if (NEd <= num16 * num15 * num / GammaM0)
                                            {
                                                goto IL_713;
                                            }
                                            num9 = num5;
                                            if (num11 > num17)
                                            {
                                                num9 = num5 * (1.0 - Math.Pow((num11 - num17) / (1.0 - num17), 2.0));
                                                goto IL_713;
                                            }
                                            goto IL_713;
                                        }
                                    case SectionTypes.RHS:
                                        {
                                            RectangularHollowSection structRHSProperties = RetrieveRHSProps(SectionDenomination);
                                            double num14 = structRHSProperties.b * 1000.0;
                                            double num13 = structRHSProperties.h * 1000.0;
                                            double num18 = structRHSProperties.t * 1000.0;
                                            double num16 = num13 - 2.0 * num10;
                                            double num19 = (num12 - 2.0 * num14 * num18) / num12;
                                            double num20 = (num12 - 2.0 * num13 * num18) / num12;
                                            if (NEd > 0.25 * num3 || NEd > 0.5 * (2.0 * num16 * num18) * num / GammaM0)
                                            {
                                                num8 = num4 * (1.0 - num11) / (1.0 - num19 / 2.0);
                                                num9 = num5 * (1.0 - num11) / (1.0 - num20 / 2.0);
                                                goto IL_713;
                                            }
                                            goto IL_713;
                                        }
                                    case SectionTypes.BOX:
                                        {
                                            StructWeldedBoxProperties structWeldedBoxProperties = RetrieveWELDEDBOXProps(SectionDenomination);
                                            double num14 = structWeldedBoxProperties.b * 1000.0;
                                            double num13 = structWeldedBoxProperties.h * 1000.0;
                                            num10 = structWeldedBoxProperties.tf * 1000.0;
                                            double num15 = structWeldedBoxProperties.tw * 1000.0;
                                            double num16 = num13 - 2.0 * num10;
                                            double num19 = (num12 - 2.0 * num14 * num15) / num12;
                                            double num20 = (num12 - 2.0 * num13 * num10) / num12;
                                            if (NEd > 0.25 * num3 || NEd > 0.5 * (2.0 * num16 * num15) * num / GammaM0)
                                            {
                                                num8 = num4 * (1.0 - num11) / (1.0 - num19 / 2.0);
                                                num9 = num5 * (1.0 - num11) / (1.0 - num20 / 2.0);
                                                goto IL_713;
                                            }
                                            goto IL_713;
                                        }
                                }
                                throw new SectionNotYetHandledException();
                            IL_713:
                                if (MyEd != 0.0 && MzEd == 0.0)
                                {
                                    if (VyEd <= num6 / 2.0)
                                    {
                                        if (verbose)
                                        {
                                            result = string.Concat(new object[]
											{
												"MyEd / MNyRd : ",
												MyEd,
												" / ",
												num8
											});
                                        }
                                        else
                                        {
                                            result = MyEd / num8;
                                        }
                                    }
                                    else
                                    {
                                        if (verbose)
                                        {
                                            result = string.Concat(new object[]
											{
												"NEd / NRd + MyEd / MNyRd : (",
												NEd,
												" / ",
												num3,
												") + (",
												MyEd,
												" / ",
												num8,
												")"
											});
                                        }
                                        else
                                        {
                                            result = NEd / num3 + MyEd / num8;
                                        }
                                    }
                                }
                                else
                                {
                                    if (MyEd == 0.0 && MzEd != 0.0)
                                    {
                                        if (VzEd <= num7 / 2.0)
                                        {
                                            if (verbose)
                                            {
                                                result = string.Concat(new object[]
												{
													"MzEd / MNzRd : ",
													MzEd,
													" / ",
													num9
												});
                                            }
                                            else
                                            {
                                                result = MzEd / num9;
                                            }
                                        }
                                        else
                                        {
                                            if (verbose)
                                            {
                                                result = string.Concat(new object[]
												{
													"NEd / NRd + MzEd / MNzRd : (",
													NEd,
													" / ",
													num3,
													") + (",
													MzEd,
													" / ",
													num9,
													")"
												});
                                            }
                                            else
                                            {
                                                result = NEd / num3 + MzEd / num9;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (MyEd == 0.0 || MzEd == 0.0)
                                        {
                                            throw new GeneralException("Unable to calculate interaction check");
                                        }
                                        if (VyEd <= num6 / 2.0 && VzEd <= num7 / 2.0 && strCheckType.ToUpper() == "ADVANCED")
                                        {
                                            double num21;
                                            double num22;
                                            switch (sectionTypes)
                                            {
                                                case SectionTypes.I:
                                                    num21 = 2.0;
                                                    num22 = 5.0 * num11;
                                                    if (num22 < 1.0)
                                                    {
                                                        num22 = 1.0;
                                                    }
                                                    break;
                                                case SectionTypes.RHS:
                                                    num21 = 1.66 / (1.0 - 1.13 * Math.Pow(num11, 2.0));
                                                    if (num21 > 6.0)
                                                    {
                                                        num21 = 6.0;
                                                    }
                                                    num22 = num21;
                                                    break;
                                                case SectionTypes.CHS:
                                                    num21 = 2.0;
                                                    num22 = num21;
                                                    break;
                                                default:
                                                    throw new SectionNotYetHandledException();
                                            }
                                            if (verbose)
                                            {
                                                result = string.Concat(new object[]
												{
													"(MyEd / MNyRd) ^ dblalpha + (MzEd / MNzRd) ^ dblbeta : (",
													MyEd,
													" / ",
													num8,
													") ^ ",
													num21,
													" + (",
													MzEd,
													" / ",
													num9,
													") ^ ",
													num22
												});
                                            }
                                            else
                                            {
                                                result = Math.Pow(MyEd / num8, num21) + Math.Pow(MzEd / num9, num22);
                                            }
                                        }
                                        else
                                        {
                                            if (verbose)
                                            {
                                                result = string.Concat(new object[]
												{
													"NEd / NRd + MyEd / MNyRd + MzEd / MNzRd : ",
													NEd,
													" / ",
													num3,
													" + ",
													MyEd,
													" / ",
													num8,
													" + ",
													MzEd,
													" / ",
													num9
												});
                                            }
                                            else
                                            {
                                                result = NEd / num3 + MyEd / num4 + MzEd / num5;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (num2 != 3)
                    {
                        throw new Class4SectionException();
                    }
                    double num23 = structBaseProperties.A * 1000000.0;
                    double num24 = structBaseProperties.Welyy * 1000000000.0;
                    double num25 = structBaseProperties.Welzz * 1000000000.0;
                    double num26 = NEd / num23 + MyEd / num24 + MzEd / num25;
                    if (verbose)
                    {
                        result = string.Concat(new object[]
						{
							"GammaM0 * dblMaxStress / Designfy : ",
							GammaM0,
							" * ",
							num26,
							" / ",
							num
						});
                    }
                    else
                    {
                        result = GammaM0 * num26 / num;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EC3 Functions Help", Name = "Version", Description = "EXP functions version information")]
        public static string ExpeditionEC3Version()
        {
            return "Expedition EC3 functions version 1.31. Last updated 26/01/2008";
        }
        private static StructBaseProperties RetrieveSectionProps(string Denomination)
        {
            StructBaseProperties structBaseProperties = parseGWA.parseGWASectStr(Denomination);
            if (structBaseProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structBaseProperties;
        }
        private static ISection RetrieveISectionProps(string Denomination)
        {
            ISection structIProperties = (ISection)parseGWA.parseGWASectStr(Denomination);
            if (structIProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structIProperties;
        }
        private static RectangularHollowSection RetrieveRHSProps(string Denomination)
        {
            RectangularHollowSection structRHSProperties = (RectangularHollowSection)parseGWA.parseGWASectStr(Denomination);
            if (structRHSProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structRHSProperties;
        }
        private static CircularHollowSection RetrieveCHSProps(string Denomination)
        {
            CircularHollowSection structCHSProperties = (CircularHollowSection)parseGWA.parseGWASectStr(Denomination);
            if (structCHSProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structCHSProperties;
        }
        private static StructWeldedBoxProperties RetrieveWELDEDBOXProps(string Denomination)
        {
            StructWeldedBoxProperties structWeldedBoxProperties = (StructWeldedBoxProperties)parseGWA.parseGWASectStr(Denomination);
            if (structWeldedBoxProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structWeldedBoxProperties;
        }
        private static StructINoWebProperties RetrieveINoWebSectionProps(string Denomination)
        {
            StructINoWebProperties structINoWebProperties = (StructINoWebProperties)parseGWA.parseGWASectStr(Denomination);
            if (structINoWebProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structINoWebProperties;
        }
        private static StructWeldedBoxNoWebsProperties RetrieveWELDEDBOXNoWebsSectionProps(string Denomination)
        {
            StructWeldedBoxNoWebsProperties structWeldedBoxNoWebsProperties = (StructWeldedBoxNoWebsProperties)parseGWA.parseGWASectStr(Denomination);
            if (structWeldedBoxNoWebsProperties == null)
            {
                throw new SectionNotFoundException();
            }
            return structWeldedBoxNoWebsProperties;
        }
        private static SectionTypes DetermineType(string Denomination)
        {
            try
            {
                if (parseGWA.parseGWASectStr(Denomination) == null)
                {
                    throw new SectionNotFoundException();
                }
            }
            catch
            {
            }
            try
            {
                ISection structIProperties = (ISection)parseGWA.parseGWASectStr(Denomination);
                if (structIProperties != null)
                {
                    SectionTypes result = SectionTypes.I;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            try
            {
                RectangularHollowSection structRHSProperties = (RectangularHollowSection)parseGWA.parseGWASectStr(Denomination);
                if (structRHSProperties != null)
                {
                    SectionTypes result = SectionTypes.RHS;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            try
            {
                CircularHollowSection structCHSProperties = (CircularHollowSection)parseGWA.parseGWASectStr(Denomination);
                if (structCHSProperties != null)
                {
                    SectionTypes result = SectionTypes.CHS;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            try
            {
                StructWeldedBoxProperties structWeldedBoxProperties = (StructWeldedBoxProperties)parseGWA.parseGWASectStr(Denomination);
                if (structWeldedBoxProperties != null)
                {
                    SectionTypes result = SectionTypes.BOX;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            try
            {
                StructINoWebProperties structINoWebProperties = (StructINoWebProperties)parseGWA.parseGWASectStr(Denomination);
                if (structINoWebProperties != null)
                {
                    SectionTypes result = SectionTypes.INOWEB;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            try
            {
                StructWeldedBoxNoWebsProperties structWeldedBoxNoWebsProperties = (StructWeldedBoxNoWebsProperties)parseGWA.parseGWASectStr(Denomination);
                if (structWeldedBoxNoWebsProperties != null)
                {
                    SectionTypes result = SectionTypes.BOXNOWEB;
                    return result;
                }
                throw new SectionNotFoundException();
            }
            catch
            {
            }
            return SectionTypes.OTHER;
        }
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
                    throw new GeneralException("Steel grade not found");
            }
        }
        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Steel_Design_Strength_BSEN10025", Description = "Design strength calculations in N/sq.mm according to BS:EN10025")]
        public static object DesignStrength([ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Thickness", Description = "Plate thickness in mm")] double Thickness)
        {
            object result;
            try
            {
                if (Thickness <= 0.0)
                {
                    throw new GeneralException("Thickness less than or equal to 0");
                }
                switch ((EN1993SteelGrade)Enum.Parse(typeof(EN1993SteelGrade), SteelGrade.ToUpper()))
                {
                    case EN1993SteelGrade.S235:
                        if (Thickness <= 16.0)
                        {
                            result = 235;
                            return result;
                        }
                        if (Thickness <= 40.0)
                        {
                            result = 225;
                            return result;
                        }
                        if (Thickness <= 63.0)
                        {
                            result = 215;
                            return result;
                        }
                        if (Thickness <= 80.0)
                        {
                            result = 215;
                            return result;
                        }
                        if (Thickness <= 100.0)
                        {
                            result = 215;
                            return result;
                        }
                        if (Thickness <= 150.0)
                        {
                            result = 195;
                            return result;
                        }
                        if (Thickness <= 200.0)
                        {
                            result = 185;
                            return result;
                        }
                        if (Thickness <= 250.0)
                        {
                            result = 175;
                            return result;
                        }
                        if (Thickness <= 400.0)
                        {
                            result = 165;
                            return result;
                        }
                        throw new GeneralException("Thickness exceeds code limits");
                    case EN1993SteelGrade.S275:
                        if (Thickness <= 16.0)
                        {
                            result = 275;
                            return result;
                        }
                        if (Thickness <= 40.0)
                        {
                            result = 265;
                            return result;
                        }
                        if (Thickness <= 63.0)
                        {
                            result = 255;
                            return result;
                        }
                        if (Thickness <= 80.0)
                        {
                            result = 245;
                            return result;
                        }
                        if (Thickness <= 100.0)
                        {
                            result = 235;
                            return result;
                        }
                        if (Thickness <= 150.0)
                        {
                            result = 225;
                            return result;
                        }
                        if (Thickness <= 200.0)
                        {
                            result = 215;
                            return result;
                        }
                        if (Thickness <= 250.0)
                        {
                            result = 205;
                            return result;
                        }
                        if (Thickness <= 400.0)
                        {
                            result = 195;
                            return result;
                        }
                        throw new GeneralException("Thickness exceeds code limits");
                    case EN1993SteelGrade.S355:
                        if (Thickness <= 16.0)
                        {
                            result = 355;
                            return result;
                        }
                        if (Thickness <= 40.0)
                        {
                            result = 345;
                            return result;
                        }
                        if (Thickness <= 63.0)
                        {
                            result = 335;
                            return result;
                        }
                        if (Thickness <= 80.0)
                        {
                            result = 325;
                            return result;
                        }
                        if (Thickness <= 100.0)
                        {
                            result = 315;
                            return result;
                        }
                        if (Thickness <= 150.0)
                        {
                            result = 295;
                            return result;
                        }
                        if (Thickness <= 200.0)
                        {
                            result = 285;
                            return result;
                        }
                        if (Thickness <= 250.0)
                        {
                            result = 275;
                            return result;
                        }
                        if (Thickness <= 400.0)
                        {
                            result = 265;
                            return result;
                        }
                        throw new GeneralException("Thickness exceeds code limits");
                    case EN1993SteelGrade.S450:
                        if (Thickness <= 16.0)
                        {
                            result = 450;
                            return result;
                        }
                        if (Thickness <= 40.0)
                        {
                            result = 430;
                            return result;
                        }
                        if (Thickness <= 63.0)
                        {
                            result = 410;
                            return result;
                        }
                        if (Thickness <= 80.0)
                        {
                            result = 390;
                            return result;
                        }
                        if (Thickness <= 100.0)
                        {
                            result = 380;
                            return result;
                        }
                        if (Thickness <= 150.0)
                        {
                            result = 380;
                            return result;
                        }
                        throw new GeneralException("Thickness exceeds code limits");
                    case EN1993SteelGrade.S460:
                        if (Thickness <= 16.0)
                        {
                            result = 460;
                            return result;
                        }
                        if (Thickness <= 40.0)
                        {
                            result = 460;
                            return result;
                        }
                        if (Thickness <= 63.0)
                        {
                            result = 430;
                            return result;
                        }
                        if (Thickness <= 80.0)
                        {
                            result = 430;
                            return result;
                        }
                        if (Thickness <= 100.0)
                        {
                            result = 400;
                            return result;
                        }
                        if (Thickness <= 150.0)
                        {
                            result = 400;
                            return result;
                        }
                        throw new GeneralException("Thickness exceeds code limits");
                }
                throw new GeneralException("Steel grade not covered");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Section_Design_Strength_BSEN10025", Description = "Section design strength calculations in N/sq.mm (BS:EN10025)")]
        public static object SectionDesignStrength([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade)
        {
            object result;
            try
            {
                switch (DetermineType(SectionDenomination))
                {
                    case SectionTypes.I:
                        {
                            ISection structIProperties = RetrieveISectionProps(SectionDenomination);
                            double thickness = Math.Max(structIProperties.tf, structIProperties.tw) * 1000.0;
                            result = DesignStrength(SteelGrade, thickness);
                            break;
                        }
                    case SectionTypes.RHS:
                        {
                            RectangularHollowSection structRHSProperties = RetrieveRHSProps(SectionDenomination);
                            result = DesignStrength(SteelGrade, structRHSProperties.t * 1000.0);
                            break;
                        }
                    case SectionTypes.CHS:
                        {
                            CircularHollowSection structCHSProperties = RetrieveCHSProps(SectionDenomination);
                            result = DesignStrength(SteelGrade, structCHSProperties.t * 1000.0);
                            break;
                        }
                    case SectionTypes.BOX:
                        {
                            StructWeldedBoxProperties structWeldedBoxProperties = RetrieveWELDEDBOXProps(SectionDenomination);
                            double thickness = Math.Max(structWeldedBoxProperties.tf, structWeldedBoxProperties.tw) * 1000.0;
                            result = DesignStrength(SteelGrade, thickness);
                            break;
                        }
                    case SectionTypes.INOWEB:
                        {
                            StructINoWebProperties structINoWebProperties = RetrieveINoWebSectionProps(SectionDenomination);
                            double thickness = structINoWebProperties.tf * 1000.0;
                            result = DesignStrength(SteelGrade, thickness);
                            break;
                        }
                    case SectionTypes.BOXNOWEB:
                        {
                            StructWeldedBoxNoWebsProperties structWeldedBoxNoWebsProperties = RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
                            double thickness = structWeldedBoxNoWebsProperties.tw * 1000.0;
                            result = DesignStrength(SteelGrade, thickness);
                            break;
                        }
                    default:
                        throw new SectionNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_class_EC3", Description = "Section class for bending or compression (EC3 cl 5.5)")]
        public static object SectionClass([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Type", Description = "Classification type: ''BENDING'' or ''COMPRESSION''. Input as string")] string ClassificationType)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double fy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                string a;
                if ((a = ClassificationType.ToUpper()) != null)
                {
                    if (a == "BENDING")
                    {
                        result = structBaseProperties.getEN1993FlexureClass(fy);
                        return result;
                    }
                    if (a == "COMPRESSION")
                    {
                        result = structBaseProperties.getEN1993CompressionClass(fy);
                        return result;
                    }
                }
                result = "Please specify the classification type";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Section_properties", Description = "Outputs the section properties: A, Iyy, Izz, iy, iz, Welyy, Welzz, Wplyy, Wplzz, It, Iw. Base unit: mm")]
        public static object SectionProperties([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Property index", Description = "Section property index: 1.A, 2.Iyy, 3.Izz, 4.iy, 5.iz, 6.Welyy, 7.Welzz, 8.Wplyy, 9.Wplzz, 10.It, 11.Iw")] int PropIndex)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double[] array = new double[]
				{
					structBaseProperties.A * 1000000.0,
					structBaseProperties.Iyy * 1000000000000.0,
					structBaseProperties.Izz * 1000000000000.0,
					structBaseProperties.iy * 1000.0,
					structBaseProperties.iz * 1000.0,
					structBaseProperties.Welyy * 1000000000.0,
					structBaseProperties.Welzz * 1000000000.0,
					structBaseProperties.Wplyy * 1000000000.0,
					structBaseProperties.Wplzz * 1000000000.0,
					structBaseProperties.It * 1000000000000.0,
					structBaseProperties.Iw * 1E+18
				};
                if (PropIndex < 0 || PropIndex > 11)
                {
                    throw new GeneralException("Invalid property index");
                }
                if (PropIndex == 0)
                {
                    result = array;
                }
                else
                {
                    result = array[PropIndex - 1];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP Engineering Functions", Name = "Generate_Box_Section_GWA", Description = "Outputs the GSA walkaround code for a box section")]
        public static object GENBoxSectionGwa([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination)
        {
            object result;
            try
            {
                switch (DetermineType(SectionDenomination))
                {
                    case SectionTypes.BOX:
                        {
                            StructWeldedBoxProperties structWeldedBoxProperties = RetrieveWELDEDBOXProps(SectionDenomination);
                            result = structWeldedBoxProperties.genGWADef();
                            return result;
                        }
                    case SectionTypes.BOXNOWEB:
                        {
                            StructWeldedBoxNoWebsProperties structWeldedBoxNoWebsProperties = RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
                            result = structWeldedBoxNoWebsProperties.genGWADef();
                            return result;
                        }
                }
                throw new SectionNotYetHandledException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ExcelFunction(Category = "EC3 Test functions", Name = "Cross_Section_Tension_Resistance", Description = "Cross-section tension resistance in kN, see EC3 cl. 6.2.3")]
        private static object CrossSectionTensionResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Area reduction", Description = "(Optional) Ratio of the net cross section area over the gross cross section area. (Default = 1)")] double AreaRed, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "GammaM2", Description = "(Optional) Partial safety factor for net cross-section resistance. (Default = 1.25)")] double GammaM2)
        {
            object result;
            try
            {
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                if (GammaM2 == 0.0)
                {
                    GammaM2 = 1.25;
                }
                if (AreaRed == 0.0)
                {
                    AreaRed = 1.0;
                }
                else
                {
                    if (AreaRed > 1.0)
                    {
                        throw new GeneralException("Net area can not exceed gross area");
                    }
                }
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double num2 = structBaseProperties.A * 1000000.0;
                double num3 = num2 * num / GammaM0;
                if (AreaRed < 1.0)
                {
                    throw new GeneralException("Net cross section areas not dealt with yet");
                }
                result = num3;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Compression_Resistance", Description = "Section compression resistance in kN (EC3 cl. 6.2.4)")]
        public static object CrossSectionCompressionResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade/fy", Description = "Steel grade input as string, e.g. ''S235'', or design yield stress input as integer")] object SteelGrade, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                double num;
                if (!double.TryParse(SteelGrade.ToString(), out num))
                {
                    num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());
                }
                if (CompClass < 0 || CompClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (CompClass == 0)
                {
                    CompClass = structBaseProperties.getEN1993CompressionClass(num);
                }
                if (CompClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num2 = structBaseProperties.A * 1000000.0;
                result = num2 * num / GammaM0 / 1000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Bending_Resistance", Description = "Section bending resistance in kNm (EC3 cl. 6.2.5)")]
        public static object CrossSectionBendingResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Bending axis", Description = "Axis of bending, either ''YY'' or ''ZZ''")] string Axis, [ExcelArgument(Name = "VEd", Description = "(Optional) Design shear force perpendicular to the axis of bending in kN. (Default = 0kN)")] double DesignShear, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
        {
            object result;
            try
            {
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(num);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num2;
                double num3;
                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        num2 = structBaseProperties.Wplyy;
                        if (BendClass == 3)
                        {
                            num2 = structBaseProperties.Welyy;
                        }
                        num3 = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Z", GammaM0).ToString());
                        break;
                    case BendingAxis.ZZ:
                        num2 = structBaseProperties.Wplzz;
                        if (BendClass == 3)
                        {
                            num2 = structBaseProperties.Welzz;
                        }
                        num3 = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Y", GammaM0).ToString());
                        break;
                    default:
                        result = "Please specify the bending axis";
                        return result;
                }
                num2 *= 1000000000.0;
                double num4 = 0.0;
                double num5 = 0.0;
                if (DesignShear > num3 / 2.0 && DesignShear <= num3)
                {
                    num4 = Math.Pow(2.0 * DesignShear / num3 - 1.0, 2.0);
                    switch (DetermineType(SectionDenomination))
                    {
                        case SectionTypes.I:
                            {
                                ISection structIProperties = RetrieveISectionProps(SectionDenomination);
                                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                                {
                                    case BendingAxis.YY:
                                        {
                                            double num6 = structIProperties.h - 2.0 * structIProperties.tf;
                                            double tw = structIProperties.tw;
                                            num5 = Math.Pow(num6, 2.0) * tw / 4.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = Math.Pow(num6, 3.0) * tw / 12.0 / (num6 / 2.0);
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    case BendingAxis.ZZ:
                                        {
                                            double b = structIProperties.b;
                                            double tf = structIProperties.tf;
                                            num5 = Math.Pow(b, 2.0) * tf / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(b, 3.0) * tf / 12.0 / (b / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    default:
                                        goto IL_698;
                                }
                            }
                        case SectionTypes.RHS:
                            {
                                RectangularHollowSection structRHSProperties = RetrieveRHSProps(SectionDenomination);
                                double t = structRHSProperties.t;
                                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                                {
                                    case BendingAxis.YY:
                                        {
                                            double num7 = structRHSProperties.h - 2.0 * t;
                                            num5 = Math.Pow(num7, 2.0) * t / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(num7, 3.0) * t / 12.0 / (num7 / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    case BendingAxis.ZZ:
                                        {
                                            double num8 = structRHSProperties.b - 2.0 * t;
                                            num5 = Math.Pow(num8, 2.0) * t / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(num8, 3.0) * t / 12.0 / (num8 / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    default:
                                        goto IL_698;
                                }
                            }
                        case SectionTypes.BOX:
                            {
                                StructWeldedBoxProperties structWeldedBoxProperties = RetrieveWELDEDBOXProps(SectionDenomination);
                                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                                {
                                    case BendingAxis.YY:
                                        {
                                            double h = structWeldedBoxProperties.h;
                                            double tw2 = structWeldedBoxProperties.tw;
                                            num5 = Math.Pow(h, 2.0) * tw2 / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(h, 3.0) * tw2 / 12.0 / (h / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    case BendingAxis.ZZ:
                                        {
                                            double num9 = structWeldedBoxProperties.b - 2.0 * structWeldedBoxProperties.tw;
                                            double tf2 = structWeldedBoxProperties.tf;
                                            num5 = Math.Pow(num9, 2.0) * tf2 / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(num9, 3.0) * tf2 / 12.0 / (num9 / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    default:
                                        goto IL_698;
                                }
                            }
                        case SectionTypes.INOWEB:
                            {
                                StructINoWebProperties structINoWebProperties = RetrieveINoWebSectionProps(SectionDenomination);
                                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                                {
                                    case BendingAxis.YY:
                                        result = "Section can not carry shear in this direction";
                                        return result;
                                    case BendingAxis.ZZ:
                                        {
                                            double b2 = structINoWebProperties.b;
                                            double tf3 = structINoWebProperties.tf;
                                            num5 = Math.Pow(b2, 2.0) * tf3 / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(b2, 3.0) * tf3 / 12.0 / (b2 / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    default:
                                        goto IL_698;
                                }
                            }
                        case SectionTypes.BOXNOWEB:
                            {
                                StructWeldedBoxNoWebsProperties structWeldedBoxNoWebsProperties = RetrieveWELDEDBOXNoWebsSectionProps(SectionDenomination);
                                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                                {
                                    case BendingAxis.YY:
                                        {
                                            double h2 = structWeldedBoxNoWebsProperties.h;
                                            double tw3 = structWeldedBoxNoWebsProperties.tw;
                                            num5 = Math.Pow(h2, 2.0) * tw3 / 2.0;
                                            if (BendClass == 3)
                                            {
                                                num5 = 2.0 * (Math.Pow(h2, 3.0) * tw3 / 12.0 / (h2 / 2.0));
                                                goto IL_698;
                                            }
                                            goto IL_698;
                                        }
                                    case BendingAxis.ZZ:
                                        result = "Section can not carry shear in this direction";
                                        return result;
                                    default:
                                        goto IL_698;
                                }
                            }
                    }
                    throw new SectionNotYetHandledException();
                }
                if (DesignShear > num3)
                {
                    result = "Shear failure";
                    return result;
                }
            IL_698:
                num5 *= 1000000000.0;
                double num10 = (num2 - num4 * num5) * num / GammaM0 / 1000000.0;
                if (num10 > num2 * num / GammaM0 / 1000000.0)
                {
                    num10 = num2 * num / GammaM0 / 1000000.0;
                }
                result = num10;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Shear_Resistance", Description = "Section shear resistance in kN (EC3 cl. 6.2.6)")]
        public static object CrossSectionPlasticShearResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Axis", Description = "Axis parallel to the shear force, ''Y'' (major axis) or ''Z'' (minor axis)")] string ShearForceParallelAxis, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                switch ((ShearAxis)Enum.Parse(typeof(ShearAxis), ShearForceParallelAxis.ToUpper()))
                {
                    case ShearAxis.Y:
                        {
                            double num2 = structBaseProperties.getEN1993ShearArea(fy).Avzz * 1000000.0;
                            result = num2 * (num / Math.Sqrt(3.0)) / GammaM0 / 1000.0;
                            break;
                        }
                    case ShearAxis.Z:
                        {
                            double num2 = structBaseProperties.getEN1993ShearArea(fy).Avyy * 1000000.0;
                            result = num2 * (num / Math.Sqrt(3.0)) / GammaM0 / 1000.0;
                            break;
                        }
                    default:
                        result = "Please specify bending axis";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Torsion_Resistance", Description = "Section torsion resistance kNm (EC3 cl. 6.2.7)")]
        public static object CrossSectionTorsionResitance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Bending axis", Description = "Axis of bending, ''YY'' or ''ZZ''")] string Axis, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. Taken as 1 by default")] double GammaM0)
        {
            object result;
            try
            {
                result = "Not implemented yet";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Section_Combined_Axial_And_Bending_Check", Description = "Section utilisation under combined axial load and bending (EC3 cl. 6.2.9)")]
        public static object CombinedAxialAndBendingChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd, [ExcelArgument(Name = "VyEd", Description = "(Optional) Shear force parallel to the major axis in kN. (Default = 0kN)")] double VyEd, [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double VzEd, [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MyEd, [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MzEd, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. Calculated according to EC3 cl. 5.6 by default")] int CompClass, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. Calculated according to EC3 cl. 5.6 by default")] int BendClass, [ExcelArgument(Name = "Check type", Description = "(Optional) ''ADVANCED'' or ''CONSERVATIVE''. ''ADVANCED'' carries out non-linear checks even for high shear. (Default = ''CONSERVATIVE'')")] string strCheckType)
        {
            return CombinedAxialAndBendingChecks(false, SectionDenomination, SteelGrade, NEd, VyEd, VzEd, MyEd, MzEd, GammaM0, CompClass, BendClass, strCheckType);
        }
        [ExcelFunction(Category = "EXP EC3 Functions Verbose", Name = "vSection_Combined_Axial_And_Bending_Check", Description = "Section utilisation under combined axial load and bending (EC3 cl. 6.2.9)")]
        public static object vCombinedAxialAndBendingChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd, [ExcelArgument(Name = "VyEd", Description = "(Optional) Shear force parallel to the major axis in kN. (Default = 0kN)")] double VyEd, [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double VzEd, [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MyEd, [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MzEd, [ExcelArgument(Name = "GammaM0", Description = "(Optional) Partial safety factor for cross-section resistance. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. Calculated according to EC3 cl. 5.6 by default")] int CompClass, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. Calculated according to EC3 cl. 5.6 by default")] int BendClass, [ExcelArgument(Name = "Check type", Description = "(Optional) ''ADVANCED'' or ''CONSERVATIVE''. ''ADVANCED'' carries out non-linear checks even for high shear. (Default = ''CONSERVATIVE'')")] string strCheckType)
        {
            return CombinedAxialAndBendingChecks(true, SectionDenomination, SteelGrade, NEd, VyEd, VzEd, MyEd, MzEd, GammaM0, CompClass, BendClass, strCheckType);
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Buckling_Curve", Description = "Buckling curve for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingCurve([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                EN1993SteelGrade grade = (EN1993SteelGrade)Enum.Parse(typeof(EN1993SteelGrade), SteelGrade.ToUpper());
                EN1993CompressionBucklingCurve eN1993CompressionBucklingCurves = structBaseProperties.getEN1993CompressionBucklingCurves(grade);
                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        result = eN1993CompressionBucklingCurves.majorAxis.ToString();
                        break;
                    case BendingAxis.ZZ:
                        result = eN1993CompressionBucklingCurves.minorAxis.ToString();
                        break;
                    default:
                        throw new SectionNotYetHandledException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Imperfection_Factor", Description = "Imperfection factor for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingImperfectionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
        {
            object result;
            try
            {
                RetrieveSectionProps(SectionDenomination);
                string text = CompressionBucklingCurve(SectionDenomination, SteelGrade, Axis).ToString();
                switch ((EN1993CompressionBucklingCurves)Enum.Parse(typeof(EN1993CompressionBucklingCurves), text.ToLower()))
                {
                    case EN1993CompressionBucklingCurves.a0:
                        result = 0.13;
                        break;
                    case EN1993CompressionBucklingCurves.a:
                        result = 0.21;
                        break;
                    case EN1993CompressionBucklingCurves.b:
                        result = 0.34;
                        break;
                    case EN1993CompressionBucklingCurves.c:
                        result = 0.49;
                        break;
                    case EN1993CompressionBucklingCurves.d:
                        result = 0.76;
                        break;
                    default:
                        throw new SectionNotYetHandledException();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Elastic_Critical_Force", Description = "Elastic critical compression force for compression buckling in kN (EC 3 cl. 6.3.1.2)")]
        public static object CompressionBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                Lcr *= 1000.0;
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                switch ((BendingAxis)Enum.Parse(typeof(BendingAxis), Axis.ToUpper()))
                {
                    case BendingAxis.YY:
                        {
                            double num = structBaseProperties.Iyy * 1000000000000.0;
                            result = Math.Pow(3.1415926535897931, 2.0) * E * num / Math.Pow(Lcr, 2.0) / 1000.0;
                            break;
                        }
                    case BendingAxis.ZZ:
                        {
                            double num = structBaseProperties.Izz * 1000000000000.0;
                            result = Math.Pow(3.1415926535897931, 2.0) * E * num / Math.Pow(Lcr, 2.0) / 1000.0;
                            break;
                        }
                    default:
                        throw new GeneralException("Elastic critical buckling force error");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_NonDimensional_Slenderness", Description = "Non-dimensional slenderness for compression buckling (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingNonDimensionalSlenderness([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string.")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis, [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                double num2 = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, Lcr, Axis, E).ToString()) * 1000.0;
                if (CompClass < 0 || CompClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (CompClass == 0)
                {
                    CompClass = structBaseProperties.getEN1993CompressionClass(num);
                }
                if (CompClass == 4)
                {
                    throw new Class4SectionException();
                }
                result = Math.Sqrt(structBaseProperties.A * 1000000.0 * num / num2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Reduction_Factor", Description = "Compression buckling resistance reduction factor (EC3 cl. 6.3.1.2)")]
        public static object CompressionBucklingReductionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis, [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm as recommended in EC3)")] double E, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                double num = double.Parse(CompressionBucklingImperfectionFactor(SectionDenomination, SteelGrade, Axis).ToString());
                double num2 = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, Axis, Lcr, E, CompClass).ToString());
                double num3 = 0.5 * (1.0 + num * (num2 - 0.2) + Math.Pow(num2, 2.0));
                double num4 = 1.0 / (num3 + Math.Sqrt(Math.Pow(num3, 2.0) - Math.Pow(num2, 2.0)));
                if (num4 > 1.0)
                {
                    result = 1;
                }
                else
                {
                    result = num4;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Compression_Buckling_Design_Resistance", Description = "Design compression buckling resistance in kN (EC3 cl. 6.3.1.1)")]
        public static object CompressionBucklingDesignResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Buckling axis", Description = "Buckling Axis, ''YY'' or ''ZZ''. Input as string")] string Axis, [ExcelArgument(Name = "Lcr", Description = "Buckling length in m. (See BS5950 for reference)")] double Lcr, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                if (GammaM1 == 0.0)
                {
                    GammaM1 = 1.0;
                }
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                double num2 = double.Parse(CompressionBucklingReductionFactor(SectionDenomination, SteelGrade, Axis, Lcr, E, CompClass).ToString());
                result = num2 * structBaseProperties.A * 1000000.0 * num / GammaM1 / 1000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_buckling_curve", Description = "Lateral torsional buckling buckling curve (EC3 cl. 6.3.2.2)")]
        public static object LTBBucklingCurve([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                EN1993LTBBucklingCurve eN1993LTBBucklingCurve = structBaseProperties.getEN1993LTBBucklingCurve();
                result = eN1993LTBBucklingCurve.LTBBucklingCurve.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Buckling_Imperfection_Factor", Description = "Imperfection factor for lateral torsional buckling (EC3 cl. 6.3.2.2)")]
        public static object LTBBucklingImperfectionFactor([ExcelArgument(Name = "Section name", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade)
        {
            object result;
            try
            {
                DetermineType(SectionDenomination);
                string text = LTBBucklingCurve(SectionDenomination).ToString();
                switch ((EN1993LTBBucklingCurves)Enum.Parse(typeof(EN1993LTBBucklingCurves), text.ToLower()))
                {
                    case EN1993LTBBucklingCurves.a:
                        result = 0.21;
                        break;
                    case EN1993LTBBucklingCurves.b:
                        result = 0.34;
                        break;
                    case EN1993LTBBucklingCurves.c:
                        result = 0.49;
                        break;
                    case EN1993LTBBucklingCurves.d:
                        result = 0.76;
                        break;
                    default:
                        throw new GeneralException("Buckling factor could not be determined");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Critical_Elastic_Moment", Description = "Elastic critical moment for LTB in kNm (cl. 6.3.2.2)")]
        public static object LTBCriticalMoment([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                if (nu == 0.0)
                {
                    nu = 0.297;
                }
                Lcr *= 1000.0;
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section is not doubly-symmetric");
                }
                double num = E / (2.0 * (1.0 + nu));
                double num2 = structBaseProperties.Izz * 1000000000000.0;
                double num3 = structBaseProperties.Iw * 1E+18;
                double num4 = structBaseProperties.It * 1000000000000.0;
                double num5 = Math.Pow(3.1415926535897931, 2.0) * E * num2 / Math.Pow(Lcr, 2.0);
                double num6 = Math.Sqrt(num3 / num2 + Math.Pow(Lcr, 2.0) * num * num4 / (Math.Pow(3.1415926535897931, 2.0) * E * num2));
                result = C1 * num5 * num6 / 1000000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_NonDimensional_Slenderness", Description = "Determines the non-dimensional slenderness (cl. 6.3.2)")]
        public static object LTBNonDimensionalSlenderness([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
        {
            object result;
            try
            {
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                if (nu == 0.0)
                {
                    nu = 0.297;
                }
                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section not doubly-symmetric");
                }
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(fy);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num2 = structBaseProperties.Wplyy * 1000000000.0;
                if (BendClass == 3)
                {
                    num2 = structBaseProperties.Welyy * 1000000000.0;
                }
                double num3 = double.Parse(LTBCriticalMoment(SectionDenomination, Lcr, C1, E, nu).ToString()) * 1000000.0;
                result = Math.Sqrt(num2 * num / num3);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Reduction_Factor", Description = "Determines the LTB buckling reduction factor")]
        public static object LTBReductionFactor([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1s, [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
        {
            object result;
            try
            {
                if (GammaM1 == 0.0)
                {
                    GammaM1 = 1.0;
                }
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                if (nu == 0.0)
                {
                    nu = 0.297;
                }
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section not doubly-symmetric");
                }
                double num = double.Parse(LTBBucklingImperfectionFactor(SectionDenomination, SteelGrade).ToString());
                double num2 = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination, SteelGrade, Lcr, C1s, E, nu, BendClass).ToString());
                double num3 = 0.5 * (1.0 + num * (num2 - 0.2) + Math.Pow(num2, 2.0));
                double num4 = 1.0 / (num3 + Math.Sqrt(Math.Pow(num3, 2.0) - Math.Pow(num2, 2.0)));
                if (num4 > 1.0)
                {
                    result = 1;
                }
                else
                {
                    result = num4;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "LTB_Design_Buckling_Resistance", Description = "Determines the LTB buckling design resistance Mb,rd in kNm (EC3 cl.6.3.2 eq.6.55)")]
        public static object LTBDesignBucklingResistance([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "Lcr", Description = "Length in m between points of lateral restraint")] double Lcr, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1, [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu, [ExcelArgument(Name = "Bend. class", Description = "((Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass)
        {
            object result;
            try
            {
                if (GammaM1 == 0.0)
                {
                    GammaM1 = 1.0;
                }
                if (E == 0.0)
                {
                    E = 210000.0;
                }
                if (nu == 0.0)
                {
                    nu = 0.297;
                }
                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                double num = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade).ToString());
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section not doubly-symmetric");
                }
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(fy);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num2 = double.Parse(LTBReductionFactor(SectionDenomination, SteelGrade, Lcr, C1, GammaM1, E, nu, BendClass).ToString());
                double num3 = structBaseProperties.Wplyy * 1000000000.0;
                if (BendClass == 3)
                {
                    num3 = structBaseProperties.Welyy * 1000000000.0;
                }
                result = num2 * num3 * num / GammaM1 / 1000000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Buckling_Interaction_K_Factors", Description = "Determines the K factors used in the combined buckling and axial bending checks")]
        public static object OutputKfactors([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd, [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double DesignShear, [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MEdY, [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MEdZ, [ExcelArgument(Name = "LcrY (m)", Description = "Buckling length about YY axis in m. (See BS5950 for reference)")] double LcrY, [ExcelArgument(Name = "LcrZ (m)", Description = "Buckling length about ZZ axis in m. (See BS5950 for reference)")] double LcrZ, [ExcelArgument(Name = "LcrLT (m)", Description = "Lateral torsion buckling length in m. (See BS5950 for reference)")] double LcrLT, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1, [ExcelArgument(Name = "CmY", Description = "Factor CmY depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmY, [ExcelArgument(Name = "CmZ", Description = "Factor CmZ depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmZ, [ExcelArgument(Name = "CmLT", Description = "Factor CmLT depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmLT, [ExcelArgument(Name = "GammaM0", Description = "(Optional) GammaM0 safety factor. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass, [ExcelArgument(Name = "Kfactor", Description = "K factor to output: ''YY'', ''ZZ'', ''YZ'' or ''ZY''")] string KFactor)
        {
            object result;
            try
            {
                SectionTypes sectionType = DetermineType(SectionDenomination);
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                if (GammaM1 == 0.0)
                {
                    GammaM1 = 1.0;
                }
                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                if (CompClass < 0 || CompClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (CompClass == 0)
                {
                    CompClass = structBaseProperties.getEN1993CompressionClass(fy);
                }
                if (CompClass == 4)
                {
                    throw new Class4SectionException();
                }
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(fy);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num = double.Parse(CompressionBucklingDesignResistance(SectionDenomination, SteelGrade, "YY", LcrY, E, GammaM1, CompClass).ToString());
                double nbRdz = double.Parse(CompressionBucklingDesignResistance(SectionDenomination, SteelGrade, "ZZ", LcrZ, E, GammaM1, CompClass).ToString());
                double.Parse(LTBDesignBucklingResistance(SectionDenomination, SteelGrade, LcrLT, C1, GammaM1, E, nu, BendClass).ToString());
                double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", DesignShear, GammaM0, BendClass).ToString());
                double lambdaBarY = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, "YY", LcrY, E, CompClass).ToString());
                double lambdaBarZ = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, "ZZ", LcrZ, E, CompClass).ToString());
                double num2 = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination, SteelGrade, LcrLT, C1, E, nu, BendClass).ToString());
                string a;
                if ((a = KFactor.ToUpper()) != null)
                {
                    if (a == "YY")
                    {
                        result = double.Parse(FactorKyy(NEd, num, CmY, lambdaBarY, GammaM1, BendClass).ToString());
                        return result;
                    }
                    if (a == "ZZ")
                    {
                        result = double.Parse(FactorKzz_AppBB(NEd, nbRdz, CmZ, lambdaBarZ, GammaM1, BendClass, sectionType).ToString());
                        return result;
                    }
                    if (a == "YZ")
                    {
                        result = double.Parse(FactorKyz_AppBB(NEd, nbRdz, CmZ, lambdaBarZ, GammaM1, BendClass, sectionType).ToString());
                        return result;
                    }
                    if (a == "ZY")
                    {
                        double num3 = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, LcrZ, "ZZ", E).ToString());
                        double num4 = double.Parse(TorsionFlexuralBucklingCriticalElasticForce(SectionDenomination, LcrZ, LcrLT, E, nu, "ZZ").ToString());
                        double num5 = 0.2 * Math.Sqrt(C1) * Math.Pow((1.0 - NEd / num3) * (1.0 - NEd / num4), 0.0);
                        if (num5 < num2)
                        {
                            result = double.Parse(FactorKzyTorsion_AppBB(NEd, num, CmLT, lambdaBarZ, GammaM1, structBaseProperties.getEN1993FlexureClass(fy)).ToString());
                            return result;
                        }
                        result = double.Parse(FactorKzy_AppBB(NEd, num, CmY, lambdaBarY, GammaM1, structBaseProperties.getEN1993FlexureClass(fy)).ToString());
                        return result;
                    }
                }
                throw new GeneralException("Unrecognised K factor");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static double FactorKyy(double Ned, double NbRdy, double Cmy, double LambdaBarY, double GammaM1, int SectionClass)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                double num = Cmy * (1.0 + (LambdaBarY - 0.2) * Ned / NbRdy);
                if (num > Cmy * (1.0 + 0.8 * Ned / NbRdy))
                {
                    return Cmy * (1.0 + 0.8 * Ned / NbRdy);
                }
                return num;
            }
            else
            {
                double num = Cmy * (1.0 + 0.6 * LambdaBarY * Ned / NbRdy);
                if (num > Cmy * (1.0 + 0.6 * Ned / NbRdy))
                {
                    return Cmy * (1.0 + 0.6 * Ned / NbRdy);
                }
                return num;
            }
        }
        private static double FactorKyz_AppBB(double Ned, double NbRdz, double CmZ, double LambdaBarZ, double GammaM1, int SectionClass, SectionTypes SectionType)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                return 0.6 * FactorKzz_AppBB(Ned, NbRdz, CmZ, LambdaBarZ, GammaM1, SectionClass, SectionType);
            }
            return FactorKzz_AppBB(Ned, NbRdz, CmZ, LambdaBarZ, GammaM1, SectionClass, SectionType);
        }
        private static double FactorKzy_AppBB(double Ned, double NbRdy, double Cmy, double LambdaBarY, double GammaM1, int SectionClass)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                return 0.6 * FactorKyy(Ned, NbRdy, Cmy, LambdaBarY, GammaM1, SectionClass);
            }
            return 0.8 * FactorKyy(Ned, NbRdy, Cmy, LambdaBarY, GammaM1, SectionClass);
        }
        private static double FactorKzyTorsion_AppBB(double Ned, double NbRdz, double CmLT, double LambdaBarZ, double GammaM1, int SectionClass)
        {
            double num = 1.0 - 0.05 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
            if (SectionClass == 1 || SectionClass == 2)
            {
                num = 1.0 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
                if (LambdaBarZ < 0.4)
                {
                    num = 0.6 + LambdaBarZ;
                }
                if (num > 1.0 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz))
                {
                    num = 1.0 - 0.1 * LambdaBarZ * Ned / ((CmLT - 0.25) * NbRdz);
                }
                if (num < 1.0 - 0.1 * Ned / ((CmLT - 0.25) * NbRdz))
                {
                    return 1.0 - 0.1 * Ned / ((CmLT - 0.25) * NbRdz);
                }
                return num;
            }
            else
            {
                if (num < 1.0 - 0.05 * Ned / ((CmLT - 0.25) * NbRdz))
                {
                    return 1.0 - 0.05 * Ned / ((CmLT - 0.25) * NbRdz);
                }
                return num;
            }
        }
        private static double FactorKzz_AppBB(double Ned, double NbRdz, double CmZ, double LambdaBarZ, double GammaM1, int SectionClass, SectionTypes SectionType)
        {
            if (SectionClass == 1 || SectionClass == 2)
            {
                double num = CmZ * (1.0 + (2.0 * LambdaBarZ - 0.6) * Ned / NbRdz);
                if (num > CmZ * (1.0 + 1.4 * Ned / NbRdz))
                {
                    num = CmZ * (1.0 + 1.4 * Ned / NbRdz);
                }
                double num2 = CmZ * (1.0 + (LambdaBarZ - 0.2) * Ned / NbRdz);
                if (num2 > CmZ * (1.0 + 0.8 * Ned / NbRdz))
                {
                    num2 = CmZ * (1.0 + 0.8 * Ned / NbRdz);
                }
                switch (SectionType)
                {
                    case SectionTypes.I:
                        return num;
                    case SectionTypes.RHS:
                        return num2;
                    default:
                        return Math.Max(num, num2);
                }
            }
            else
            {
                double num3 = CmZ * (1.0 + 0.6 * LambdaBarZ * Ned / NbRdz);
                if (num3 > CmZ * (1.0 + 0.6 * Ned / NbRdz))
                {
                    return CmZ * (1.0 + 0.6 * Ned / NbRdz);
                }
                return num3;
            }
        }
        [ExcelFunction(Category = "EXP EC3 Functions", Name = "Buckling_Combined_Axial_And_Bending_Checks", Description = "Section utilisation with regards to buckling under combined bending and axial loads")]
        public static object BendingAxialInteractionChecks([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Steel grade", Description = "Steel grade, e.g. ''S235''. Input as string")] string SteelGrade, [ExcelArgument(Name = "NEd", Description = "Axial Force in kN")] double NEd, [ExcelArgument(Name = "VzEd", Description = "(Optional) Shear force parallel to the minor axis in kN. (Default = 0kN)")] double DesignShear, [ExcelArgument(Name = "MyEd", Description = "Major axis moment in kNm")] double MEdY, [ExcelArgument(Name = "MzEd", Description = "Minor axis moment in kNm")] double MEdZ, [ExcelArgument(Name = "LcrY (m)", Description = "Buckling length about YY axis in m. (See BS5950 for reference)")] double LcrY, [ExcelArgument(Name = "LcrZ (m)", Description = "Buckling length about ZZ axis in m. (See BS5950 for reference)")] double LcrZ, [ExcelArgument(Name = "LcrLT (m)", Description = "Lateral torsion buckling length in m. (See BS5950 for reference)")] double LcrLT, [ExcelArgument(Name = "C1", Description = "Factor C1 depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double C1, [ExcelArgument(Name = "CmY", Description = "Factor CmY depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmY, [ExcelArgument(Name = "CmZ", Description = "Factor CmZ depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmZ, [ExcelArgument(Name = "CmLT", Description = "Factor CmLT depending on the moment distribution along the beam. See blue book or Access Steel for reference")] double CmLT, [ExcelArgument(Name = "GammaM0", Description = "(Optional) GammaM0 safety factor. (Default = 1)")] double GammaM0, [ExcelArgument(Name = "GammaM1", Description = "(Optional) GammaM1 safety factor. (Default = 1)")] double GammaM1, [ExcelArgument(Name = "E", Description = "(Optional) Elastic modulus in N/sq.mm. (Default = 210000N/sq.mm. as recommended in EC3)")] double E, [ExcelArgument(Name = "nu", Description = "(Optional) Poisson's ratio. (Default = 0.297)")] double nu, [ExcelArgument(Name = "Comp. class", Description = "(Optional) Section compression class. (Calculated according to EC3 cl. 5.6 by default)")] int CompClass, [ExcelArgument(Name = "Bend. class", Description = "(Optional) Section bending class. (Calculated according to EC3 cl. 5.6 by default)")] int BendClass, [ExcelArgument(Name = "Check", Description = "Check to carry out: 1 for eq. (6.61), 2 for eq. (6.62)")] int Check1Or2)
        {
            object result;
            try
            {
                SectionTypes sectionType = DetermineType(SectionDenomination);
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (GammaM0 == 0.0)
                {
                    GammaM0 = 1.0;
                }
                if (GammaM1 == 0.0)
                {
                    GammaM1 = 1.0;
                }
                double fy = double.Parse(YieldStrength(SteelGrade).ToString());
                if (CompClass < 0 || CompClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (CompClass == 0)
                {
                    CompClass = structBaseProperties.getEN1993CompressionClass(fy);
                }
                if (CompClass == 4)
                {
                    throw new Class4SectionException();
                }
                if (BendClass < 0 || BendClass > 4)
                {
                    throw new InvalidSectionClassException();
                }
                if (BendClass == 0)
                {
                    BendClass = structBaseProperties.getEN1993FlexureClass(fy);
                }
                if (BendClass == 4)
                {
                    throw new Class4SectionException();
                }
                double num = double.Parse(CompressionBucklingDesignResistance(SectionDenomination, SteelGrade, "YY", LcrY, E, GammaM1, CompClass).ToString());
                double num2 = double.Parse(CompressionBucklingDesignResistance(SectionDenomination, SteelGrade, "ZZ", LcrZ, E, GammaM1, CompClass).ToString());
                double num3 = double.Parse(LTBDesignBucklingResistance(SectionDenomination, SteelGrade, LcrLT, C1, GammaM1, E, nu, BendClass).ToString());
                double num4 = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", DesignShear, GammaM0, BendClass).ToString());
                double lambdaBarY = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, "YY", LcrY, E, CompClass).ToString());
                double lambdaBarZ = double.Parse(CompressionBucklingNonDimensionalSlenderness(SectionDenomination, SteelGrade, "ZZ", LcrZ, E, CompClass).ToString());
                double num5 = double.Parse(LTBNonDimensionalSlenderness(SectionDenomination, SteelGrade, LcrLT, C1, E, nu, BendClass).ToString());
                double num6 = double.Parse(FactorKyy(NEd, num, CmY, lambdaBarY, GammaM1, BendClass).ToString());
                double num7 = double.Parse(FactorKzz_AppBB(NEd, num2, CmZ, lambdaBarZ, GammaM1, BendClass, sectionType).ToString());
                double num8 = double.Parse(FactorKyz_AppBB(NEd, num2, CmZ, lambdaBarZ, GammaM1, BendClass, sectionType).ToString());
                double num9 = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, LcrZ, "ZZ", E).ToString());
                double num10 = double.Parse(TorsionFlexuralBucklingCriticalElasticForce(SectionDenomination, LcrZ, LcrLT, E, nu, "ZZ").ToString());
                double num11 = 0.2 * Math.Sqrt(C1) * Math.Pow((1.0 - NEd / num9) * (1.0 - NEd / num10), 0.0);
                double num12 = double.Parse(FactorKzy_AppBB(NEd, num, CmY, lambdaBarY, GammaM1, structBaseProperties.getEN1993FlexureClass(fy)).ToString());
                if (num11 < num5)
                {
                    num12 = double.Parse(FactorKzyTorsion_AppBB(NEd, num, CmLT, lambdaBarZ, GammaM1, structBaseProperties.getEN1993FlexureClass(fy)).ToString());
                }
                if (Check1Or2 == 1)
                {
                    result = NEd / num + num6 * MEdY / num3 + num8 * MEdZ / (num4 / GammaM1);
                }
                else
                {
                    result = NEd / num2 + num12 * MEdY / num3 + num7 * MEdZ / (num4 / GammaM1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EC3 test functions", Name = "Torsion_Buckling_Elastic_Critical_Force", Description = "Determines the elastic critical torsion buckling load Ncr,T in kN (cl. 6.3.1)")]
        public static object TorsionBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "LcrT (m)", Description = "Buckling length in m. Generally taken as the member length")] double LcrT, [ExcelArgument(Name = "E (N/sq.mm)", Description = "Elastic modulus in N/sq.mm (210000N/sq.mm recommended in EC3)")] double E, [ExcelArgument(Name = "v", Description = "Poisson's ratio")] double nu)
        {
            object result;
            try
            {
                LcrT *= 1000.0;
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section not doubly-symmetric");
                }
                double x = structBaseProperties.iy * 1000.0;
                double x2 = structBaseProperties.iz * 1000.0;
                double num = Math.Pow(x, 2.0) + Math.Pow(x2, 2.0);
                double num2 = E / (2.0 * (1.0 + nu));
                double num3 = structBaseProperties.Iw * 1E+18;
                double num4 = structBaseProperties.It * 1000000000000.0;
                double arg_AD_0 = structBaseProperties.A;
                result = (num2 * num4 + Math.Pow(3.1415926535897931, 2.0) * E * num3 / Math.Pow(LcrT, 2.0)) / num / 1000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [ExcelFunction(Category = "EC3 test functions", Name = "Torsion_Flexural_Buckling_Elastic_Critical_Force", Description = "Determines the elastic critical torsion-flexural buckling load Ncr,TF in kN (cl. 6.3.1)")]
        public static object TorsionFlexuralBucklingCriticalElasticForce([ExcelArgument(Name = "Section", Description = "Section catalogue name. Input as string")] string SectionDenomination, [ExcelArgument(Name = "Lcr (m)", Description = "Compression buckling length in m. See BS5950 for reference")] double Lcr, [ExcelArgument(Name = "LcrT (m)", Description = "Torsional buckling length in m. See EN1993-1-3 for guidance")] double LcrT, [ExcelArgument(Name = "E (N/sq.mm)", Description = "Elastic modulus in N/sq.mm (210000N/sq.mm recommended in EC3)")] double E, [ExcelArgument(Name = "v", Description = "Poisson's ratio")] double nu, [ExcelArgument(Name = "Bending axis", Description = "Bending Axis, ''YY'' or ''ZZ''. Input as string")] string Axis)
        {
            object result;
            try
            {
                StructBaseProperties structBaseProperties = RetrieveSectionProps(SectionDenomination);
                if (structBaseProperties.Symmetry != Symmetry.DOUBLYSYMMETRIC)
                {
                    throw new GeneralException("Section not doubly-symmetric");
                }
                double x = structBaseProperties.iy * 1000.0;
                double x2 = structBaseProperties.iz * 1000.0;
                double num = Math.Pow(x, 2.0) + Math.Pow(x2, 2.0);
                double num2 = 0.0;
                double num3 = 1.0 - num2 / num;
                double num4 = double.Parse(CompressionBucklingCriticalElasticForce(SectionDenomination, Lcr, Axis, E).ToString()) * 1000.0;
                double num5 = double.Parse(TorsionBucklingCriticalElasticForce(SectionDenomination, LcrT, E, nu).ToString()) * 1000.0;
                result = num4 / (2.0 * num3) * (1.0 + num5 / num4 - Math.Sqrt(Math.Pow(1.0 - num5 / num4, 2.0) + 4.0 * Math.Pow(num2 / num, 2.0) * num5 / num4)) / 1000.0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
