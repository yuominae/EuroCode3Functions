using System;
using System.Collections.Generic;
using System.Linq;
using SectionCatalogue.StructProperties;

namespace SectionCatalogue
{
    public class SectionTypeCollection
    {
        public string Abbreviation { get; private set; }

        public string Denomination { get; private set; }

        public Dictionary<string, StructBaseProperties> SectionTypes { get; private set; }

        public SectionTypeCollection(string Denomination, string Abbreviation)
        {
            this.SectionTypes = new Dictionary<string, StructBaseProperties>();
            this.Denomination = Denomination;
            this.Abbreviation = Abbreviation;
        }

        protected internal void AddSection(StructBaseProperties Section)
        {
            if (this.SectionTypes.ContainsKey(Section.Denomination))
                throw new Exception(string.Format("Section {0} already exists", Section.Denomination));
            this.SectionTypes.Add(Section.Denomination.ToUpper(), Section);
        }

        protected internal StructBaseProperties GetSection(string title)
        {
            return this.SectionTypes[title.ToUpper()];
        }
    }
}
