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

        public readonly Region Region;

        public void AddSectionCollection(SectionCollection sectionCollection)
        {
            string sectionCollectionDenomination = sectionCollection.Denomination.ToLower();

            if (this.sectionCollections.ContainsKey(sectionCollectionDenomination))
            {
                throw new Exception($"Section collection {sectionCollectionDenomination} already exists");
            }

            this.sectionCollections.Add(sectionCollectionDenomination, sectionCollection);
        }

        public SectionCollection GetSectionCollection(string sectionDenomination)
        {
            return this.sectionCollections[sectionDenomination];
        }

        public SectionBase GetSection(string SectionType, string SectionDenomination)
        {
            return this.GetSection<SectionBase>(SectionType, SectionDenomination);
        }

        public T GetSection<T>(string SectionType, string SectionDenomination) 
            where T : SectionBase
        {
            return this.GetSectionCollection(SectionType).GetSection<T>(SectionDenomination);
        }
    }
}