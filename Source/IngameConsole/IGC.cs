using FlaxEditor;
using FlaxEditor.Content;
using FlaxEngine;
using System;
using System.Runtime;

namespace IngameConsole;

/// <summary>
/// The sample game plugin.
/// </summary>
public class IGC : GamePlugin
{
    private ConsoleSettings _cSettings { get; set; } = null;

    private Console Con = null;

    /// <inheritdoc />
    public IGC()
    {
        _description = new PluginDescription
        {
            Name = "Ingame Console",
            Category = "Other",
            Author = "IceCold",
            AuthorUrl = null,
            HomepageUrl = null,
            RepositoryUrl = "",
            Description = "Ingame Console.",
            Version = new Version(1, 0, 0),
            IsAlpha = false,
            IsBeta = false,
        };
    }    

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        JsonAsset SettingsJson = Engine.GetCustomSettings("ConsoleSettings");

        if (SettingsJson == null)
        {
            Debug.LogError("Missing Console settings, Check GameSettings File");
            return;
        }
        _cSettings = SettingsJson.CreateInstance<ConsoleSettings>();
        Debug.Log("[IGC] Loaded Console Settings");

        Scripting.Update += Scripting_Update;        

        Con = new Console(_cSettings);

        Debug.Log("[IGC] Ingame Console Initalized");
    }    

    private void Scripting_Update()
    {
        Con?.Update();
    }

    /// <inheritdoc />
    public override void Deinitialize()
    {
        // Use it to cleanup Data        
        _cSettings = null;

        Scripting.Update -= Scripting_Update;

        Con?.Dispose();

        Debug.Log("[IGC] Ingame Console Deinitalized");
        base.Deinitialize();
    }
}

