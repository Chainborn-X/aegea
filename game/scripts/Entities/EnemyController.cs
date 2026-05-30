using Godot;

namespace Aegea;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Return,
    Dead
}

public partial class EnemyController : CharacterBody2D
{
    [Export] public float PatrolRadius { get; set; } = 72f;
    [Export] public float DetectRadius { get; set; } = 190f;
    [Export] public float ResetRadius { get; set; } = 320f;
    [Export] public float MoveSpeed { get; set; } = 80f;
    [Export] public int ContactDamage { get; set; } = 1;

    public Damageable Health { get; private set; } = null!;

    private Vector2 _home;
    private Vector2 _patrolTarget;
    private EnemyState _state = EnemyState.Idle;
    private float _thinkTimer;
    private float _contactCooldown;
    private float _flashTimer;
    private Vector2 _knockbackVelocity;

    public override void _Ready()
    {
        _home = GlobalPosition;
        _patrolTarget = _home;
        CollisionLayer = 1;
        CollisionMask = uint.MaxValue;
        AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = 12 } });

        Health = new Damageable { Name = "Health", MaxHealth = 3, Team = "Enemy", InvulnerabilitySeconds = 0.18f };
        AddChild(Health);
        Health.Damaged += OnDamaged;
        Health.Died += OnDied;
    }

    public override void _Process(double delta)
    {
        _flashTimer = Mathf.Max(0, _flashTimer - (float)delta);
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        _contactCooldown = Mathf.Max(0, _contactCooldown - dt);
        if (_state == EnemyState.Dead)
        {
            return;
        }

        PlayerController? player = GameManager.Instance?.Player;
        if (player is null)
        {
            return;
        }

        float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
        float distanceFromHome = GlobalPosition.DistanceTo(_home);

        if ((_state == EnemyState.Chase && distanceFromHome > ResetRadius) || player.Health.IsDead)
        {
            _state = EnemyState.Return;
        }
        else if (distanceToPlayer <= DetectRadius)
        {
            _state = EnemyState.Chase;
        }
        else if (_state == EnemyState.Idle)
        {
            _state = EnemyState.Patrol;
        }

        Vector2 desired = Vector2.Zero;
        switch (_state)
        {
            case EnemyState.Patrol:
                desired = Patrol(dt);
                break;
            case EnemyState.Chase:
                desired = GlobalPosition.DirectionTo(player.GlobalPosition) * MoveSpeed;
                break;
            case EnemyState.Return:
                desired = GlobalPosition.DirectionTo(_home) * MoveSpeed;
                if (GlobalPosition.DistanceTo(_home) < 8f)
                {
                    _state = EnemyState.Patrol;
                }
                break;
        }

        Velocity = desired;
        if (_knockbackVelocity.LengthSquared() > 1f)
        {
            Velocity += _knockbackVelocity;
            _knockbackVelocity = _knockbackVelocity.MoveToward(Vector2.Zero, 620f * dt);
        }

        MoveAndSlide();
        TryContactAttack(player);
    }

    public override void _Draw()
    {
        Color body = _flashTimer > 0
            ? new Color(1f, 0.83f, 0.62f)
            : new Color(0.48f, 0.28f, 0.58f);
        DrawCircle(Vector2.Zero, 13f, body);
        DrawCircle(new Vector2(-5, -4), 2.2f, new Color(0.95f, 0.93f, 0.64f));
        DrawCircle(new Vector2(5, -4), 2.2f, new Color(0.95f, 0.93f, 0.64f));
        DrawArc(Vector2.Zero, 16f, 0, Mathf.Tau, 18, new Color(0.2f, 0.12f, 0.24f), 2f);
    }

    private Vector2 Patrol(float dt)
    {
        _thinkTimer -= dt;
        if (_thinkTimer <= 0 || GlobalPosition.DistanceTo(_patrolTarget) < 8f)
        {
            _thinkTimer = 1.5f + GD.Randf() * 2f;
            Vector2 offset = Vector2.Right.Rotated(GD.Randf() * Mathf.Tau) * (24f + GD.Randf() * PatrolRadius);
            _patrolTarget = _home + offset;
        }

        return GlobalPosition.DirectionTo(_patrolTarget) * (MoveSpeed * 0.45f);
    }

    private void TryContactAttack(PlayerController player)
    {
        if (_contactCooldown > 0 || GlobalPosition.DistanceTo(player.GlobalPosition) > 24f || player.Health.IsDead)
        {
            return;
        }

        _contactCooldown = 0.8f;
        Vector2 knockback = GlobalPosition.DirectionTo(player.GlobalPosition) * 260f;
        player.Health.ApplyDamage(ContactDamage, this, knockback);
    }

    private void OnDamaged(int amount, Vector2 knockback)
    {
        _flashTimer = 0.16f;
        _knockbackVelocity = knockback;
    }

    private void OnDied()
    {
        _state = EnemyState.Dead;
        GameManager.Instance?.SpawnPickup(GlobalPosition, "bronze_coin", 3);
        QueueFree();
    }
}
