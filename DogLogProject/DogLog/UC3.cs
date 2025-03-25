using Spectre.Console;
using System;
using System.IO;
using System.Linq;

namespace DogLog;

public class UC3
{
    private static readonly string MealsFilePath = "meals_log.txt";

    public static void Handle()
    {
        string mostRecentMeal = GetMostRecentMeal();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Meal Logging")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(mostRecentMeal) ? "No Recent Meals" : $"Most Recent: {mostRecentMeal}")}")
                    .AddChoices(new[]
                    {
                        "Log Meal",
                        "View History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Log Meal":
                    LogMeal();
                    mostRecentMeal = GetMostRecentMeal();
                    break;
                case "View History":
                    ViewHistory();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private static string GetMostRecentMeal()
    {
        if (File.Exists(MealsFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(MealsFilePath);
                if (lines.Length > 0)
                {
                    var dates = lines.Select(line =>
                    {
                        if (line.StartsWith("Meal logged on: "))
                        {
                            string dateString = line.Substring("Meal logged on: ".Length).Split(" - ")[0];
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

    private static void LogMeal()
    {
        DateTime now = DateTime.Now;
        string mealDetails = AnsiConsole.Ask<string>("Enter meal details:");
        string logEntry = $"Meal logged on: {now} - {mealDetails}\n";

        try
        {
            File.AppendAllText(MealsFilePath, logEntry);
            AnsiConsole.WriteLine($"Meal logged: {now}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error logging meal: {ex.Message}");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    private static void ViewHistory()
    {
        if (File.Exists(MealsFilePath))
        {
            try
            {
                string history = File.ReadAllText(MealsFilePath);
                AnsiConsole.WriteLine(history);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("No meal history found.");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
}