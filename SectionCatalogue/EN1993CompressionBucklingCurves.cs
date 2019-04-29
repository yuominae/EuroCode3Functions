using System;
namespace SectionCatalogue
{
    public enum EN1993CompressionBucklingCurves
    {
        a0,
        a,
        b,
        c,
        d
    }

    public class EN1993CompressionBucklingCurve
    {
        public EN1993CompressionBucklingCurves majorAxis  { get; set; } = EN1993CompressionBucklingCurves.d;

        public EN1993CompressionBucklingCurves minorAxis  { get; set; } = EN1993CompressionBucklingCurves.d;
    }
}