using Godot;

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
        var ambience = new CanvasModulate { Color = new Color(0.94f, 0.98f, 0.9f) };
        AddChild(ambience);

        var world = new OutdoorArea { Name = "OutdoorArea" };
        AddChild(world);

        Player = new PlayerController { Name = "Player", GlobalPosition = new Vector2(560, 585) };
        AddChild(Player);

        var camera = new Camera2D
        {
            Name = "Camera2D",
            PositionSmoothingEnabled = true,
            PositionSmoothingSpeed = 7f,
            Zoom = new Vector2(2.2f, 2.2f)
        };
        Player.AddChild(camera);
        camera.MakeCurrent();

        AddOracleStone(new Vector2(930, 382));
        AddNpc(new Vector2(720, 520));
        AddSign(new Vector2(520, 642));
        AddShrineGate(new Vector2(1198, 292));
        AddChest(new Vector2(1035, 438));
        AddEnemies();
        AddPlants();
        SpawnPickup(new Vector2(790, 610), "sunleaf_herb", 1);
    }

    private void AddOracleStone(Vector2 position)
    {
        var oracle = new DialogueInteractable
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
            GlobalPosition = position
        };
        AddChild(oracle);
    }

    private void AddSign(Vector2 position)
    {
        var sign = new DialogueInteractable
        {
            Name = "WeatheredSign",
            Speaker = "Weathered Sign",
            Prompt = "Read",
            Lines =
            [
                "North: Salt Shrine.",
                "South: shore road, broken by the last winter tide."
            ],
            GlobalPosition = position
        };
        AddChild(sign);
    }

    private void AddNpc(Vector2 position)
    {
        var npc = new DialogueInteractable
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
            GlobalPosition = position
        };
        AddChild(npc);
    }

    private void AddShrineGate(Vector2 position)
    {
        var gate = new ShrineGateInteractable
        {
            Name = "SaltWornGate",
            GlobalPosition = position
        };
        AddChild(gate);
    }

    private void AddChest(Vector2 position)
    {
        var chest = new ChestInteractable
        {
            Name = "HiddenCedarChest",
            GlobalPosition = position,
            ItemId = "old_shrine_key"
        };
        AddChild(chest);
    }

    private void AddEnemies()
    {
        AddChild(new EnemyController { Name = "CorruptedGuardian", GlobalPosition = new Vector2(970, 560) });
        AddChild(new EnemyController { Name = "ShoreShade", GlobalPosition = new Vector2(640, 735), DetectRadius = 150f, MoveSpeed = 70f });
    }

    private void AddPlants()
    {
        Vector2[] grassPositions =
        [
            new Vector2(652, 560), new Vector2(690, 584), new Vector2(734, 548),
            new Vector2(815, 455), new Vector2(846, 432), new Vector2(878, 460),
            new Vector2(1050, 510), new Vector2(1095, 535), new Vector2(1125, 488),
            new Vector2(640, 710), new Vector2(685, 735), new Vector2(732, 700),
            new Vector2(890, 650), new Vector2(932, 675), new Vector2(970, 632)
        ];

        foreach (Vector2 position in grassPositions)
        {
            AddChild(new DestructiblePlant
            {
                Name = "CuttableGrass",
                PlantKind = "grass",
                DropItemId = GD.Randf() > 0.6f ? "sunleaf_herb" : "bronze_coin",
                DropChance = 0.3f,
                GlobalPosition = position
            });
        }

        Vector2[] bushes =
        [
            new Vector2(1015, 455),
            new Vector2(1050, 478),
            new Vector2(1120, 428),
            new Vector2(850, 350),
            new Vector2(890, 380)
        ];

        foreach (Vector2 position in bushes)
        {
            AddChild(new DestructiblePlant
            {
                Name = "CuttableBush",
                PlantKind = "bush",
                DropItemId = "laurel_sprig",
                DropChance = 0.55f,
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
