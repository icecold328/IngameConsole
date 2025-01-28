using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.GUI;


namespace IngameConsole;

/// <summary>
/// Represents an in-game console for logging messages and executing commands.
/// </summary>
public class Console
{
    /// <summary>
    /// Singleton instance of console.
    /// </summary>
    public static Console i;

    /// <summary>
    /// Writes a message to the in-game console and the editor console.
    /// </summary>
    /// <param name="s">The message to be written.</param>
    public static void WriteLine(string s)
    {
        if (i is not null) { i.DisplayMessage(s, Color.White); }
#if FLAX_EDITOR
        Debug.Log(s);
#endif
    }

    /// <summary>
    /// Writes a warning message to the in-game console and the editor console.
    /// </summary>
    /// <param name="s">The warning message to be written.</param>
    public static void WriteWarning(string s)
    {
        if (i is not null) { i.DisplayMessage(s, Color.Yellow); }
#if FLAX_EDITOR
        Debug.LogWarning(s);
#endif
    }

    /// <summary>
    /// Writes an error message to the in-game console and the editor console.
    /// </summary>
    /// <param name="s">The error message to be written.</param>
    public static void WriteError(string s)
    {
        if (i is not null) { i.DisplayMessage(s, Color.Red); }
#if FLAX_EDITOR
        Debug.LogError(s);
#endif
    }


    /// <summary>
    /// Delegate for user messages to pass out.
    /// </summary>    
    public delegate void UserInputEvent(string message);
    /// <summary>
    /// event to subscribe too for text input events to console.
    /// </summary>
    public static event UserInputEvent TextInputEvent;

    /// <summary>
    /// Commands to display when help is entered.
    /// </summary>
    public static string[] Commands;

    private ConsoleSettings _settings; // Console configuration settings

    // UI Components
    private AlphaPanel _bdr; // Panel for the console background
    private VerticalPanel _topVP; // Panel for holding the input textbox
    private TextBox _tb; // Input textbox for console commands
    private Panel _pan; // Panel for displaying messages
    private VerticalPanel _vpInfo; // Panel for holding the message labels
    private List<Label> labels = new List<Label>(); // List of message labels

    private bool DisplayConsole = false; // Flag to track console visibility

    private CancellationTokenSource _cts;
    private Task WaitingTask;

    /// <summary>
    /// Initializes a new instance of the Console class with the specified settings.
    /// </summary>
    /// <param name="settings">The settings for the console.</param>
    public Console(ConsoleSettings settings)
    {
        // Set the singleton instance
        if (i is null)
            i = this;

        _settings = settings;

        if (RootControl.GameRoot is null)
        {
            _cts = new CancellationTokenSource();
            WaitingTask = Task.Run(async () =>
            {
                while (RootControl.GameRoot is null)
                {
                    if (_cts.IsCancellationRequested)
                    {                        
                        return;
                    }

                    await Task.Yield();
                }

                SetupConsole();
            }, _cts.Token);

            return;
        }

        SetupConsole();
    }

    void SetupConsole()
    {
        // Create and configure the console background panel
        _bdr = new AlphaPanel()
        {
            Parent = RootControl.GameRoot,
            Bounds = new Rectangle(new Float2(0, 0), new Float2(Screen.Size.X, Screen.Size.Y / 2)),
            BackgroundColor = _settings.ConsoleBackgroundColor,
        };

        _bdr.Enabled = DisplayConsole; // Set initial visibility
        _bdr.Visible = DisplayConsole;

        // Create the top vertical panel for the input textbox
        _topVP = new VerticalPanel()
        {
            Parent = _bdr,
        };

        // Create the panel for displaying messages
        _pan = new Panel()
        {
            Parent = _topVP,
            ScrollBars = ScrollBars.Vertical,
            AlwaysShowScrollbars = true,
        };

        // Create and configure the input textbox
        _tb = new TextBox()
        {
            Parent = _topVP,
            BackgroundColor = _settings.ConsoleInputBackgroundColor
        };

        _tb.TextBoxEditEnd += OnTextBoxEditEnd; // Subscribe to the edit end event

        // Create the vertical panel for holding message labels
        _vpInfo = new VerticalPanel()
        {
            Parent = _pan,
        };

        // Set bounds for various UI elements based on settings
        _bdr.Bounds = new Rectangle(new Float2(5, 0), new Float2(Screen.Size.X - 10, Screen.Size.Y / 2));
        _topVP.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, 0));
        _tb.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, 20));
        _pan.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, _bdr.Bounds.Size.Y - _tb.Bounds.Size.Y));
        _vpInfo.Bounds = new Rectangle(Float2.Zero, new Float2(_pan.Bounds.Size.X - _pan.ScrollBarsSize, _vpInfo.Bounds.Size.Y));

        // Configure scrolling properties
        _pan.AutoFocus = true;
        _pan.VScrollBar.AutoFocus = true;
        _pan.IsScrollable = true;
        _pan.VScrollBar.IsScrollable = true;

#if FLAX_EDITOR
        Debug.Log("Ingame Console Initalized");
#endif

    }

    /// <summary>
    /// Displays all available help commands.
    /// </summary>
    void DisplayHelp()
    {
        string availableCommands = " Available Commands : Help, Clear";
        if (Commands is not null)
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                availableCommands += $", {Commands[i]}";
            }
        }
        DisplayMessage(availableCommands, Color.White);
    }

    /// <summary>
    /// Checks and processes commands entered in the console.
    /// </summary>
    /// <param name="input">The command input by the user.</param>
    private void CheckCommands(string input)
    {
        if (input.Length > _settings.MaxUserInput)
        {
            DisplayMessage("Overflow Detected..", Color.Red);
            return;
        }
        // Clean and split input into sections
        string cleaned = input.ToLower();
        string[] Sections = cleaned.Split(' ');
        // Handle commands
        switch (Sections[0])
        {
            case "clear":
                ClearMessages(); // Clear the console messages
                break;
            case "help":
                DisplayHelp(); // Display help information
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Displays a message in the console with a specified text color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="TextColor">The color of the text.</param>
    void DisplayMessage(string message, Color TextColor)
    {
        // Split the message into lines using the newline character
        string[] lines = message.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        bool _displayTime = false; // Flag to determine if time should be displayed

        // Iterate through each line and create a label for it
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            if (i == 0)
                _displayTime = _settings.DisplayTimeData; // Set flag based on settings

            // Create a new label for each line and configure its properties
            labels.Add(new Label()
            {
                Parent = _vpInfo,
                Text = _displayTime ? $"[{DateTime.Now.ToString("HH:mm:ss")}] {lines[i]}" : lines[i],
                TextColor = TextColor,
                TextColorHighlighted = TextColor,
                AnchorPreset = AnchorPresets.HorizontalStretchBottom,
                HorizontalAlignment = TextAlignment.Near,
                VerticalAlignment = TextAlignment.Center,
                Margin = new Margin(4, 0, 0, 0),
                Bounds = new Rectangle(new Float2(0, 0), new Float2(0, 20))
            });
        }

        // Remove the oldest message if the limit is reached
        int Over = labels.Count - _settings.MaxMessages;
        if (Over > 0)
        {
            for (int i = 0; i < Over; i++)
            {
                labels[0].Dispose(); // Dispose the oldest label
                labels.RemoveAt(0); // Remove it from the list
            }
        }

        _pan.ScrollViewTo(_vpInfo, true); // Scroll to the latest message
    }

    /// <summary>
    /// Clears all messages from the console.
    /// </summary>
    public void ClearMessages()
    {
        // Dispose and clear all message labels
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].Dispose();
            labels[i] = null;
        }
        labels.Clear();
    }

    /// <summary>
    /// Called when the user finishes editing the textbox.
    /// </summary>    
    private void OnTextBoxEditEnd(TextBoxBase obj)
    {
        // Ignore empty input or specific characters
        if (_tb.Text == "" || _tb.Text == "`" || _tb.Text == "~")
            return;

        string _text = _tb.Text; // Store the entered text
        _tb.Text = ""; // Clear the textbox for new input

        TextInputEvent?.Invoke(_text); // Raise the text input event

        CheckCommands(_text); // Check and process the command
    }

    /// <summary>
    /// Updates the console state and checks for input.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(_settings.ConsoleKey))
        {
            DisplayConsole = !DisplayConsole; // Toggle console visibility
            _bdr.Enabled = DisplayConsole;
            _bdr.Visible = DisplayConsole;
        }

        if (DisplayConsole)
        {
            // Update bounds for UI elements when the console is displayed
            _bdr.Bounds = new Rectangle(new Float2(5, 0), new Float2(Screen.Size.X - 10, Screen.Size.Y / 2));
            _topVP.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, 0));
            _tb.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, 20));
            _pan.Bounds = new Rectangle(Float2.Zero, new Float2(_bdr.Bounds.Size.X, _bdr.Bounds.Size.Y - _tb.Bounds.Size.Y));
            _vpInfo.Bounds = new Rectangle(Float2.Zero, new Float2(_pan.Bounds.Size.X - _pan.ScrollBarsSize, _vpInfo.Bounds.Size.Y));

            // Focus the textbox when hitting enter
            if (Input.GetKeyDown(KeyboardKeys.Return) & !_tb.IsFocused)
                _tb.Focus();
        }
    }

    /// <summary>
    /// Disposes of the console and its UI components.
    /// </summary>
    public void Dispose()
    {
        if (WaitingTask is not null)
            if (!WaitingTask.IsCanceled)
                _cts.Cancel();

        // Dispose all message labels
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].Dispose();
            labels[i] = null;
        }

        _tb.TextBoxEditEnd -= OnTextBoxEditEnd; // Unsubscribe from the event

        labels.Clear(); // Clear the labels list
        labels = null; // Nullify the reference

        // Dispose of UI components
        _vpInfo.Dispose();
        _vpInfo = null;

        _pan.Dispose();
        _pan = null;

        _tb.Dispose();
        _tb = null;

        _topVP.Dispose();
        _topVP = null;

        _bdr.Dispose();
        _bdr = null;

        Commands = null; // Clear commands

        if (i is not null)
            i = null; // Clear the singleton instance

#if FLAX_EDITOR
        Debug.Log("Ingame Console Disposed");
#endif
    }
}
