using Spectre.Console;
using System;
using System.IO;
using System.Linq;

namespace DogLog;

public class UC1
{
    private static readonly string LogFilePath = "flea_treatment_log.txt";

    public static void Handle()
    {
        string mostRecentDate = GetMostRecentTreatmentDate();
        bool isDateOld = CheckDateAge(mostRecentDate);

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Flea Medication Administration")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(mostRecentDate) ? "No Recent Treatment" : $"Most Recent: {(isDateOld ? "[red]" + mostRecentDate + " (!)[/]" : mostRecentDate)}")}")
                    .AddChoices(new[]
                    {
                        "Log Flea Treatment",
                        "View History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Log Flea Treatment":
                    LogTreatment();
                    mostRecentDate = GetMostRecentTreatmentDate();
                    isDateOld = CheckDateAge(mostRecentDate);
                    break;
                case "View History":
                    ViewHistory();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private static string GetMostRecentTreatmentDate()
    {
        if (File.Exists(LogFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(LogFilePath);
                if (lines.Length > 0)
                {
                    var dates = lines.Select(line =>
                    {
                        if (line.StartsWith("Treatment administered on: "))
                        {
                            string dateString = line.Substring("Treatment administered on: ".Length);
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

    private static bool CheckDateAge(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return false;
        }

        if (DateTime.TryParse(dateString, out DateTime recentDate))
        {
            return recentDate < DateTime.Now.AddMonths(-1);
        }

        return false;
    }

    private static void LogTreatment()
    {
        DateTime now = DateTime.Now;
        string logEntry = $"Treatment administered on: {now}\n";

        try
        {
            File.AppendAllText(LogFilePath, logEntry);
            AnsiConsole.WriteLine($"Treatment logged: {now}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error logging treatment: {ex.Message}");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    private static void ViewHistory()
    {
        if (File.Exists(LogFilePath))
        {
            try
            {
                string history = File.ReadAllText(LogFilePath);
                AnsiConsole.WriteLine(history);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("No treatment history found.");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
}