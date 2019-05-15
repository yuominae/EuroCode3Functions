using System;
namespace SectionCatalogue.SectionProperties
{
    public abstract class SectionBase
    {
        public virtual string Denomination { get; set; }

        public virtual double Weight { get; set; }

        public virtual double A { get; set; }

        public virtual double Iyy { get; set; }

        public virtual double Welyy { get; set; }

        public virtual double Wplyy { get; set; }

        public virtual double iy { get; set; }

        public virtual double Avz { get; set; }

        public virtual double Izz { get; set; }

        public virtual double Welzz { get; set; }

        public virtual double Wplzz { get; set; }

        public virtual double iz { get; set; }

        public virtual double It { get; set; }

        public virtual double Iw { get; set; }

        public virtual Symmetry Symmetry { get; set; } = Symmetry.Asymmetric;

        public virtual EN1993CompressionBucklingCurve GetEN1993CompressionBucklingCurves(EN1993SteelGrade steelGrade) => throw new NotImplementedException();

        public virtual EN1993LTBBucklingCurve GetEN1993LTBBucklingCurve() => throw new NotImplementedException();

        public virtual int GetEN1993CompressionClass(double fy) => throw new NotImplementedException();

        public virtual int GetEN1993FlexureClass(double fy) => throw new NotImplementedException();

        public virtual Av GetEN1993ShearArea(double fy) => throw new NotImplementedException();

        protected internal double CalcEpsilon(double fy) => Math.Sqrt(235.0 / fy);
    }
}
