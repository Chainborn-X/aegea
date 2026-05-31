using Godot;
using System.Linq;

namespace Aegea;

public partial class PlayerController : CharacterBody2D
{
    private const string PlayerSpriteFramesPath = "res://assets/characters/player_traveler_4dir_frames.tres";
    private const string PlayerSpriteSheetPath = "res://assets/characters/player_traveler_4dir_sheet.png";
    private static readonly Vector2I PlayerSpriteFrameSize = new(192, 256);

    [Signal] public delegate void StaminaChangedEventHandler(float current, float max);
    [Signal] public delegate void InteractionPromptChangedEventHandler(string prompt);
    [Signal] public delegate void PlayerStateChangedEventHandler(string state);

    [Export] public float WalkSpeed { get; set; } = 135f;
    [Export] public float RunSpeed { get; set; } = 215f;
    [Export] public float Acceleration { get; set; } = 950f;
    [Export] public float Deceleration { get; set; } = 1150f;
    [Export] public float MaxStamina { get; set; } = 5f;
    [Export] public float AttackCooldownSeconds { get; set; } = 0.34f;
    [Export] public float AttackActiveSeconds { get; set; } = 0.14f;
    [Export] public bool UseSpriteSheet { get; set; } = true;

    public Damageable Health { get; private set; } = null!;
    public InventoryComponent Inventory { get; private set; } = null!;
    public Vector2 FacingDirection { get; private set; } = Vector2.Down;
    public Vector2 SpawnPosition { get; set; }

    private AttackHitbox _attackHitbox = null!;
    private Area2D _interactionSensor = null!;
    private AnimatedSprite2D? _sprite;
    private float _stamina;
    private float _attackCooldownTimer;
    private float _attackStateTimer;
    private float _hurtTimer;
    private float _deathTimer;
    private Vector2 _knockbackVelocity;
    private string _state = "idle";
    private string _lastPrompt = "";
    private float _promptRefreshTimer;

    public override void _Ready()
    {
        SpawnPosition = GlobalPosition;
        _stamina = MaxStamina;

        CollisionLayer = 1;
        CollisionMask = uint.MaxValue;
        AddChild(new CollisionShape2D { Shape = new CapsuleShape2D { Radius = 10, Height = 24 } });

        Health = new Damageable { Name = "Health", MaxHealth = 6, Team = "Player", InvulnerabilitySeconds = 0.75f };
        AddChild(Health);
        Health.Damaged += OnDamaged;
        Health.Died += OnDied;

        Inventory = new InventoryComponent { Name = "Inventory" };
        AddChild(Inventory);

        _attackHitbox = new AttackHitbox { Name = "SwordHitbox", Damage = 1, IgnoredTeam = "Player" };
        AddChild(_attackHitbox);

        _interactionSensor = new Area2D { Name = "InteractionSensor", Monitoring = true, Monitorable = false };
        _interactionSensor.AddChild(new CollisionShape2D { Shape = new CircleShape2D { Radius = 38 } });
        AddChild(_interactionSensor);

        SetupSprite();
        EmitSignal(SignalName.StaminaChanged, _stamina, MaxStamina);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("attack") || (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left && mouse.Pressed))
        {
            TryAttack();
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("interact"))
        {
            if (GameManager.Instance?.Dialogue.IsOpen == true)
            {
                GameManager.Instance.Dialogue.AdvanceOrClose();
            }
            else
            {
                TryInteract();
            }

            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _attackCooldownTimer = Mathf.Max(0, _attackCooldownTimer - dt);
        _attackStateTimer = Mathf.Max(0, _attackStateTimer - dt);
        _hurtTimer = Mathf.Max(0, _hurtTimer - dt);

        if (Health.IsDead)
        {
            _deathTimer -= dt;
            SetState("death");
            if (_deathTimer <= 0)
            {
                Respawn();
            }
        }

        RefreshPrompt(dt);
        UpdateSpriteAnimation();
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        if (Health.IsDead || GameManager.Instance?.Dialogue.IsOpen == true)
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, Deceleration * dt);
            MoveAndSlide();
            return;
        }

        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input.LengthSquared() > 0.01f)
        {
            FacingDirection = input.Normalized();
        }

        bool wantsRun = Input.IsActionPressed("run") && input.LengthSquared() > 0.01f && _stamina > 0.1f;
        float targetSpeed = wantsRun ? RunSpeed : WalkSpeed;
        Vector2 targetVelocity = input * targetSpeed;
        float rate = input.LengthSquared() > 0.01f ? Acceleration : Deceleration;

        Velocity = Velocity.MoveToward(targetVelocity, rate * dt);
        if (_knockbackVelocity.LengthSquared() > 1)
        {
            Velocity += _knockbackVelocity;
            _knockbackVelocity = _knockbackVelocity.MoveToward(Vector2.Zero, 720f * dt);
        }

        MoveAndSlide();

        if (wantsRun)
        {
            _stamina = Mathf.Max(0, _stamina - dt);
        }
        else
        {
            _stamina = Mathf.Min(MaxStamina, _stamina + dt * 0.8f);
        }

        EmitSignal(SignalName.StaminaChanged, _stamina, MaxStamina);
        UpdateMovementState(input, wantsRun);
    }

    public override void _Draw()
    {
        if (UseSpriteSheet && _sprite is not null)
        {
            return;
        }

        Color tunic = _state switch
        {
            "hurt" => new Color(1f, 0.55f, 0.48f),
            "death" => new Color(0.35f, 0.34f, 0.32f),
            "run" => new Color(0.15f, 0.57f, 0.72f),
            _ => new Color(0.18f, 0.47f, 0.58f)
        };

        DrawCircle(new Vector2(0, -8), 8, new Color(0.86f, 0.68f, 0.48f));
        DrawRect(new Rect2(-9, -5, 18, 22), tunic, true);
        DrawRect(new Rect2(-9, -5, 18, 22), new Color(0.07f, 0.12f, 0.14f), false, 2f);
        DrawLine(Vector2.Zero, FacingDirection * 15f, new Color(0.98f, 0.91f, 0.65f), 3f);

        if (_attackStateTimer > 0)
        {
            Vector2 bladeStart = FacingDirection * 8f;
            Vector2 bladeEnd = FacingDirection * 30f;
            DrawLine(bladeStart, bladeEnd, new Color(0.95f, 0.94f, 0.86f), 5f);
            DrawLine(bladeStart, bladeEnd, new Color(0.45f, 0.55f, 0.66f), 2f);
        }
    }

    public void TryAttack()
    {
        if (Health.IsDead || _attackCooldownTimer > 0 || GameManager.Instance?.Dialogue.IsOpen == true)
        {
            return;
        }

        _attackCooldownTimer = AttackCooldownSeconds;
        _attackStateTimer = AttackActiveSeconds + 0.1f;
        SetState("attack");
        _attackHitbox.Activate(this, FacingDirection, AttackActiveSeconds);
    }

    public void TryInteract()
    {
        IInteractable? interactable = FindBestInteractable();
        interactable?.Interact(this);
    }

    public void Respawn()
    {
        GlobalPosition = SpawnPosition;
        Velocity = Vector2.Zero;
        _knockbackVelocity = Vector2.Zero;
        Health.RestoreFull();
        SetState("idle");
        GameManager.Instance?.ShowNotification("You wake beside the old spring.");
    }

    private void OnDamaged(int amount, Vector2 knockback)
    {
        _hurtTimer = 0.22f;
        _knockbackVelocity = knockback;
        SetState("hurt");
    }

    private void OnDied()
    {
        _deathTimer = 1.4f;
        SetState("death");
    }

    private void UpdateMovementState(Vector2 input, bool running)
    {
        if (_hurtTimer > 0 || _attackStateTimer > 0)
        {
            return;
        }

        if (input.LengthSquared() < 0.01f)
        {
            SetState("idle");
        }
        else
        {
            SetState(running ? "run" : "walk");
        }
    }

    private void SetState(string state)
    {
        if (_state == state)
        {
            return;
        }

        _state = state;
        EmitSignal(SignalName.PlayerStateChanged, _state);
        UpdateSpriteAnimation();
    }

    private void SetupSprite()
    {
        if (!UseSpriteSheet)
        {
            return;
        }

        _sprite = GetNodeOrNull<AnimatedSprite2D>("PlayerSprite") ?? new AnimatedSprite2D { Name = "PlayerSprite" };
        if (_sprite.GetParent() is null)
        {
            AddChild(_sprite);
        }

        _sprite.SpriteFrames ??= LoadPlayerSpriteFrames();
        _sprite.Scale = new Vector2(0.24f, 0.24f);
        _sprite.Position = new Vector2(0, -14);
        _sprite.ZIndex = 10;
        _sprite.Play("idle_down");
    }

    private static SpriteFrames LoadPlayerSpriteFrames()
    {
        return ResourceLoader.Exists(PlayerSpriteFramesPath)
            ? GD.Load<SpriteFrames>(PlayerSpriteFramesPath)
            : BuildSpriteFrames(LoadPlayerSpriteSheet());
    }

    private static Texture2D LoadPlayerSpriteSheet()
    {
        return ResourceLoader.Exists(PlayerSpriteSheetPath)
            ? GD.Load<Texture2D>(PlayerSpriteSheetPath)
            : ImageTexture.CreateFromImage(Image.LoadFromFile(PlayerSpriteSheetPath));
    }

    private static SpriteFrames BuildSpriteFrames(Texture2D sheet)
    {
        var frames = new SpriteFrames();
        if (frames.HasAnimation("default"))
        {
            frames.RemoveAnimation("default");
        }

        AddDirectionalAnimation(frames, sheet, "idle", [0], 1.0, true);
        AddDirectionalAnimation(frames, sheet, "walk", [0, 1, 2, 3], 6.0, true);
        AddDirectionalAnimation(frames, sheet, "run", [0, 1, 2, 3], 9.0, true);
        AddDirectionalAnimation(frames, sheet, "attack", [4, 5], 10.0, false);
        AddDirectionalAnimation(frames, sheet, "hurt", [6], 1.0, false);
        AddDirectionalAnimation(frames, sheet, "death", [7], 1.0, false);
        return frames;
    }

    private static void AddDirectionalAnimation(SpriteFrames frames, Texture2D sheet, string animation, int[] columns, double speed, bool loop)
    {
        string[] directions = ["down", "right", "left", "up"];
        for (int row = 0; row < directions.Length; row++)
        {
            string animationName = $"{animation}_{directions[row]}";
            frames.AddAnimation(animationName);
            frames.SetAnimationSpeed(animationName, speed);
            frames.SetAnimationLoop(animationName, loop);
            foreach (int column in columns)
            {
                frames.AddFrame(animationName, CreateFrame(sheet, column, row), 1f, -1);
            }
        }
    }

    private static AtlasTexture CreateFrame(Texture2D sheet, int column, int row)
    {
        return new AtlasTexture
        {
            Atlas = sheet,
            Region = new Rect2(column * PlayerSpriteFrameSize.X, row * PlayerSpriteFrameSize.Y, PlayerSpriteFrameSize.X, PlayerSpriteFrameSize.Y)
        };
    }

    private void UpdateSpriteAnimation()
    {
        if (_sprite is null)
        {
            return;
        }

        string animationName = $"{_state}_{GetDirectionName()}";
        if (_sprite.SpriteFrames is null || !_sprite.SpriteFrames.HasAnimation(animationName))
        {
            return;
        }

        if (_sprite.Animation == animationName && _sprite.IsPlaying())
        {
            return;
        }

        _sprite.Play(animationName);
    }

    private string GetDirectionName()
    {
        if (Mathf.Abs(FacingDirection.X) > Mathf.Abs(FacingDirection.Y))
        {
            return FacingDirection.X < 0 ? "left" : "right";
        }

        return FacingDirection.Y < 0 ? "up" : "down";
    }

    private IInteractable? FindBestInteractable()
    {
        return _interactionSensor.GetOverlappingAreas()
            .OfType<IInteractable>()
            .OrderBy(interactable => interactable is Node2D node ? GlobalPosition.DistanceSquaredTo(node.GlobalPosition) : float.MaxValue)
            .FirstOrDefault();
    }

    private void RefreshPrompt(float dt)
    {
        _promptRefreshTimer -= dt;
        if (_promptRefreshTimer > 0)
        {
            return;
        }

        _promptRefreshTimer = 0.08f;
        string prompt = FindBestInteractable()?.GetPrompt() ?? "";
        if (prompt == _lastPrompt)
        {
            return;
        }

        _lastPrompt = prompt;
        EmitSignal(SignalName.InteractionPromptChanged, prompt);
    }
}
