using System.Collections.Generic;

namespace Aegea;

public static class ItemCatalog
{
    private static readonly Dictionary<string, ItemDefinition> Items = new()
    {
        ["bronze_coin"] = new("bronze_coin", "Bronze Coin", ItemKind.Material, "An old coin greened by sea air."),
        ["sunleaf_herb"] = new("sunleaf_herb", "Sunleaf Herb", ItemKind.Consumable, "A bright herb that restores a little health."),
        ["laurel_sprig"] = new("laurel_sprig", "Laurel Sprig", ItemKind.Material, "A fragrant sprig used in offerings."),
        ["old_shrine_key"] = new("old_shrine_key", "Old Shrine Key", ItemKind.Key, "A bronze key marked with a broken sun."),
        ["reed_sword"] = new("reed_sword", "Reed-Bronze Sword", ItemKind.Weapon, "A plain blade with a wave-worn grip.")
    };

    public static ItemDefinition Get(string id)
    {
        return Items.TryGetValue(id, out ItemDefinition? definition)
            ? definition
            : new ItemDefinition(id, id, ItemKind.Material, "");
    }
}
