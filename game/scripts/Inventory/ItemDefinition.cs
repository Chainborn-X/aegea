namespace Aegea;

public enum ItemKind
{
    Material,
    Consumable,
    Key,
    Weapon
}

public sealed record ItemDefinition(string Id, string DisplayName, ItemKind Kind, string Description);
