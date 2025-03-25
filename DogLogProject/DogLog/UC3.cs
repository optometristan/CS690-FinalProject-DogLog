using Spectre.Console;
using System;

namespace DogLog;

public class UC3
{
    public static void Handle()
    {
        AnsiConsole.WriteLine("Handling UC3: Log and Track Meals");
        string mealDetails = AnsiConsole.Ask<string>("Enter meal details:");
        AnsiConsole.WriteLine($"Meal details logged: {mealDetails}");
        AnsiConsole.WriteLine("Meal information stored.");
        AnsiConsole.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey();
        Console.Clear();
    }
}