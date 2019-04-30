using System;
using System.Collections.Generic;
using SectionCatalogue.SectionProperties;

namespace SectionCatalogue
{
    public class SectionCollection
    {
        readonly Dictionary<string, SectionBase> sections = new Dictionary<string, SectionBase>();

        public SectionCollection(string denomination, string abbreviation)
        {
            this.Denomination = denomination;

            this.Abbreviation = abbreviation;
        }

        public string Denomination { get; }

        public string Abbreviation { get; }

        public void AddSection(SectionBase Section)
        {
            string sectionDenomination = Section.Denomination.ToLower();

            if (this.sections.ContainsKey(sectionDenomination))
            {
                throw new Exception($"Section {sectionDenomination} already exists");
            }

            this.sections.Add(sectionDenomination, Section);
        }

        public T GetSection<T>(string sectionDenomination)
            where T : SectionBase
        {
            return (T)this.sections[sectionDenomination.ToLower()];
        }
    }
}
