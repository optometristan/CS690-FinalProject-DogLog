using Spectre.Console;
using System;

namespace DogLog;

public class UC2
{
    public static void Handle()
    {
        AnsiConsole.WriteLine("Handling UC2: Schedule Vet Appointment Reminder");
        DateTime appointmentDate = AnsiConsole.Ask<DateTime>("Enter vet appointment date and time:");
        AnsiConsole.WriteLine($"Vet appointment scheduled for: {appointmentDate}");
        AnsiConsole.WriteLine("Reminder set for appointment.");
        AnsiConsole.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey();
        Console.Clear();
    }
}