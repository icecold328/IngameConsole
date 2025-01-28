using FlaxEditor;
using FlaxEditor.Content;

namespace IngameConsole;

/// <summary>
/// </summary>
public class IngameConsoleEditor : EditorPlugin
{
    private CustomSettingsProxy _sproxy;

    /// <inheritdoc />
    public override void InitializeEditor()
    {
        base.InitializeEditor();
		
        _sproxy = new CustomSettingsProxy(typeof(ConsoleSettings), "ConsoleSettings");
        Editor.ContentDatabase.AddProxy(_sproxy);        
    }

    /// <inheritdoc />
    public override void DeinitializeEditor()
    {
        Editor.ContentDatabase.RemoveProxy(_sproxy);
		
        base.DeinitializeEditor();
    }
}