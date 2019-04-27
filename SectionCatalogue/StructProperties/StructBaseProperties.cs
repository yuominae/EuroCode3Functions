﻿using System;
namespace SectionCatalogue.StructProperties
{
    public class StructBaseProperties
    {
        public virtual string Denomination { get; set; }

        public virtual double weight { get; set; }

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

        public virtual Symmetry Symmetry { get; set; }

        public StructBaseProperties()
        {
            this.Symmetry = Symmetry.ASYMMETRIC;
        }

        public virtual EN1993CompressionBucklingCurve getEN1993CompressionBucklingCurves(EN1993SteelGrade grade)
        {
            throw new NotImplementedException();
        }

        public virtual EN1993LTBBucklingCurve getEN1993LTBBucklingCurve()
        {
            throw new NotImplementedException();
        }

        public virtual int getEN1993CompressionClass(double fy)
        {
            throw new NotImplementedException();
        }

        public virtual int getEN1993FlexureClass(double fy)
        {
            throw new NotImplementedException();
        }

        public virtual Av getEN1993ShearArea(double fy)
        {
            throw new NotImplementedException();
        }

        protected internal double calcEpsilon(double fy)
        {
            return Math.Sqrt(235.0 / fy);
        }
    }
}