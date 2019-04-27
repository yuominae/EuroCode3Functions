using System;
using System.Collections.Generic;
using System.Text;
using SSI.SectionCatalogue.StructProperties;
using SSI.SectionCatalogue;


namespace EC3Functions
{
	public static partial class EC3Functions
    {
        /*
         * Combined bending and axial checks to BS:EN1993-1 cl. 6.2.9
         * These are the cross-section capacity checks. Currently only implemented for class 1 and 2 I sections as these are handled explicitly in the code
         * and are simpler. Class 3 and 4 sections have to undergo different checks.
         * The checks dpending on whether the sections work under high shear or not. (see end of code)
         */

        public static object CombinedAxialAndBendingChecks(bool verbose,string SectionDenomination,
            object SteelGrade,double NEd,double VyEd,double VzEd,double MyEd,double MzEd,double GammaM0,
            int CompClass,int BendClass,string strCheckType)
        {
            try
            {
                if (GammaM0 == 0)
                    GammaM0 = 1;

                if (strCheckType.ToUpper() != "ADVANCED")
                    strCheckType = "CONSERVATIVE";
                
                //Determine design yield stress
				double Designfy;
				if (!double.TryParse(SteelGrade.ToString(), out Designfy))
					Designfy = double.Parse(SectionDesignStrength(SectionDenomination, SteelGrade.ToString()).ToString());

                StructBaseProperties ThisSectionBaseProps = (StructBaseProperties)RetrieveSectionProps(SectionDenomination);
				
               
                   // IBaseProp baseProp = (IBaseProp)RetrieveSectionProps(SectionDenomination);


                    //Checks and determines the section compression class
                if (CompClass < 0 || CompClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (CompClass == 0)
                        CompClass = ThisSectionBaseProps.getEN1993CompressionClass(Designfy);// ThisSectionBaseProps.getEN1993CompressionClass(Designfy);
                    if (CompClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }

                //Checks and determines the section bending class
                if (BendClass < 0 || BendClass > 4)
                    throw new CustomExceptions.InvalidSectionClassException();
                else
                {
                    if (BendClass == 0)
                        BendClass = ThisSectionBaseProps.getEN1993FlexureClass(Designfy);// ThisSectionBaseProps.getEN1993FlexureClass(Designfy);
                    if (BendClass == 4)
                        throw new CustomExceptions.Class4SectionException();
                }
                
                int intSectClass = Math.Max(CompClass, BendClass);

                SectionTypes ThisSectionType = (SectionTypes)DetermineType(SectionDenomination);
                double NRd = double.Parse(CrossSectionCompressionResistance(SectionDenomination, SteelGrade, GammaM0, CompClass).ToString()) * 1e3;
                double MyRd = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "YY", VzEd, GammaM0, BendClass).ToString()) * 1e6;
                double MzRd = double.Parse(CrossSectionBendingResistance(SectionDenomination, SteelGrade, "ZZ", VyEd, GammaM0, BendClass).ToString()) * 1e6;
                double VyPlRd = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Y", GammaM0).ToString()) * 1e3;
                double VzPlRd = double.Parse(CrossSectionPlasticShearResistance(SectionDenomination, SteelGrade, "Z", GammaM0).ToString()) * 1e3;
                NEd *= 1e3; VyEd *= 1e3; VzEd *= 1e3;
                MyEd *= 1e6; MzEd *= 1e6;

                //Dealing with class 1 and 2 cross-sections
					if (intSectClass < 3)
                {
                    //Dealing with the case where only one of the actions applies
						if (NEd != 0 && MyEd == 0 && MzEd == 0)
					{
						if (verbose)
							return "Ned / NRd : " + NEd + " / " + NRd;
						return NEd / NRd;
					}
					if (NEd == 0 && MyEd != 0 && MzEd == 0)
					{
						if (verbose)
							return "MyEd / MyRd : " + MyEd + " / " + MyRd;
						return MyEd / MyRd;
					}
					if (NEd == 0 && MyEd == 0 && MzEd != 0)
					{
						if (verbose)
							return "MzEd / MzRd : " + MzEd + " / " + MzRd;
						return MzEd / MzRd;
					}

                    double MNyRd = MyRd;
                    double MNzRd = MzRd;

                    double b = 0; double h = 0; double tf = 0; double tw = 0; double t = 0; double hw = 0;
                    double a = 0; double aw = 0; double af = 0;
                    double n = NEd / NRd; ;
                    double A = ThisSectionBaseProps.A * 1e6;

                    //Check if the moment resistance needs to be reduced (cl. 6.2.9(4) and 6.2.9(5))
                    switch (ThisSectionType)
                    {
						/*
						 * Dealing with I sections and the I without web variant 
						 */ 
                        case SectionTypes.I:
                        case SectionTypes.INOWEB:
                            StructIProp ThisISectionProps = (StructIProp)RetrieveISectionProps(SectionDenomination);
                            h = ThisISectionProps.h * 1e3; b = ThisISectionProps.b * 1e3; tf = ThisISectionProps.tf * 1e3; tw = ThisISectionProps.tw * 1e3;
                            hw = h - 2 * tf;

                            a = (A - 2 * b * tf) / A;
                            if (a > 0.5)
                                a = 0.5;

                            if (NEd > 0.25 * NRd || NEd > 0.5 * hw * tw * Designfy / GammaM0) //Check if reduction of major axis bending capacity necessary
                            {
                                MNyRd = MyRd * (1 - n) / (1 - 0.5 * a);
                            }

                            if (NEd > hw * tw * Designfy / GammaM0) //Check if reduction of minor axis bending capacity necessary
                            {
                                MNzRd = MzRd;
                                if (n > a)
                                    MNzRd = MzRd * (1 - Math.Pow((n - a) / (1 - a), 2));
                            }
                            break;
                        case SectionTypes.RHS:
                            StructRHSProp ThisRHSSectionProps = (StructRHSProp)RetrieveRHSProps(SectionDenomination);
                            b = ThisRHSSectionProps.b * 1e3; h = ThisRHSSectionProps.h * 1e3; t = ThisRHSSectionProps.t * 1e3;
                            hw = h - 2 * tf;

                            aw = (A - 2 * b * t) / A;
                            af = (A - 2 * h * t) / A;

                            if (NEd > 0.25 * NRd || NEd > 0.5 * (2 * hw * t) * Designfy / GammaM0) //Check if reduction of major axis bending capacity necessary
                            {
                                MNyRd = MyRd * (1 - n) / (1 - aw / 2);
                                MNzRd = MzRd * (1 - n) / (1 - af / 2);
                            }
                            break;
						case SectionTypes.CHS:
							{
								StructCHSProp ThisCHSSectionProps = (StructCHSProp)RetrieveCHSProps(SectionDenomination);

                                MNyRd = MyRd * Math.Cos(n * Math.PI / 2);
                                MNzRd = MzRd * Math.Cos(n * Math.PI / 2);
                            }
							break;
                        case SectionTypes.BOX:
                            StructWeldedBoxProp ThisBoxSectionProps = (StructWeldedBoxProp)RetrieveWELDEDBOXProps(SectionDenomination);
                            b = ThisBoxSectionProps.b * 1e3; h = ThisBoxSectionProps.h * 1e3; tf = ThisBoxSectionProps.tf * 1e3; tw = ThisBoxSectionProps.tw * 1e3;
                            hw = h - 2 * tf;

                            aw = (A - 2 * b * tw) / A;
                            af = (A - 2 * h * tf) / A;

                            if (NEd > 0.25 * NRd || NEd > 0.5 * (2 * hw * tw) * Designfy / GammaM0) //Check if reduction of major axis bending capacity necessary
                            {
                                MNyRd = MyRd * (1 - n) / (1 - aw / 2);
                                MNzRd = MzRd * (1 - n) / (1 - af / 2);
                            }
                            break;
                        default:
                            throw new CustomExceptions.SectionNotYetHandledException();
                    }


                    /*
                     * NOTE: If the shear force on the section exceeds half the shear capacity of the section, the subsequent
                     * checks default to linear interaction checks for class 1 and 2 cross-sections (class 3 and 4 cross-sections undergo a linear
                     * interaction check in any case). If shear is low, the axial force does not need to be included for uniaxial bending
                     * whilst for biaxial bending the non-linear interaction check for bending capacities can be used.
                     * Note that I could not find the restriction to linear checks for sections under high shear explicitly stated in EC3,
                     * but the Quick EC3 software appears to do it this way, and this is a safer alternative than using the non-linear checks and
                     * ignoring acial stresses.
                     */

                    if (MyEd != 0 && MzEd == 0) //Uniaxial bending major axis moment only
                    {
						if (VyEd <= VyPlRd / 2)
						{
							if (verbose)
								return "MyEd / MNyRd : " + MyEd + " / " + MNyRd;
							return MyEd / MNyRd;
						}
						if (verbose)
							return "NEd / NRd + MyEd / MNyRd : (" + NEd + " / " + NRd +") + (" + MyEd + " / " + MNyRd + ")";
                        return NEd / NRd + MyEd / MNyRd;
                    }

                    if (MyEd == 0 && MzEd != 0) //Uniaxial bending minor axis moment only
                    {
						if (VzEd <= VzPlRd / 2)
						{
							if (verbose)
								return "MzEd / MNzRd : " + MzEd + " / " + MNzRd;
							return MzEd / MNzRd;
						}
						if (verbose)
							return "NEd / NRd + MzEd / MNzRd : (" + NEd + " / " + NRd + ") + (" + MzEd + " / " + MNzRd + ")";
                        return NEd / NRd + MzEd / MNzRd;
                    }

                    if (MyEd != 0 && MzEd != 0) //Biaxial bending
                    {
                        double dblalpha = 1; double dblbeta = 1;
						if ((VyEd <= VyPlRd / 2 && VzEd <= VzPlRd / 2) && (strCheckType.ToUpper() == "ADVANCED"))
						{
							switch (ThisSectionType)
							{
								case SectionTypes.I:
									dblalpha = 2; dblbeta = 5 * n;
									if (dblbeta < 1)
										dblbeta = 1;
									break;
								case SectionTypes.CHS:
									dblalpha = 2;
									dblbeta = dblalpha;
									break;
								case SectionTypes.RHS:
									dblalpha = 1.66 / (1 - 1.13 * Math.Pow(n, 2));
									if (dblalpha > 6)
										dblalpha = 6;
									dblbeta = dblalpha;
									break;
								default:
									throw new CustomExceptions.SectionNotYetHandledException();
							}
							if (verbose)
								return "(MyEd / MNyRd) ^ dblalpha + (MzEd / MNzRd) ^ dblbeta : (" + MyEd + " / " + MNyRd + ") ^ " + dblalpha + " + (" + MzEd + " / " + MNzRd + ") ^ " + dblbeta;

							return Math.Pow(MyEd / MNyRd, dblalpha) + Math.Pow(MzEd / MNzRd, dblbeta); //Advanced interaction formula
						}
						else
						{
							if (verbose)
								return "NEd / NRd + MyEd / MNyRd + MzEd / MNzRd : " + NEd + " / " + NRd + " + " + MyEd + " / " + MNyRd + " + " + MzEd + " / " + MNzRd;
								return NEd / NRd + MyEd / MyRd + MzEd / MzRd;
						}
                    }

                    throw new CustomExceptions.GeneralException("Unable to calculate interaction check");

                }
                else if (intSectClass == 3) //Dealing with class 3 cross sections
                {
                    /*
                     * For class 3 cross-sections, the stress in the extreme fibres is
                     * limited to the design yield stress of the section
                    */
                    double A = ThisSectionBaseProps.A * 1e6; double Welyy = ThisSectionBaseProps.Welyy * 1e9; double Welzz = ThisSectionBaseProps.Welzz * 1e9;

                    double dblMaxStress = NEd / A + MyEd / Welyy + MzEd / Welzz;
					if (verbose)
						return "GammaM0 * dblMaxStress / Designfy : " + GammaM0 + " * " + dblMaxStress + " / " + Designfy;

                    return GammaM0 * dblMaxStress / Designfy;
                }
                else
                    throw new CustomExceptions.Class4SectionException();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
