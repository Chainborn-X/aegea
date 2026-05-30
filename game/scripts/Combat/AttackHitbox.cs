using Godot;
using System.Collections.Generic;

namespace Aegea;

public partial class AttackHitbox : Area2D
{
    [Export] public int Damage { get; set; } = 1;
    [Export] public float KnockbackForce { get; set; } = 230f;
    [Export] public string IgnoredTeam { get; set; } = "Player";

    private readonly HashSet<ulong> _hitInstanceIds = new();
    private CollisionShape2D _shape = null!;
    private RectangleShape2D _rectangle = null!;
    private Vector2 _direction = Vector2.Down;
    private float _activeTimer;
    private Node? _source;

    public bool IsActive => _activeTimer > 0;

    public override void _Ready()
    {
        Monitoring = false;
        Monitorable = false;
        CollisionLayer = 0;
        CollisionMask = uint.MaxValue;

        _rectangle = new RectangleShape2D { Size = new Vector2(42, 30) };
        _shape = new CollisionShape2D { Shape = _rectangle, Disabled = true };
        AddChild(_shape);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_activeTimer <= 0)
        {
            return;
        }

        _activeTimer -= (float)delta;
        ScanBodyOverlaps(GetOverlappingBodies());
        ScanAreaOverlaps(GetOverlappingAreas());

        if (_activeTimer <= 0)
        {
            Monitoring = false;
            _shape.Disabled = true;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (!IsActive)
        {
            return;
        }

        var color = new Color(1f, 0.86f, 0.34f, 0.38f);
        DrawRect(new Rect2(-_rectangle.Size / 2f, _rectangle.Size), color, true);
        DrawRect(new Rect2(-_rectangle.Size / 2f, _rectangle.Size), new Color(1f, 0.95f, 0.65f, 0.8f), false, 2f);
    }

    public void Activate(Node source, Vector2 direction, float activeSeconds)
    {
        _source = source;
        _direction = direction.LengthSquared() > 0.001f ? direction.Normalized() : Vector2.Down;
        _hitInstanceIds.Clear();
        _activeTimer = activeSeconds;

        bool horizontal = Mathf.Abs(_direction.X) >= Mathf.Abs(_direction.Y);
        _rectangle.Size = horizontal ? new Vector2(48, 32) : new Vector2(34, 48);
        Position = _direction * 26f;

        Monitoring = true;
        _shape.Disabled = false;
        QueueRedraw();
    }

    private void ScanBodyOverlaps(Godot.Collections.Array<Node2D> nodes)
    {
        foreach (Node2D node in nodes)
        {
            TryDamage(node);
        }
    }

    private void ScanAreaOverlaps(Godot.Collections.Array<Area2D> nodes)
    {
        foreach (Node2D node in nodes)
        {
            TryDamage(node);
        }
    }

    private void TryDamage(Node2D node)
    {
        if (_source is not null && (node == _source || node.IsAncestorOf(_source)))
        {
            return;
        }

        Damageable? damageable = CombatUtility.FindDamageable(node);
        if (damageable is null || damageable.Team == IgnoredTeam || damageable.IsDead)
        {
            return;
        }

        ulong id = damageable.GetInstanceId();
        if (_hitInstanceIds.Contains(id))
        {
            return;
        }

        _hitInstanceIds.Add(id);
        Vector2 knockback = _direction * KnockbackForce;
        damageable.ApplyDamage(Damage, _source, knockback);
    }
}
