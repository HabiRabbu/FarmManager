// Runtime-only – put in a new file DebugConsoleCommand.cs
using System;

public struct DebugConsoleCommand
{
    public readonly string   Name;
    public readonly string   Usage; // To let the user know how it's actally used
    public readonly Action<string[]> Execute;

    public DebugConsoleCommand(string name, string usage, Action<string[]> exec)
    {
        Name    = name;
        Usage   = usage;
        Execute = exec;
    }
}
