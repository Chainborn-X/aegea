using Godot;

namespace Aegea;

public partial class Pickup : Area2D
{
    [Export] public string ItemId { get; set; } = "bronze_coin";
    [Export] public int Amount { get; set; } = 1;

    private float _bobTime;

    public override void _Ready()
    {
        Monitoring = true;
        Monitorable = true;
        BodyEntered += OnBodyEntered;
        AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = 10 } });
    }

    public override void _Process(double delta)
    {
        _bobTime += (float)delta;
        Position += new Vector2(0, Mathf.Sin(_bobTime * 8f) * 0.02f);
        QueueRedraw();
    }

    public override void _Draw()
    {
        ItemDefinition item = ItemCatalog.Get(ItemId);
        Color color = item.Kind switch
        {
            ItemKind.Consumable => new Color(0.32f, 0.85f, 0.38f),
            ItemKind.Key => new Color(0.98f, 0.76f, 0.22f),
            _ => new Color(0.88f, 0.55f, 0.24f)
        };

        DrawCircle(Vector2.Zero, 7f, color);
        DrawCircle(Vector2.Zero, 8.5f, new Color(1f, 1f, 1f, 0.35f));
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController player)
        {
            return;
        }

        player.Inventory.Add(ItemId, Amount);
        GameManager.Instance?.ShowNotification($"+{Amount} {ItemCatalog.Get(ItemId).DisplayName}");
        QueueFree();
    }
}
