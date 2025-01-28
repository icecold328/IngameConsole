using FlaxEditor.CustomEditors.Editors;
using FlaxEditor.CustomEditors;

using FlaxEngine;
using IngameConsole;

#if FLAX_EDITOR

/// <summary>
/// Information Display in Settings File.
/// </summary>
[CustomEditor(typeof(ConsoleSettings))]
public class InformationEditor : GenericEditor
{
    /// <inheritdoc/>   
    public override void Initialize(LayoutElementsContainer layout)
    {
        layout.Space(10);
        layout.Label("  [Information]");
        layout.Label(" Add too the Game.Build.cs file for Script calls. ");
        var textbox = layout.TextBox(false);
        textbox.TextBox.IsReadOnly = true;
        textbox.Text = "options.PrivateDependencies.Add(\"IngameConsole\");";
        layout.Label(" once added to Game.Build.cs click File>Generate Script Project Files");
        layout.Label(" then open editor of choose and enjoy.");
        layout.Space(10);
        layout.Label("  [Settings]");

        base.Initialize(layout);
    }    
}

#endif