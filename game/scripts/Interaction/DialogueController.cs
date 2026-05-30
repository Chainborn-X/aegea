using Godot;
using System.Collections.Generic;

namespace Aegea;

public partial class DialogueController : Node
{
    [Signal] public delegate void DialogueLineChangedEventHandler(string speaker, string line);
    [Signal] public delegate void DialogueClosedEventHandler();

    private readonly List<string> _lines = new();
    private string _speaker = "";
    private int _index = -1;

    public bool IsOpen => _index >= 0;

    public void StartDialogue(string speaker, IEnumerable<string> lines)
    {
        _speaker = speaker;
        _lines.Clear();
        _lines.AddRange(lines);
        _index = _lines.Count == 0 ? -1 : 0;

        if (_index < 0)
        {
            EmitSignal(SignalName.DialogueClosed);
            return;
        }

        EmitSignal(SignalName.DialogueLineChanged, _speaker, _lines[_index]);
    }

    public void AdvanceOrClose()
    {
        if (!IsOpen)
        {
            return;
        }

        _index++;
        if (_index >= _lines.Count)
        {
            _index = -1;
            EmitSignal(SignalName.DialogueClosed);
            return;
        }

        EmitSignal(SignalName.DialogueLineChanged, _speaker, _lines[_index]);
    }
}
