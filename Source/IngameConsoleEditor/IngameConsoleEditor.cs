using FlaxEditor;
using FlaxEditor.Content;
using FlaxEngine;
using System;

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

        _description = new PluginDescription
        {
            Name = "Ingame Console Editor",
            Category = "Other",
            Author = "IceCold",
            AuthorUrl = null,
            HomepageUrl = null,
            RepositoryUrl = "https://github.com/icecold328/IngameConsole",
            Description = "Ingame Console.",
            Version = new Version(1, 0, 1),
            IsAlpha = false,
            IsBeta = false,
        };

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