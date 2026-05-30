using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Aegea;

public partial class InventoryComponent : Node
{
    [Signal] public delegate void InventoryChangedEventHandler();
    [Signal] public delegate void ItemAddedEventHandler(string itemId, int amount);

    private readonly Dictionary<string, int> _items = new();
    private readonly Dictionary<string, string> _equipmentSlots = new()
    {
        ["weapon"] = "reed_sword",
        ["relic"] = ""
    };

    public string EquippedWeaponId { get; private set; } = "reed_sword";

    public IReadOnlyDictionary<string, int> Items => _items;
    public IReadOnlyDictionary<string, string> EquipmentSlots => _equipmentSlots;

    public override void _Ready()
    {
        Add(EquippedWeaponId, 1, notify: false);
    }

    public void Add(string itemId, int amount, bool notify = true)
    {
        if (amount <= 0)
        {
            return;
        }

        _items[itemId] = _items.GetValueOrDefault(itemId) + amount;
        if (notify)
        {
            EmitSignal(SignalName.ItemAdded, itemId, amount);
        }
        EmitSignal(SignalName.InventoryChanged);
    }

    public bool Has(string itemId, int amount = 1)
    {
        return _items.GetValueOrDefault(itemId) >= amount;
    }

    public bool Consume(string itemId, int amount = 1)
    {
        if (!Has(itemId, amount))
        {
            return false;
        }

        _items[itemId] -= amount;
        if (_items[itemId] <= 0)
        {
            _items.Remove(itemId);
        }

        EmitSignal(SignalName.InventoryChanged);
        return true;
    }

    public bool UseHealingItem(Damageable health)
    {
        const string herbId = "sunleaf_herb";
        if (!Consume(herbId))
        {
            return false;
        }

        health.Heal(2);
        GameManager.Instance?.ShowNotification("Used Sunleaf Herb");
        return true;
    }

    public Dictionary<string, int> Snapshot()
    {
        return _items.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public void Restore(Dictionary<string, int> items)
    {
        _items.Clear();
        foreach ((string id, int amount) in items)
        {
            if (amount > 0)
            {
                _items[id] = amount;
            }
        }

        if (!Has(EquippedWeaponId))
        {
            _items[EquippedWeaponId] = 1;
        }

        EmitSignal(SignalName.InventoryChanged);
    }
}
