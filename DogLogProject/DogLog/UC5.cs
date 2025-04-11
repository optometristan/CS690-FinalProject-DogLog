using Spectre.Console;
using System;
using System.IO;

namespace DogLog;

public class UC5
{
    private static readonly string InventoryFilePath = "inventory_log.txt";
    private static readonly string TestInventoryFilePath = "test_inventory_log.txt"; // Test-specific log file

    private static bool runningLowFood = false;
    private static bool runningLowTreats = false;

    public static void Handle(IAnsiConsole console, string? testChoice = null)
    {
        bool isTesting = testChoice != null;

        // If testChoice contains food or treats, bypass menu and log item immediately
        if (isTesting && !string.IsNullOrWhiteSpace(testChoice))
        {
            string itemType = testChoice.Contains("Food") ? "Food" : "Treats";
            LogItem(console, itemType, testChoice);
            return;
        }

        while (true)
        {
            string choice = isTesting ? testChoice : console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Pet Supply Inventory")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        $"Running Low on Food: {(runningLowFood ? "[red]yes (!)[/]" : "no")}",
                        $"Running Low on Treats: {(runningLowTreats ? "[red]yes (!)[/]": "no")}",
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
                    LogItem(console, "Food");
                    runningLowFood = false;
                    break;
                case "Log Treats":
                    LogItem(console, "Treats");
                    runningLowTreats = false;
                    break;
                case "View History":
                    ViewHistory(console);
                    break;
                case "Back":
                    return;
            }

            if (isTesting) return;
        }
    }


    public static void LogItem(IAnsiConsole console, string itemType, string? testChoice = null)
    {
        string itemDetails;

        if (testChoice != null)
        {
            itemDetails = testChoice;
            console.WriteLine($"Test mode: Logging {itemType} -> {itemDetails}"); // Force test output
        }
        else
        {
            itemDetails = console.Prompt(
                new TextPrompt<string>($"Enter {itemType} details:")
                    .Validate(input => string.IsNullOrWhiteSpace(input) ? ValidationResult.Error($"{itemType} details cannot be empty.") : ValidationResult.Success()));
        }

        string fileToWrite = File.Exists(TestInventoryFilePath) ? TestInventoryFilePath : InventoryFilePath;
        string logEntry = $"{itemType} logged on: {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} - {itemDetails}\n";

        try
        {
            File.AppendAllText(fileToWrite, logEntry);
            console.WriteLine($"{itemType} logged: {itemDetails}"); // Ensure test output appears
        }
        catch (Exception ex)
        {
            console.WriteLine($"Error logging {itemType}: {ex.Message}");
        }
    }


    public static void ViewHistory(IAnsiConsole console) // Made public for testing
    {
        string fileToRead = File.Exists(TestInventoryFilePath) ? TestInventoryFilePath : InventoryFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string history = File.ReadAllText(fileToRead);
                console.WriteLine(history); // Ensure history prints correctly
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            console.WriteLine("No history found.");
        }
    }
}
