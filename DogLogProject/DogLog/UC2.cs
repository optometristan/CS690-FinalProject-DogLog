using Spectre.Console;
using System;
using System.IO;
using System.Linq;
using System.Globalization;

namespace DogLog;

public class UC2
{
    private static readonly string AppointmentFilePath = "vet_appointments.txt";

    public static void Handle()
    {
        string nextAppointment = GetNextAppointment();
        bool isAppointmentOld = CheckAppointmentAge(nextAppointment);

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vet Appointment Scheduling")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(nextAppointment) ? "No Upcoming Appointment" : $"Next Appointment: {(isAppointmentOld ? "[red]" + nextAppointment + " (!)[/]" : nextAppointment)}")}")
                    .AddChoices(new[]
                    {
                        "Schedule Appointment",
                        "View History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Schedule Appointment":
                    ScheduleAppointment();
                    nextAppointment = GetNextAppointment();
                    isAppointmentOld = CheckAppointmentAge(nextAppointment);
                    break;
                case "View History":
                    ViewHistory();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private static string GetNextAppointment()
    {
        if (File.Exists(AppointmentFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(AppointmentFilePath);
                if (lines.Length > 0)
                {
                    var appointments = lines.Select(line =>
                    {
                        if (line.StartsWith("Appointment scheduled for: "))
                        {
                            string dateString = line.Substring("Appointment scheduled for: ".Length);
                            if (DateTime.TryParse(dateString, out DateTime date))
                            {
                                return date;
                            }
                        }
                        return DateTime.MaxValue; // Default to MaxValue if parsing fails
                    }).Where(date => date != DateTime.MaxValue).Where(date => date >= DateTime.Now).OrderBy(date => date).ToList();

                    if (appointments.Count > 0)
                    {
                        return appointments[0].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        return null;
    }

    private static bool CheckAppointmentAge(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return false;
        }

        if (DateTime.TryParse(dateString, out DateTime appointmentDate))
        {
            return appointmentDate < DateTime.Now; // Check if the appointment is in the past
        }

        return false;
    }

    private static void ScheduleAppointment()
    {
        var month = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Month:")
                .AddChoices(DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToArray()));

        int day;
        while (true)
        {
            day = AnsiConsole.Ask<int>("Enter Day (1-31):");
            if (day >= 1 && day <= 31)
            {
                break;
            }
            AnsiConsole.WriteLine("[red]Invalid day. Please enter a value between 1 and 31.[/]");
        }

        int hour;
        while (true)
        {
            hour = AnsiConsole.Ask<int>("Enter Hour (0-23):");
            if (hour >= 0 && hour <= 23)
            {
                break;
            }
            AnsiConsole.WriteLine("[red]Invalid hour. Please enter a value between 0 and 23.[/]");
        }

        int minute;
        while (true)
        {
            minute = AnsiConsole.Ask<int>("Enter Minute (0-59):");
            if (minute >= 0 && minute <= 59)
            {
                break;
            }
            AnsiConsole.WriteLine("[red]Invalid minute. Please enter a value between 0 and 59.[/]");
        }

        int year = DateTime.Now.Year;

        DateTime appointmentDate = new DateTime(year, DateTime.ParseExact(month, "MMMM", null).Month, day, hour, minute, 0);

        if (appointmentDate < DateTime.Now)
        {
            appointmentDate = appointmentDate.AddYears(1);
        }

        string logEntry = $"Appointment scheduled for: {appointmentDate}\n";

        try
        {
            File.AppendAllText(AppointmentFilePath, logEntry);
            AnsiConsole.WriteLine($"Appointment scheduled: {appointmentDate}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"Error scheduling appointment: {ex.Message}");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }

    private static void ViewHistory()
    {
        if (File.Exists(AppointmentFilePath))
        {
            try
            {
                string history = File.ReadAllText(AppointmentFilePath);
                AnsiConsole.WriteLine(history);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Error reading history: {ex.Message}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("No appointment history found.");
        }

        AnsiConsole.WriteLine("Press any key to continue.");
        Console.ReadKey();
        Console.Clear();
    }
}