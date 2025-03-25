using Spectre.Console;
using System;
using System.IO;

namespace DogLog;

public class UC5
{
    private static readonly string InventoryFilePath = "inventory_log.txt";

    private static bool runningLowFood = false;
    private static bool runningLowTreats = false;

    public static void Handle()
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Pet Supply Inventory")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        $"Running Low on Food: {(runningLowFood ? "[red]yes (!)[/]" : "no")}",
                        $"Running Low on Treats: {(runningLowTreats ? "[red]yes (!)[/]" : "no")}",
                        "Log Food",
                        "Log Treats",
                        "View History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Running Low on Food: yes (!)" or "Running Low on Food: no":
                    runningLowFood = !runningLowFood;
                    break;
                case "Running Low on Treats: yes (!)" or "Running Low on Treats: no":
                    runningLowTreats = !runningLowTreats;
                    break;
                case "Log Food":
                    LogItem("Food");
                    runningLowFood = false; // Reset alert
                    break;
                case "Log Treats":
                    LogItem("Treats");
                    runningLowTreats = false; // Reset alert
                    break;
                case "View History":
                    ViewHistory();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private static void LogItem(string itemType)
    {
        DateTime now = DateTime.Now;
        string itemDetails = AnsiConsole.Ask<string>($"Enter {itemType} details:");
        string logEntry = $"{itemType} logged on: {now} - {itemDetails}\n";

        try
        {
            File.AppendAllText(InventoryFilePath, logEntry);
            AnsiConsole.WriteLine($"{itemType} logged: {now}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error logging {itemType}: {ex.Message}");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    private static void ViewHistory()
    {
        if (File.Exists(InventoryFilePath))
        {
            try
            {
                string history = File.ReadAllText(InventoryFilePath);
                AnsiConsole.WriteLine(history);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("No history found.");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
}