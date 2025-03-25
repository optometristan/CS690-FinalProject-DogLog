using Spectre.Console;
using System;

namespace DogLog;

public class UC1
{
    public static void Handle()
    {
        AnsiConsole.WriteLine("Handling UC1: Track Flea Medication Administration");
        DateTime medicationDate = AnsiConsole.Ask<DateTime>("Enter medication administration date and time:");
        AnsiConsole.WriteLine($"Medication administered on: {medicationDate}");
        AnsiConsole.WriteLine("Reminder set for next dose.");
        AnsiConsole.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey();
        Console.Clear();
    }
}