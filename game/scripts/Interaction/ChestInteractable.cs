using Godot;

namespace Aegea;

public partial class ChestInteractable : Area2D, IInteractable
{
    [Export] public string ItemId { get; set; } = "old_shrine_key";
    [Export] public int Amount { get; set; } = 1;

    private bool _opened;

    public override void _Ready()
    {
        Monitorable = true;
        Monitoring = false;
        AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = 36 } });
    }

    public override void _Draw()
    {
        var baseColor = _opened ? new Color(0.45f, 0.35f, 0.25f) : new Color(0.62f, 0.38f, 0.18f);
        DrawRect(new Rect2(-14, -10, 28, 20), baseColor, true);
        DrawRect(new Rect2(-14, -10, 28, 20), new Color(0.18f, 0.1f, 0.05f), false, 2f);
        DrawLine(new Vector2(-12, -2), new Vector2(12, -2), new Color(0.9f, 0.76f, 0.28f), 2f);
    }

    public string GetPrompt() => _opened ? "Empty" : "Open";

    public void Interact(PlayerController player)
    {
        if (_opened)
        {
            GameManager.Instance?.ShowNotification("The cedar chest is empty.");
            return;
        }

        _opened = true;
        player.Inventory.Add(ItemId, Amount);
        GameManager.Instance?.ShowNotification($"Found {ItemCatalog.Get(ItemId).DisplayName}");
        GameManager.Instance?.Quests.Complete("find_shrine_key");
        QueueRedraw();
    }
}
