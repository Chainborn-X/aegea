using Godot;
using System;

namespace Aegea;

public partial class GameManager : Node2D
{
    public static GameManager? Instance { get; private set; }

    public PlayerController Player { get; private set; } = null!;
    public DialogueController Dialogue { get; private set; } = null!;
    public QuestTracker Quests { get; private set; } = null!;

    private GameUi _ui = null!;
    private SaveGameService _save = null!;

    public override void _Ready()
    {
        Instance = this;
        GD.Randomize();
        RegisterInputActions();

        Dialogue = new DialogueController { Name = "Dialogue" };
        Quests = new QuestTracker { Name = "QuestTracker" };
        _save = new SaveGameService { Name = "SaveGameService" };
        AddChild(Dialogue);
        AddChild(Quests);
        AddChild(_save);

        BuildWorld();

        _ui = new GameUi { Name = "GameUi" };
        AddChild(_ui);
        _ui.Bind(Player, Quests, Dialogue);
        ShowNotification("Find the Old Shrine Key");
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("inventory") && !GetTree().Paused)
        {
            _ui.ToggleInventory(Player);
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("save_game") && !GetTree().Paused)
        {
            _save.Save(Player, Quests);
            GetViewport().SetInputAsHandled();
        }

        if (@event.IsActionPressed("load_game") && !GetTree().Paused)
        {
            _save.Load(Player, Quests);
            GetViewport().SetInputAsHandled();
        }
    }

    public void TogglePause()
    {
        bool paused = !GetTree().Paused;
        GetTree().Paused = paused;
        _ui.SetPaused(paused);
    }

    public void ShowNotification(string text)
    {
        _ui?.ShowNotification(text);
    }

    public Pickup SpawnPickup(Vector2 globalPosition, string itemId, int amount)
    {
        var pickup = new Pickup { ItemId = itemId, Amount = amount, GlobalPosition = globalPosition };
        GetTree().CurrentScene.AddChild(pickup);
        return pickup;
    }

    private void BuildWorld()
    {
        GetOrCreateChild("Ambience", () => new CanvasModulate
        {
            Name = "Ambience",
            Color = new Color(0.94f, 0.98f, 0.9f)
        });

        GetOrCreateChild("OutdoorArea", () => new OutdoorArea { Name = "OutdoorArea" });

        Player = GetOrCreateChild("Player", () => new PlayerController { Name = "Player", GlobalPosition = new Vector2(560, 585) });
        EnsurePlayerCamera();

        GetOrCreateChild("OracleStone", CreateOracleStone);
        GetOrCreateChild("MyrineNpc", CreateNpc);
        GetOrCreateChild("WeatheredSign", CreateSign);
        GetOrCreateChild("SaltWornGate", () => new ShrineGateInteractable { Name = "SaltWornGate", GlobalPosition = new Vector2(1198, 292) });
        GetOrCreateChild("HiddenCedarChest", () => new ChestInteractable { Name = "HiddenCedarChest", GlobalPosition = new Vector2(1035, 438), ItemId = "old_shrine_key" });
        GetOrCreateChild("CorruptedGuardian", () => new EnemyController { Name = "CorruptedGuardian", GlobalPosition = new Vector2(970, 560) });
        GetOrCreateChild("ShoreShade", () => new EnemyController { Name = "ShoreShade", GlobalPosition = new Vector2(640, 735), DetectRadius = 150f, MoveSpeed = 70f });
        GetOrCreateChild("StartingSunleaf", () => new Pickup { Name = "StartingSunleaf", GlobalPosition = new Vector2(790, 610), ItemId = "sunleaf_herb" });
        EnsureFallbackPlants();
    }

    private T GetOrCreateChild<T>(string nodeName, Func<T> create) where T : Node
    {
        T? existing = GetNodeOrNull<T>(nodeName);
        if (existing is not null)
        {
            return existing;
        }

        T node = create();
        if (string.IsNullOrWhiteSpace(node.Name))
        {
            node.Name = nodeName;
        }

        AddChild(node);
        return node;
    }

    private void EnsurePlayerCamera()
    {
        Camera2D? camera = Player.GetNodeOrNull<Camera2D>("Camera2D");
        if (camera is null)
        {
            camera = new Camera2D { Name = "Camera2D" };
            Player.AddChild(camera);
        }

        camera.PositionSmoothingEnabled = true;
        camera.PositionSmoothingSpeed = 7f;
        camera.Zoom = new Vector2(2.2f, 2.2f);
        camera.MakeCurrent();
    }

    private DialogueInteractable CreateOracleStone()
    {
        return new DialogueInteractable
        {
            Name = "OracleStone",
            Speaker = "Tide Oracle",
            Prompt = "Listen",
            Lines =
            [
                "The road above the shrine is sealed by an old bronze key.",
                "Overgrowth hides small things from impatient hands.",
                "Cut the laurel bushes east of the stones. Listen for the chest."
            ],
            GlobalPosition = new Vector2(930, 382)
        };
    }

    private DialogueInteractable CreateSign()
    {
        return new DialogueInteractable
        {
            Name = "WeatheredSign",
            Speaker = "Weathered Sign",
            Prompt = "Read",
            Lines =
            [
                "North: Salt Shrine.",
                "South: shore road, broken by the last winter tide."
            ],
            GlobalPosition = new Vector2(520, 642)
        };
    }

    private DialogueInteractable CreateNpc()
    {
        return new DialogueInteractable
        {
            Name = "MyrineNpc",
            Speaker = "Myrine",
            Prompt = "Talk",
            Lines =
            [
                "The shrine used to answer at dawn.",
                "Now the bushes grow fast and the little guardians bite at shadows.",
                "If you find the old key, do not force the northern gate. Let it wake."
            ],
            GlobalPosition = new Vector2(720, 520)
        };
    }

    private void EnsureFallbackPlants()
    {
        (string Name, Vector2 Position, string Kind, string DropItem, float DropChance)[] plants =
        [
            ("CuttableGrass01", new Vector2(652, 560), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass02", new Vector2(690, 584), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass03", new Vector2(734, 548), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass04", new Vector2(815, 455), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass05", new Vector2(846, 432), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass06", new Vector2(878, 460), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass07", new Vector2(1050, 510), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass08", new Vector2(1095, 535), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass09", new Vector2(1125, 488), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass10", new Vector2(640, 710), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass11", new Vector2(685, 735), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass12", new Vector2(732, 700), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass13", new Vector2(890, 650), "grass", "sunleaf_herb", 0.3f),
            ("CuttableGrass14", new Vector2(932, 675), "grass", "bronze_coin", 0.3f),
            ("CuttableGrass15", new Vector2(970, 632), "grass", "sunleaf_herb", 0.3f),
            ("CuttableBush01", new Vector2(1015, 455), "bush", "laurel_sprig", 0.55f),
            ("CuttableBush02", new Vector2(1050, 478), "bush", "laurel_sprig", 0.55f),
            ("CuttableBush03", new Vector2(1120, 428), "bush", "laurel_sprig", 0.55f),
            ("CuttableBush04", new Vector2(850, 350), "bush", "laurel_sprig", 0.55f),
            ("CuttableBush05", new Vector2(890, 380), "bush", "laurel_sprig", 0.55f)
        ];

        foreach ((string name, Vector2 position, string kind, string dropItem, float dropChance) in plants)
        {
            GetOrCreateChild(name, () => new DestructiblePlant
            {
                Name = name,
                PlantKind = kind,
                DropItemId = dropItem,
                DropChance = dropChance,
                GlobalPosition = position
            });
        }
    }

    private static void RegisterInputActions()
    {
        AddKeyAction("move_up", Key.W, Key.Up);
        AddKeyAction("move_down", Key.S, Key.Down);
        AddKeyAction("move_left", Key.A, Key.Left);
        AddKeyAction("move_right", Key.D, Key.Right);
        AddKeyAction("run", Key.Shift);
        AddKeyAction("attack", Key.Space);
        AddKeyAction("interact", Key.E);
        AddKeyAction("inventory", Key.I);
        AddKeyAction("pause", Key.Escape);
        AddKeyAction("save_game", Key.F5);
        AddKeyAction("load_game", Key.F9);
    }

    private static void AddKeyAction(string action, params Key[] keys)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        foreach (Key key in keys)
        {
            var keyEvent = new InputEventKey { Keycode = key };
            if (!InputMap.ActionHasEvent(action, keyEvent))
            {
                InputMap.ActionAddEvent(action, keyEvent);
            }
        }
    }
}
