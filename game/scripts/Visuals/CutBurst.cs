using Godot;

namespace Aegea;

public partial class CutBurst : Node2D
{
    private float _life = 0.28f;

    public override void _Process(double delta)
    {
        _life -= (float)delta;
        if (_life <= 0)
        {
            QueueFree();
            return;
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        float progress = 1f - (_life / 0.28f);
        Color color = new Color(0.74f, 0.9f, 0.34f, 1f - progress);
        for (int i = 0; i < 8; i++)
        {
            Vector2 direction = Vector2.Right.Rotated(i * Mathf.Tau / 8f);
            DrawLine(direction * (4f + progress * 8f), direction * (10f + progress * 24f), color, 2f);
        }
    }
}
