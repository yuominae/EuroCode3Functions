using SectionCatalogue.SectionProperties;

namespace SectionCatalogue
{
    public interface ISectionCollection
    {
        string Denomination { get; }

        string Abbreviation { get; }

        SectionBase GetSection(string sectionDenomination);
    }
}