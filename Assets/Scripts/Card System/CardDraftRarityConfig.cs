using System.Collections.Generic;

public static class CardDraftRarityConfig
{
    public static Dictionary<DraftType, CardRarity[]> Pools = new Dictionary<DraftType, CardRarity[]>
    {
        { DraftType.Major, new[] { CardRarity.Uncommon, CardRarity.Rare, CardRarity.Epic, CardRarity.Exalted } },
        { DraftType.Mini, new[] { CardRarity.Common, CardRarity.Uncommon, CardRarity.Rare } }
    };
}