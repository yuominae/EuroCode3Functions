using System;
using System.Collections.Generic;
using System.Linq;
using SectionCatalogue.StructProperties;

namespace SectionCatalogue
{
    public class SectionRegionalCollection
    {
        public Dictionary<string, SectionTypeCollection> Sections { get; private set; }

        public StructuralSectionsRegion Region { get; private set; }

        public SectionRegionalCollection(StructuralSectionsRegion Region)
        {
            this.Sections = new Dictionary<string, SectionTypeCollection>();
            this.Region = Region;
        }

        /// <summary>
        /// Adds the specified section type to the library
        /// </summary>
        /// <param name="Sections">The collection of sections to add to the library</param>
        public void AddSectionType(SectionTypeCollection Sections)
        {
            if (this.Sections.ContainsKey(Sections.Denomination))
                throw new Exception(string.Format("Section library {0} already exists", Sections.Denomination));
            this.Sections.Add(Sections.Denomination, Sections);
        }

        /// <summary>
        /// Retrieves all sections of the specified type from the collection
        /// </summary>
        /// <param name="SectionType">The type of section to retrieve</param>
        public SectionTypeCollection GetSectionType(string SectionType)
        {
            return this.Sections[SectionType];
        }

        /// <summary>
        /// Retrieves the section of the specified type and title
        /// </summary>
        /// <param name="SectionType">The type of section to retrieve</param>
        /// <param name="SectionDenomination">The denomation of the section to retrieve</param>
        /// <returns></returns>
        internal StructBaseProperties GetSection(string SectionType, string SectionDenomination)
        {
            return this.GetSectionType(SectionType).GetSection(SectionDenomination);
        }
    }
}
