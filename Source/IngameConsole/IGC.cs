using System;
using FlaxEngine;

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
            RepositoryUrl = "https://github.com/icecold328/IngameConsole",
            Description = "Ingame Console.",
            Version = new Version(1, 0, 1),
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
#if FLAX_EDITOR
            Debug.LogError("Missing Console settings, Check GameSettings File");
#endif
            return;
        }
        _cSettings = SettingsJson.CreateInstance<ConsoleSettings>();
        
        Scripting.Update += Scripting_Update;        

        Con = new Console(_cSettings);
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

#if FLAX_EDITOR
        Debug.Log("Ingame Console Deinitalized");
#endif

        base.Deinitialize();
    }
}

