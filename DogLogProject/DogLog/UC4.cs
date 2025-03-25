using Spectre.Console;
using System;

namespace DogLog;

public class UC4
{
    public static void Handle()
    {
        AnsiConsole.WriteLine("Handling UC4: Track Exercise Activities");
        string exerciseDetails = AnsiConsole.Ask<string>("Enter exercise details:");
        AnsiConsole.WriteLine($"Exercise details logged: {exerciseDetails}");
        AnsiConsole.WriteLine("Exercise information stored.");
        AnsiConsole.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey();
        Console.Clear();
    }
}