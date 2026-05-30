using Godot;

namespace Aegea;

public partial class ShrineGateInteractable : Area2D, IInteractable
{
    private bool _opened;

    public override void _Ready()
    {
        Monitorable = true;
        Monitoring = false;
        AddChild(new CollisionShape2D { Shape = new RectangleShape2D { Size = new Vector2(92, 40) } });
    }

    public override void _Draw()
    {
        Color color = _opened ? new Color(0.7f, 0.74f, 0.62f, 0.45f) : new Color(0.46f, 0.49f, 0.42f);
        DrawRect(new Rect2(-46, -20, 92, 40), color, true);
        DrawRect(new Rect2(-46, -20, 92, 40), new Color(0.16f, 0.18f, 0.15f), false, 2f);
        DrawCircle(new Vector2(0, 0), 8, new Color(0.95f, 0.82f, 0.36f));
    }

    public string GetPrompt() => _opened ? "Look beyond" : "Inspect";

    public void Interact(PlayerController player)
    {
        if (player.Inventory.Has("old_shrine_key"))
        {
            _opened = true;
            GameManager.Instance?.Dialogue.StartDialogue("Salt-Worn Gate", new[]
            {
                "The old key warms in your hand.",
                "Beyond the gate, pale steps descend toward a drowned road.",
                "This path will open in a later build."
            });
            QueueRedraw();
            return;
        }

        GameManager.Instance?.Dialogue.StartDialogue("Salt-Worn Gate", new[]
        {
            "A weathered lock bears the mark of a broken sun.",
            "Something small and bronze once turned here."
        });
    }
}
