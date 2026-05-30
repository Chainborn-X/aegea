using Godot;
using System;
using System.IO;
using System.Text.Json;

namespace Aegea;

public partial class SaveGameService : Node
{
    private const string SavePath = "user://aegea_mvp_save.json";

    public void Save(PlayerController player, QuestTracker quests)
    {
        var data = new SaveGameData
        {
            PlayerX = player.GlobalPosition.X,
            PlayerY = player.GlobalPosition.Y,
            PlayerHealth = player.Health.CurrentHealth,
            Inventory = player.Inventory.Snapshot(),
            Objectives = quests.ToDictionary()
        };

        string path = ProjectSettings.GlobalizePath(SavePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        GameManager.Instance?.ShowNotification("Game saved");
    }

    public bool Load(PlayerController player, QuestTracker quests)
    {
        string path = ProjectSettings.GlobalizePath(SavePath);
        if (!File.Exists(path))
        {
            GameManager.Instance?.ShowNotification("No save found");
            return false;
        }

        try
        {
            SaveGameData? data = JsonSerializer.Deserialize<SaveGameData>(File.ReadAllText(path));
            if (data is null)
            {
                return false;
            }

            player.GlobalPosition = new Vector2(data.PlayerX, data.PlayerY);
            player.Health.SetHealth(data.PlayerHealth);
            player.Inventory.Restore(data.Inventory);
            quests.Restore(data.Objectives);
            GameManager.Instance?.ShowNotification("Game loaded");
            return true;
        }
        catch (Exception exception)
        {
            GD.PushWarning($"Failed to load save: {exception.Message}");
            GameManager.Instance?.ShowNotification("Save could not be loaded");
            return false;
        }
    }
}
