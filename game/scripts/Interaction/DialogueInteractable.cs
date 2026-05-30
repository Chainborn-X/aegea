using Godot;

namespace Aegea;

public partial class DialogueInteractable : Area2D, IInteractable
{
    [Export] public string Speaker { get; set; } = "Oracle Stone";
    [Export] public string Prompt { get; set; } = "Speak";
    [Export] public string[] Lines { get; set; } = [];

    public override void _Ready()
    {
        Monitorable = true;
        Monitoring = false;
        AddInteractionShape(42f);
    }

    public override void _Draw()
    {
        bool sign = Speaker.Contains("Sign");
        if (sign)
        {
            DrawLine(new Vector2(0, 14), new Vector2(0, -10), new Color(0.35f, 0.23f, 0.12f), 5f);
            DrawRect(new Rect2(-22, -24, 44, 18), new Color(0.58f, 0.38f, 0.2f), true);
            DrawRect(new Rect2(-22, -24, 44, 18), new Color(0.18f, 0.1f, 0.04f), false, 2f);
            return;
        }

        if (Speaker == "Myrine")
        {
            DrawCircle(new Vector2(0, -11), 7f, new Color(0.78f, 0.58f, 0.38f));
            DrawRect(new Rect2(-8, -4, 16, 21), new Color(0.78f, 0.66f, 0.42f), true);
            DrawRect(new Rect2(-8, -4, 16, 21), new Color(0.22f, 0.2f, 0.16f), false, 2f);
            DrawLine(new Vector2(-4, 17), new Vector2(-7, 25), new Color(0.18f, 0.18f, 0.16f), 3f);
            DrawLine(new Vector2(4, 17), new Vector2(7, 25), new Color(0.18f, 0.18f, 0.16f), 3f);
            return;
        }

        DrawCircle(Vector2.Zero, 18f, new Color(0.52f, 0.57f, 0.55f));
        DrawCircle(new Vector2(0, -2), 10f, new Color(0.66f, 0.73f, 0.68f));
        DrawArc(Vector2.Zero, 20f, 0, Mathf.Tau, 24, new Color(0.25f, 0.28f, 0.27f), 2f);
        DrawLine(new Vector2(-8, -2), new Vector2(8, -2), new Color(0.9f, 0.8f, 0.42f), 2f);
    }

    public string GetPrompt() => Prompt;

    public void Interact(PlayerController player)
    {
        GameManager.Instance?.Dialogue.StartDialogue(Speaker, Lines);
    }

    protected void AddInteractionShape(float radius)
    {
        var shape = new CollisionShape2D
        {
            Shape = new CircleShape2D { Radius = radius }
        };
        AddChild(shape);
    }
}
