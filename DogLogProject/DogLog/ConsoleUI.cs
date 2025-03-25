using Spectre.Console;
using System;

namespace DogLog;

public class ConsoleUI
{
    public static void Main(string[] args)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Track Flea Medication Administration (UC1)",
                        "Schedule Vet Appointment Reminder (UC2)",
                        "Log and Track Meals (UC3)",
                        "Track Exercise Activities (UC4)",
                        "Manage Pet Supply Inventory (UC5)",
                        "Exit"
                    }));

            switch (choice)
            {
                case "Track Flea Medication Administration (UC1)":
                    UC1.Handle();
                    break;
                case "Schedule Vet Appointment Reminder (UC2)":
                    UC2.Handle();
                    break;
                case "Log and Track Meals (UC3)":
                    UC3.Handle();
                    break;
                case "Track Exercise Activities (UC4)":
                    UC4.Handle();
                    break;
                case "Manage Pet Supply Inventory (UC5)":
                    UC5.Handle();
                    break;
                case "Exit":
                    return;
            }
        }
    }
}