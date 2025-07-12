using System;
using System.Collections.Generic;
using Harvey.Farm.Events;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] TMP_Text log;

    public static ConsoleController Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        input.onSubmit.RemoveAllListeners();
        input.onSubmit.AddListener(OnCommandSubmitted);

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.OnDebugModeToggled += Toggle;
    }
    void OnDisable()
    {
        GameEvents.OnDebugModeToggled -= Toggle;
    }

    public void Toggle(bool toggle)
    {
        if (toggle) input.ActivateInputField();
        else
        {
            input.DeactivateInputField();
            input.text = string.Empty;
        }
    }

    public void OnCommandSubmitted(string line)
    {
        input.text = string.Empty;
        if (string.IsNullOrWhiteSpace(line)) return;

        WriteLine("> " + line, Color.cyan);

        var parts = line.Split(' ');
        var name = parts[0].ToLowerInvariant();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (!ConsoleCommands.All.TryGetValue(name, out var cmd))
        {
            WriteLine($"Unknown command “{name}”", Color.yellow);
            return;
        }

        try { cmd.Execute(args); }
        catch (Exception e) { WriteLine($"Error: {e.Message}", Color.red); }
    }

    /* ---------- helpers ---------- */

    public void WriteLine(string msg, Color c)
    {
        log.text += $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>{msg}</color>\n";

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            log.rectTransform);

        var sr = log.GetComponentInParent<ScrollRect>();
        sr.verticalNormalizedPosition = 0f;
    }
    
    public void Clear()
    {
        log.text = string.Empty;
        input.text = string.Empty;
        input.ActivateInputField();
    }
}
