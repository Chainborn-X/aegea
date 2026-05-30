using Godot;

namespace Aegea;

public partial class DestructiblePlant : StaticBody2D
{
    [Export] public string PlantKind { get; set; } = "grass";
    [Export] public string DropItemId { get; set; } = "";
    [Export] public float DropChance { get; set; } = 0.35f;

    private Damageable _health = null!;
    private bool _cut;
    private float _shakeTimer;

    public override void _Ready()
    {
        CollisionLayer = 1;
        CollisionMask = uint.MaxValue;
        AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = PlantKind == "bush" ? 13f : 9f } });

        _health = new Damageable { Name = "Health", MaxHealth = 1, Team = "Neutral", InvulnerabilitySeconds = 0f };
        AddChild(_health);
        _health.Damaged += OnDamaged;
        _health.Died += OnDied;
    }

    public override void _Process(double delta)
    {
        _shakeTimer = Mathf.Max(0, _shakeTimer - (float)delta);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_cut)
        {
            DrawCircle(Vector2.Zero, PlantKind == "bush" ? 8f : 5f, new Color(0.31f, 0.38f, 0.16f, 0.45f));
            return;
        }

        float shake = _shakeTimer > 0 ? Mathf.Sin(Time.GetTicksMsec() * 0.04f) * 2f : 0f;
        Color fill = PlantKind == "bush" ? new Color(0.22f, 0.48f, 0.24f) : new Color(0.36f, 0.68f, 0.25f);
        float radius = PlantKind == "bush" ? 15f : 10f;
        DrawCircle(new Vector2(shake, 0), radius, fill);
        DrawCircle(new Vector2(shake - 5, -3), radius * 0.55f, new Color(0.49f, 0.76f, 0.34f));
        DrawLine(new Vector2(shake, 4), new Vector2(shake + 3, -8), new Color(0.16f, 0.29f, 0.12f), 2f);
    }

    private void OnDamaged(int amount, Vector2 knockback)
    {
        _shakeTimer = 0.08f;
    }

    private void OnDied()
    {
        if (_cut)
        {
            return;
        }

        _cut = true;
        SetCollisionLayerValue(1, false);
        CreateCutBurst();

        if (!string.IsNullOrWhiteSpace(DropItemId) && GD.Randf() <= DropChance)
        {
            GameManager.Instance?.SpawnPickup(GlobalPosition + new Vector2(0, -6), DropItemId, 1);
        }

        QueueRedraw();
    }

    private void CreateCutBurst()
    {
        var burst = new CutBurst { GlobalPosition = GlobalPosition };
        GetTree().CurrentScene.AddChild(burst);
    }
}
