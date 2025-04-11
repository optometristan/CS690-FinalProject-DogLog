using Spectre.Console;
using System;

namespace DogLog;

public class ConsoleUI
{
    public static void Run(IAnsiConsole console, string? testChoice = null)
    {
        bool isTesting = testChoice != null;  // Check if we’re in test mode

        while (true)
        {
            string choice = isTesting ? testChoice : console.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(
                        "Track Flea Medication Administration (UC1)", 
                        "Schedule Vet Appointment Reminder (UC2)", 
                        "Log and Track Meals (UC3)", 
                        "Track Exercise Activities (UC4)", 
                        "Manage Pet Supply Inventory (UC5)", 
                        "Exit"
                    ));

            console.WriteLine($"Selected: {choice}");

            //  During tests, exit after first iteration (prevents freezing!)
            if (isTesting) return;

            if (choice == "Exit") return;

            switch (choice)
            {
                case "Track Flea Medication Administration (UC1)":
                    UC1.Handle(AnsiConsole.Console);
                    break;
                case "Schedule Vet Appointment Reminder (UC2)":
                    UC2.Handle(AnsiConsole.Console, null);
                    break;
                case "Log and Track Meals (UC3)":
                    UC3.Handle(AnsiConsole.Console, null);
                    break;
                case "Track Exercise Activities (UC4)":
                    UC4.Handle(AnsiConsole.Console, null);
                    break;
                case "Manage Pet Supply Inventory (UC5)":
                    UC5.Handle(AnsiConsole.Console, null);
                    break;
            }
        }
    }

    public static void Main(string[] args)
    {
        Run(AnsiConsole.Console);
    }
}
