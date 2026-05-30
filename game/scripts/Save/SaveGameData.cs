using System.Collections.Generic;

namespace Aegea;

public sealed class SaveGameData
{
    public float PlayerX { get; set; }
    public float PlayerY { get; set; }
    public int PlayerHealth { get; set; }
    public Dictionary<string, int> Inventory { get; set; } = new();
    public Dictionary<string, bool> Objectives { get; set; } = new();
}
