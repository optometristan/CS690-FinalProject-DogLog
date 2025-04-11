using Spectre.Console;
using System;
using System.IO;
using System.Linq;
using System.Globalization;

namespace DogLog;

public class UC2
{
    private static readonly string LogFilePath = "vet_appointments.txt";
    private static readonly string TestLogFilePath = "test_vet_appointments.txt"; 

    public static void Handle(IAnsiConsole console, string? testChoice = null)
    {
        string upcomingAppointment = GetUpcomingAppointment();
        bool isAppointmentSoon = CheckAppointmentDate(upcomingAppointment);
        bool isTesting = testChoice != null;

        // ✅ If testChoice is a direct date input, bypass menu and log appointment immediately
        if (isTesting && DateTime.TryParseExact(testChoice, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            LogAppointment(console, testChoice);
            return;
        }

        while (true)
        {
            string choice = isTesting ? testChoice : console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Vet Appointment Scheduler")
                    .PageSize(4)
                    .Title($"{(string.IsNullOrEmpty(upcomingAppointment) ? "No Upcoming Appointment" : $"Next Appointment: {(isAppointmentSoon ? "[red]" + upcomingAppointment + " (!)[/]" : upcomingAppointment)}")}")
                    .AddChoices(new[]
                    {
                        "Schedule Vet Appointment",
                        "View Appointments",
                        "Back"
                    }));

            switch (choice)
            {
                case "Schedule Vet Appointment":
                    LogAppointment(console);
                    upcomingAppointment = GetUpcomingAppointment();
                    isAppointmentSoon = CheckAppointmentDate(upcomingAppointment);
                    break;
                case "View Appointments":
                    ViewAppointments(console);
                    break;
                case "Back":
                    return;
            }

            if (isTesting) return;
        }
    }

    public static string GetUpcomingAppointment()
    {
        string fileToRead = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string[] lines = File.ReadAllLines(fileToRead);
                if (lines.Length > 0)
                {
                    var dates = lines.Select(line =>
                    {
                        if (line.StartsWith("Vet appointment on: "))
                        {
                            string dateString = line.Substring("Vet appointment on: ".Length);
                            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date))
                            {
                                return date.ToString("yyyy-MM-dd");
                            }
                        }
                        return null;
                    }).Where(date => date != null).OrderBy(date => DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();

                    if (dates.Count > 0)
                    {
                        return dates[0]; 
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        return null;
    }

    public static bool CheckAppointmentDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString)) return false;

        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime appointmentDate))
        {
            return appointmentDate < DateTime.UtcNow.AddDays(7); 
        }

        return false;
    }

    private static void LogAppointment(IAnsiConsole console, string? testChoice = null)
    {
        DateTime appointmentDate;

        if (testChoice != null)
        {
            if (DateTime.TryParseExact(testChoice, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out appointmentDate))
            {
                console.WriteLine($"Test mode: Using appointment date {appointmentDate.ToString("yyyy-MM-dd")}");
            }
            else
            {
                console.WriteLine("Test mode: Invalid date format.");
                return;
            }
        }
        else
        {
            appointmentDate = console.Prompt(
                new TextPrompt<DateTime>("Enter the appointment date (YYYY-MM-DD):")
                    .Validate(date =>
                        date >= DateTime.UtcNow ? ValidationResult.Success() : ValidationResult.Error("Date must be in the future!")));
        }

        string logEntry = $"Vet appointment on: {appointmentDate.ToString("yyyy-MM-dd")}\n";

        try
        {
            File.AppendAllText(LogFilePath, logEntry);
            console.WriteLine($"Appointment scheduled: {appointmentDate.ToString("yyyy-MM-dd")}");
        }
        catch (Exception ex)
        {
            console.WriteLine($"Error scheduling appointment: {ex.Message}");
        }
    }

    private static void ViewAppointments(IAnsiConsole console)
    {
        string fileToRead = File.Exists(TestLogFilePath) ? TestLogFilePath : LogFilePath;

        if (File.Exists(fileToRead))
        {
            try
            {
                string history = File.ReadAllText(fileToRead);
                console.WriteLine(history);
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error reading appointment history: {ex.Message}");
            }
        }
        else
        {
            console.WriteLine("No upcoming appointments found.");
        }
    }
}
