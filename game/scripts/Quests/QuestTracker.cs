using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Aegea;

public partial class QuestTracker : Node
{
    [Signal] public delegate void ObjectivesChangedEventHandler();

    private readonly Dictionary<string, bool> _objectives = new()
    {
        ["find_shrine_key"] = false
    };

    public string CurrentObjectiveText
    {
        get
        {
            if (!_objectives.GetValueOrDefault("find_shrine_key"))
            {
                return "Find the Old Shrine Key";
            }

            return "Test the key at the salt-worn gate";
        }
    }

    public IReadOnlyDictionary<string, bool> Snapshot() => _objectives;

    public void Complete(string objectiveId)
    {
        if (!_objectives.ContainsKey(objectiveId) || _objectives[objectiveId])
        {
            return;
        }

        _objectives[objectiveId] = true;
        GameManager.Instance?.ShowNotification("Objective updated");
        EmitSignal(SignalName.ObjectivesChanged);
    }

    public void Restore(Dictionary<string, bool> states)
    {
        foreach ((string key, bool value) in states)
        {
            if (_objectives.ContainsKey(key))
            {
                _objectives[key] = value;
            }
        }

        EmitSignal(SignalName.ObjectivesChanged);
    }

    public Dictionary<string, bool> ToDictionary()
    {
        return _objectives.ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
