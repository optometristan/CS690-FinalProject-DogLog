using Spectre.Console;
using System;
using System.IO;
using System.Linq;

namespace DogLog;

public class UC4
{
    private static readonly string ExerciseFilePath = "exercise_log.txt";

    public static void Handle()
    {
        string mostRecentExercise = GetMostRecentExercise();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Exercise Logging")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(mostRecentExercise) ? "No Recent Exercise" : $"Most Recent: {mostRecentExercise}")}")
                    .AddChoices(new[]
                    {
                        "Log Exercise",
                        "View History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Log Exercise":
                    LogExercise();
                    mostRecentExercise = GetMostRecentExercise();
                    break;
                case "View History":
                    ViewHistory();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private static string GetMostRecentExercise()
    {
        if (File.Exists(ExerciseFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(ExerciseFilePath);
                if (lines.Length > 0)
                {
                    var dates = lines.Select(line =>
                    {
                        if (line.StartsWith("Exercise logged on: "))
                        {
                            string dateString = line.Substring("Exercise logged on: ".Length).Split(" - ")[0];
                            if (DateTime.TryParse(dateString, out DateTime date))
                            {
                                return date;
                            }
                        }
                        return DateTime.MinValue;
                    }).Where(date => date != DateTime.MinValue).OrderByDescending(date => date).ToList();

                    if (dates.Count > 0)
                    {
                        return dates[0].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        return null;
    }

    private static void LogExercise()
    {
        DateTime now = DateTime.Now;
        string exerciseDetails = AnsiConsole.Ask<string>("Enter exercise details:");
        string logEntry = $"Exercise logged on: {now} - {exerciseDetails}\n";

        try
        {
            File.AppendAllText(ExerciseFilePath, logEntry);
            AnsiConsole.WriteLine($"Exercise logged: {now}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error logging exercise: {ex.Message}");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    private static void ViewHistory()
    {
        if (File.Exists(ExerciseFilePath))
        {
            try
            {
                string history = File.ReadAllText(ExerciseFilePath);
                AnsiConsole.WriteLine(history);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("No exercise history found.");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
}