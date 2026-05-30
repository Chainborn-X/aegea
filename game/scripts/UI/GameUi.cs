using Godot;
using System.Text;

namespace Aegea;

public partial class GameUi : CanvasLayer
{
    private Label _healthLabel = null!;
    private ProgressBar _healthBar = null!;
    private ProgressBar _staminaBar = null!;
    private Label _inventoryLabel = null!;
    private Label _objectiveLabel = null!;
    private Label _notificationLabel = null!;
    private Label _promptLabel = null!;
    private PanelContainer _dialoguePanel = null!;
    private Label _dialogueSpeaker = null!;
    private Label _dialogueLine = null!;
    private Control _pauseMenu = null!;
    private Control _inventoryPanel = null!;
    private float _notificationTimer;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        BuildHud();
        BuildDialogue();
        BuildPauseMenu();
        BuildInventoryPanel();
    }

    public override void _Process(double delta)
    {
        if (_notificationTimer <= 0)
        {
            return;
        }

        _notificationTimer -= (float)delta;
        if (_notificationTimer <= 0)
        {
            _notificationLabel.Text = "";
        }
    }

    public void Bind(PlayerController player, QuestTracker quests, DialogueController dialogue)
    {
        player.Health.HealthChanged += UpdateHealth;
        player.StaminaChanged += UpdateStamina;
        player.Inventory.InventoryChanged += () => UpdateInventory(player.Inventory);
        player.Inventory.ItemAdded += OnItemAdded;
        player.InteractionPromptChanged += UpdatePrompt;
        quests.ObjectivesChanged += () => _objectiveLabel.Text = quests.CurrentObjectiveText;
        dialogue.DialogueLineChanged += ShowDialogue;
        dialogue.DialogueClosed += HideDialogue;

        UpdateHealth(player.Health.CurrentHealth, player.Health.MaxHealth);
        UpdateStamina(player.MaxStamina, player.MaxStamina);
        UpdateInventory(player.Inventory);
        _objectiveLabel.Text = quests.CurrentObjectiveText;
    }

    public void ShowNotification(string text)
    {
        _notificationLabel.Text = text;
        _notificationTimer = 2.2f;
    }

    public void SetPaused(bool paused)
    {
        _pauseMenu.Visible = paused;
    }

    public void ToggleInventory(PlayerController player)
    {
        _inventoryPanel.Visible = !_inventoryPanel.Visible;
        UpdateInventory(player.Inventory);
    }

    private void BuildHud()
    {
        var margin = new MarginContainer();
        margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        margin.AddThemeConstantOverride("margin_left", 18);
        margin.AddThemeConstantOverride("margin_top", 14);
        margin.AddThemeConstantOverride("margin_right", 18);
        margin.AddThemeConstantOverride("margin_bottom", 14);
        AddChild(margin);

        var root = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin };
        margin.AddChild(root);

        _healthLabel = new Label { Text = "Health" };
        root.AddChild(_healthLabel);

        _healthBar = new ProgressBar { MaxValue = 6, Value = 6, CustomMinimumSize = new Vector2(220, 18), ShowPercentage = false };
        root.AddChild(_healthBar);

        root.AddChild(new Label { Text = "Stamina" });
        _staminaBar = new ProgressBar { MaxValue = 5, Value = 5, CustomMinimumSize = new Vector2(220, 14), ShowPercentage = false };
        root.AddChild(_staminaBar);

        _objectiveLabel = new Label { Text = "Find the Old Shrine Key", CustomMinimumSize = new Vector2(320, 24) };
        _objectiveLabel.AddThemeColorOverride("font_color", new Color(0.96f, 0.9f, 0.68f));
        root.AddChild(_objectiveLabel);

        _inventoryLabel = new Label { Text = "" };
        root.AddChild(_inventoryLabel);

        _notificationLabel = new Label { Text = "" };
        _notificationLabel.AddThemeColorOverride("font_color", new Color(0.9f, 0.98f, 0.78f));
        root.AddChild(_notificationLabel);

        _promptLabel = new Label { Text = "" };
        _promptLabel.SetAnchorsPreset(Control.LayoutPreset.CenterBottom);
        _promptLabel.Position = new Vector2(-120, -76);
        _promptLabel.CustomMinimumSize = new Vector2(240, 32);
        _promptLabel.HorizontalAlignment = HorizontalAlignment.Center;
        AddChild(_promptLabel);
    }

    private void BuildDialogue()
    {
        _dialoguePanel = new PanelContainer { Visible = false };
        _dialoguePanel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        _dialoguePanel.OffsetLeft = 220;
        _dialoguePanel.OffsetRight = -220;
        _dialoguePanel.OffsetTop = -150;
        _dialoguePanel.OffsetBottom = -24;
        AddChild(_dialoguePanel);

        var box = new VBoxContainer();
        _dialoguePanel.AddChild(box);
        _dialogueSpeaker = new Label();
        _dialogueSpeaker.AddThemeColorOverride("font_color", new Color(0.96f, 0.86f, 0.48f));
        _dialogueLine = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        box.AddChild(_dialogueSpeaker);
        box.AddChild(_dialogueLine);
        box.AddChild(new Label { Text = "E to continue" });
    }

    private void BuildPauseMenu()
    {
        _pauseMenu = new PanelContainer { Visible = false };
        _pauseMenu.SetAnchorsPreset(Control.LayoutPreset.Center);
        _pauseMenu.CustomMinimumSize = new Vector2(280, 220);
        _pauseMenu.Position = new Vector2(-140, -110);
        AddChild(_pauseMenu);

        var box = new VBoxContainer();
        _pauseMenu.AddChild(box);
        box.AddChild(new Label { Text = "Aegea Paused", HorizontalAlignment = HorizontalAlignment.Center });
        Button resume = new() { Text = "Resume" };
        resume.Pressed += () => GameManager.Instance?.TogglePause();
        box.AddChild(resume);
        box.AddChild(new Label { Text = "Settings" });
        box.AddChild(new CheckBox { Text = "Camera smoothing", ButtonPressed = true });
        box.AddChild(new HSlider { MinValue = 0, MaxValue = 10, Value = 7 });
        box.AddChild(new Label { Text = "Settings are a stub for the MVP." });
    }

    private void BuildInventoryPanel()
    {
        _inventoryPanel = new PanelContainer { Visible = false };
        _inventoryPanel.SetAnchorsPreset(Control.LayoutPreset.CenterRight);
        _inventoryPanel.OffsetLeft = -300;
        _inventoryPanel.OffsetRight = -24;
        _inventoryPanel.OffsetTop = -180;
        _inventoryPanel.OffsetBottom = 180;
        AddChild(_inventoryPanel);

        var box = new VBoxContainer();
        _inventoryPanel.AddChild(box);
        box.AddChild(new Label { Text = "Inventory" });
        box.AddChild(new Label { Name = "InventoryDetails", Text = "" });
        Button useHerb = new() { Text = "Use Sunleaf Herb" };
        useHerb.Pressed += () =>
        {
            PlayerController? player = GameManager.Instance?.Player;
            if (player is not null && !player.Inventory.UseHealingItem(player.Health))
            {
                GameManager.Instance?.ShowNotification("No Sunleaf Herb");
            }
        };
        box.AddChild(useHerb);
    }

    private void UpdateHealth(int current, int max)
    {
        _healthLabel.Text = $"Health {current}/{max}";
        _healthBar.MaxValue = max;
        _healthBar.Value = current;
    }

    private void UpdateStamina(float current, float max)
    {
        _staminaBar.MaxValue = max;
        _staminaBar.Value = current;
    }

    private void UpdateInventory(InventoryComponent inventory)
    {
        var summary = new StringBuilder();
        foreach ((string itemId, int amount) in inventory.Items)
        {
            if (itemId == "reed_sword")
            {
                continue;
            }

            if (summary.Length > 0)
            {
                summary.Append("  ");
            }

            summary.Append(ItemCatalog.Get(itemId).DisplayName).Append(" x").Append(amount);
        }

        _inventoryLabel.Text = summary.Length == 0 ? "Inventory: empty" : $"Inventory: {summary}";

        Label? details = _inventoryPanel.FindChild("InventoryDetails", true, false) as Label;
        if (details is not null)
        {
            details.Text = summary.Length == 0 ? "No loose items yet." : summary.ToString();
        }
    }

    private void OnItemAdded(string itemId, int amount)
    {
        ShowNotification($"+{amount} {ItemCatalog.Get(itemId).DisplayName}");
    }

    private void UpdatePrompt(string prompt)
    {
        _promptLabel.Text = string.IsNullOrWhiteSpace(prompt) ? "" : $"E: {prompt}";
    }

    private void ShowDialogue(string speaker, string line)
    {
        _dialoguePanel.Visible = true;
        _dialogueSpeaker.Text = speaker;
        _dialogueLine.Text = line;
    }

    private void HideDialogue()
    {
        _dialoguePanel.Visible = false;
    }
}
