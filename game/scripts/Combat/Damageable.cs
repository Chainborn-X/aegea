using Godot;

namespace Aegea;

public partial class Damageable : Node
{
    [Signal] public delegate void HealthChangedEventHandler(int current, int max);
    [Signal] public delegate void DamagedEventHandler(int amount, Vector2 knockback);
    [Signal] public delegate void DiedEventHandler();

    [Export] public int MaxHealth { get; set; } = 3;
    [Export] public float InvulnerabilitySeconds { get; set; } = 0.35f;
    [Export] public string Team { get; set; } = "Neutral";

    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    private float _invulnerabilityTimer;

    public override void _Ready()
    {
        CurrentHealth = Mathf.Max(1, MaxHealth);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    public override void _Process(double delta)
    {
        if (_invulnerabilityTimer > 0)
        {
            _invulnerabilityTimer = Mathf.Max(0, _invulnerabilityTimer - (float)delta);
        }
    }

    public bool ApplyDamage(int amount, Node? source, Vector2 knockback)
    {
        if (IsDead || amount <= 0 || _invulnerabilityTimer > 0)
        {
            return false;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        _invulnerabilityTimer = InvulnerabilitySeconds;
        EmitSignal(SignalName.Damaged, amount, knockback);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

        if (CurrentHealth == 0)
        {
            EmitSignal(SignalName.Died);
        }

        return true;
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    public void RestoreFull()
    {
        CurrentHealth = MaxHealth;
        _invulnerabilityTimer = 0;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    public void SetHealth(int health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
        _invulnerabilityTimer = 0;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }
}
