using Spectre.Console;
using System;

namespace DogLog;

public class UC5
{
    public static void Handle()
    {
        AnsiConsole.WriteLine("Handling UC5: Manage Pet Supply Inventory");
        string supplyDetails = AnsiConsole.Ask<string>("Enter supply details (e.g., food, treats):");
        int quantity = AnsiConsole.Ask<int>("Enter quantity:");
        AnsiConsole.WriteLine($"Supply details logged: {supplyDetails}, Quantity: {quantity}");
        AnsiConsole.WriteLine("Inventory updated.");
        AnsiConsole.WriteLine("Press any key to return to the main menu.");
        Console.ReadKey();
        Console.Clear();
    }
}