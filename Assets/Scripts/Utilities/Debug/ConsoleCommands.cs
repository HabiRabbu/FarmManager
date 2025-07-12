using System.Collections.Generic;
using Harvey.Farm.Utilities;

public static class ConsoleCommands
{
    public static readonly IReadOnlyDictionary<string, DebugConsoleCommand> All
        = new Dictionary<string, DebugConsoleCommand>
        {
            // ─────────── core ───────────
            ["help"] = new DebugConsoleCommand(
                "help",
                "help - list every console command.",
                _ => DebugManager.WriteHelp()),

            ["echo"] = new DebugConsoleCommand(
                "echo",
                "echo <text>",
                a => DebugManager.Write(string.Join(" ", a))),

            ["clear"] = new DebugConsoleCommand(
                "clear",
                "clear - clear the console output.",
                _ => DebugManager.Clear()),

            // ─────────── gameplay ───────
            ["savegame"] = new DebugConsoleCommand(
                "savegame",
                "savegame <saveFileName> - save the current game state (default “autosave”).",
                a => DebugManager.Cmd_Save(a)),

            ["loadgame"] = new DebugConsoleCommand(
                "loadgame",
                "loadgame <saveFileName> - load a saved game state (default “autosave”).",
                a => DebugManager.Cmd_Load(a)),

            ["createcoffee"] = new DebugConsoleCommand(
                "createcoffee",
                "createcoffee <name> <growSeconds> <aroma> <acid> <body>",
                a => DebugManager.CreateCoffeeCrop(a)),


        };
}
