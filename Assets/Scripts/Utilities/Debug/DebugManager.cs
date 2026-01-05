using UnityEngine;
using Harvey.Farm.Events;
using Harvey.SaveSystem;
using Harvey.Farm.TimeManagement;

namespace Harvey.Farm.Utilities
{
    public class DebugManager : Singleton<DebugManager>
    {

        [SerializeField] private ConsoleController consoleController;

        public static bool DebugOn { get; private set; } = false;

        public void ToggleDebug()
        {
            DebugOn = !DebugOn;

            if (DebugOn)
            {
                InputService.Instance.Actions.World.Disable();

                consoleController.gameObject.SetActive(DebugOn);
                consoleController.Toggle(DebugOn);
            }
            else
            {
                InputService.Instance.Actions.World.Enable();

                consoleController.Toggle(DebugOn);
                consoleController.gameObject.SetActive(DebugOn);
            }
        }

        #region ConsoleCommands

        /* ---------- helpers the UI can call ---------- */
        public static void Write(string msg, Color? c = null)
            => ConsoleController.Instance?.WriteLine(msg, c ?? Color.white);

        public static void WriteHelp()
        {
            Write("Commands:");
            foreach (var cmd in ConsoleCommands.All.Values)
                Write($"  {cmd.Usage}", Color.aquamarine);
        }

        public static void Clear()
        {
            ConsoleController.Instance?.Clear();
            Write("Console cleared", Color.grey);
        }

        /* ---------- actual game-side commands ---------- */
        public static void CreateCoffeeCrop(string[] a)
        {
            // createCoffee <name> <growSeconds> <aroma> <acid> <body>
            if (a.Length != 5
                || !float.TryParse(a[1], out var growSeconds)
                || !float.TryParse(a[2], out var aroma)
                || !float.TryParse(a[3], out var acid)
                || !float.TryParse(a[4], out var body))
            {
                Write(ConsoleCommands.All["createCoffee"].Usage, Color.yellow);
                return;
            }

            var crop = CoffeeManager.Instance.Create(a[0], growSeconds, aroma, acid, body);
            Write($"Created crop {crop.DisplayName} ({crop.Id})", Color.green);
        }

        public static void SetTime(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int hour) || hour < 0 || hour > 23)
            {
                Write("Usage: settime <hour> (0-23)", Color.yellow);
                return;
            }

            TimeManager.Instance.SetTime(hour);
            Write($"Time set to {hour:00}:00", Color.green);
        }

        public static void Cmd_Save(string[] args)
        {
            string slot = (args.Length >= 1 && !string.IsNullOrWhiteSpace(args[0]))
                            ? args[0]
                            : "autosave";

            SaveService.Instance.SaveGame(slot);
            Write($"Game saved to slot “{slot}”.", Color.green);
        }

        public static async void Cmd_Load(string[] args)
        {
            string slot = (args.Length >= 1 && !string.IsNullOrWhiteSpace(args[0]))
                            ? args[0]
                            : "autosave";

            await SaveService.Instance.LoadGame(slot);
            Write($"Attempted load from slot “{slot}”. "
                  + "Check log for section restores.", Color.green);
        }

        #endregion
    }
}