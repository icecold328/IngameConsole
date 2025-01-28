using Flax.Build;

public class IngameConsoleEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("IngameConsole");
        Modules.Add("IngameConsoleEditor");
    }
}
