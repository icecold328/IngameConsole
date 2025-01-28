using FlaxEngine;

namespace IngameConsole;

/// <summary>
/// </summary>
public class ConsoleSettings
{
    /// <summary>
    /// InputKey to Open/Close Console.
    /// </summary>
    [Space(2)][Tooltip("Button for Opening/Closing Console")]public KeyboardKeys ConsoleKey = KeyboardKeys.BackQuote;

    /// <summary>
    /// Display TimeStamps in Console Output.
    /// </summary>    
    [Space(2)][Tooltip("Display TimeStamp in Console")]public bool DisplayTimeData = true;

    /// <summary>
    /// Maximum Messages to Display.
    /// </summary>
    [Space(2)][Tooltip("Maximum Number of Message in Console")] public int MaxMessages = 50;
   
    /// <summary>
    /// Maximum Messages to Display.
    /// </summary>
    [Space(2)][Tooltip("Maximum Ammount of User Input Text into Console")] public int MaxUserInput = 1000;
    /// <summary>
    /// Console Background Color.
    /// </summary>
    [Space(2)] public Color ConsoleBackgroundColor = new Color(.4f, .4f, .4f, .6f);

    /// <summary>
    /// Console Input Background Color.
    /// </summary>
    public Color ConsoleInputBackgroundColor = new Color(.5f, .5f, .5f, .6f);
}
