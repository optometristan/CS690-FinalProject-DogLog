using Spectre.Console;
using System;
using System.IO;
using System.Globalization;
using System.Linq;

namespace DogLog;

public class UC4
{
    private static readonly string LogFilePath = "exercise_log.txt";
    private static readonly string TestLogFilePath = "test_exercise_log.txt"; // Test-specific log file

    public static void Handle(IAnsiConsole console, string? testChoice = null)
    {
        string latestExercise = GetLatestExercise();
        bool isTesting = testChoice != null;

        // If testChoice is an exercise entry, bypass menu and log exercise immediately
        if (isTesting && !string.IsNullOrWhiteSpace(testChoice))
        {
            LogExercise(console, testChoice);
            return;
        }

        while (true)
        {
            string choice = isTesting ? testChoice : console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Exercise Tracking")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(latestExercise) ? "No Recent Exercise Logged" : $"Latest Activity: {latestExercise}")}")
                    .AddChoices(new[]
                    {
                        "Log Exercise",
                        "View Exercise History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Log Exercise":
                    LogExercise(console);
                    latestExercise = GetLatestExercise();
                    break;
                case "View Exercise History":
                    ViewExercises(console);
                    break;
                case "Back":
                    return;
            }

            if (isTesting) return;
        }
    }


    public static string GetLatestExercise()
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

    private static void LogExercise(IAnsiConsole console, string? testChoice = null)
    {
        string exerciseEntry;

        if (testChoice != null)
        {
            exerciseEntry = testChoice;
            console.WriteLine($"Test mode: Logging exercise -> {exerciseEntry}"); // Force test output
        }
        else
        {
            exerciseEntry = console.Prompt(
                new TextPrompt<string>("Enter exercise details (e.g., 'Jogging - 30 minutes'):")
                    .Validate(input => string.IsNullOrWhiteSpace(input) ? ValidationResult.Error("Exercise details cannot be empty.") : ValidationResult.Success()));
        }

        string fileToWrite = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;
        string logEntry = $"Exercise logged: {exerciseEntry} - {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}\n";

        try
        {
            File.AppendAllText(fileToWrite, logEntry);
            console.WriteLine($"Exercise logged: {exerciseEntry}"); // Ensure test output appears
        }
        catch (Exception ex)
        {
            console.WriteLine($"Error logging exercise: {ex.Message}");
        }
    }

    public static void ViewExercises(IAnsiConsole console)
    {
        string fileToRead = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string history = File.ReadAllText(fileToRead);
                console.WriteLine(history); // Ensure history prints correctly
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error reading exercise history: {ex.Message}");
            }
        }
        else
        {
            console.WriteLine("No exercise history found.");
        }
    }
}
