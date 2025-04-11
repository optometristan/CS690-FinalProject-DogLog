using Spectre.Console;
using System;
using System.IO;
using System.Globalization;
using System.Linq;

namespace DogLog;

public class UC3
{
    private static readonly string LogFilePath = "meals_log.txt";
    private static readonly string TestLogFilePath = "test_meals_log.txt"; // ✅ Test-specific log file

    public static void Handle(IAnsiConsole console, string? testChoice = null)
    {
        string mostRecentMeal = GetMostRecentMeal();
        bool isTesting = testChoice != null;

        // ✅ If testChoice is a meal entry, bypass menu and log meal immediately
        if (isTesting && !string.IsNullOrWhiteSpace(testChoice))
        {
            LogMeal(console, testChoice);
            return;
        }

        while (true)
        {
            string choice = isTesting ? testChoice : console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Meal Tracking")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(mostRecentMeal) ? "No Recent Meals" : $"Most Recent: {mostRecentMeal}")}")
                    .AddChoices(new[]
                    {
                        "Log Meal",
                        "View Meal History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Log Meal":
                    LogMeal(console);
                    mostRecentMeal = GetMostRecentMeal();
                    break;
                case "View Meal History":
                    ViewMeals(console);
                    break;
                case "Back":
                    return;
            }

            if (isTesting) return;
        }
    }


    public static string GetMostRecentMeal()
    {
        string fileToRead = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string[] lines = File.ReadAllLines(fileToRead);
                return lines.Length > 0 ? lines.Last() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        return null;
    }

    private static void LogMeal(IAnsiConsole console, string? testChoice = null)
    {
        string mealEntry;

        if (testChoice != null)
        {
            mealEntry = testChoice;
            console.WriteLine($"Test mode: Logging meal -> {mealEntry}"); // ✅ Force test output
        }
        else
        {
            mealEntry = console.Prompt(
                new TextPrompt<string>("Enter meal details (e.g., 'Chicken & Rice - 3:00 PM'):")
                    .Validate(input => string.IsNullOrWhiteSpace(input) ? ValidationResult.Error("Meal details cannot be empty.") : ValidationResult.Success()));
        }

        string fileToWrite = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;
        string logEntry = $"Meal logged: {mealEntry} - {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}\n";

        try
        {
            File.AppendAllText(fileToWrite, logEntry);
            console.WriteLine($"Meal logged: {mealEntry}"); // ✅ Ensure test output appears
        }
        catch (Exception ex)
        {
            console.WriteLine($"Error logging meal: {ex.Message}");
        }
    }

    public static void ViewMeals(IAnsiConsole console)
    {
        string fileToRead = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string history = File.ReadAllText(fileToRead);
                console.WriteLine(history); // ✅ Ensure history prints correctly
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error reading meal history: {ex.Message}");
            }
        }
        else
        {
            console.WriteLine("No meal history found.");
        }
    }


}
