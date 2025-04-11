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

        // ✅ If testChoice contains appointment details, bypass menu and log immediately
        if (isTesting && testChoice.Contains("|"))
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
                        "View Appointment History",
                        "Back"
                    }));

            switch (choice)
            {
                case "Schedule Vet Appointment":
                    LogAppointment(console);
                    upcomingAppointment = GetUpcomingAppointment();
                    isAppointmentSoon = CheckAppointmentDate(upcomingAppointment);
                    break;
                case "View Appointment History":
                    ViewHistory(console);
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
                            string dateString = line.Substring("Vet appointment on: ".Length, 10);
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
        string appointmentTime, appointmentType;

        if (testChoice != null)
        {
            string[] testInputs = testChoice.Split('|');
            if (DateTime.TryParseExact(testInputs[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out appointmentDate))
            {
                appointmentTime = testInputs.Length > 1 ? testInputs[1] : "Unknown Time";
                appointmentType = testInputs.Length > 2 ? testInputs[2] : "General Checkup";
                console.WriteLine($"Test mode: Scheduling appointment -> {appointmentDate.ToString("yyyy-MM-dd")} at {appointmentTime} ({appointmentType})");
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

            appointmentTime = console.Prompt(
                new TextPrompt<string>("Enter appointment time (e.g., 'April 20, 10:00 AM'):")
                    .Validate(input => string.IsNullOrWhiteSpace(input) ? ValidationResult.Error("Time cannot be empty.") : ValidationResult.Success()));

            appointmentType = console.Prompt(
                new TextPrompt<string>("Enter appointment type (e.g., 'Vaccination, Checkup, Surgery'):")
                    .Validate(input => string.IsNullOrWhiteSpace(input) ? ValidationResult.Error("Type cannot be empty.") : ValidationResult.Success()));
        }

        string logEntry = $"Vet appointment on: {appointmentDate.ToString("yyyy-MM-dd")} at {appointmentTime} ({appointmentType})\n";

        try
        {
            File.AppendAllText(LogFilePath, logEntry);
            console.WriteLine($"Vet appointment scheduled: {appointmentDate.ToString("yyyy-MM-dd")} at {appointmentTime} ({appointmentType})");
        }
        catch (Exception ex)
        {
            console.WriteLine($"Error scheduling appointment: {ex.Message}");
        }
    }

    public static void ViewHistory(IAnsiConsole console) // ✅ Made public for testing
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
