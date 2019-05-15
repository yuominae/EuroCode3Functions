using System;
using System.Collections.Generic;
using SectionCatalogue.SectionProperties;

namespace SectionCatalogue
{
    public class SectionCatalogue
    {
        readonly Dictionary<string, SectionCollection> sectionCollections = new Dictionary<string, SectionCollection>();

        public SectionCatalogue(Region region)
        {
            this.Region = region;
        }

        public Region Region { get; }

        public void AddSectionCollection(SectionCollection sectionCollection)
        {
            string sectionCollectionDenomination = sectionCollection.Denomination.ToLower();

            if (this.sectionCollections.ContainsKey(sectionCollectionDenomination))
            {
                throw new Exception($"Section collection {sectionCollectionDenomination} already exists");
            }

            this.sectionCollections.Add(sectionCollectionDenomination, sectionCollection);
        }

        public SectionCollection GetSectionCollection(string sectionCollectionDenomination)
        {
            return this.sectionCollections[sectionCollectionDenomination];
        }
    }
}