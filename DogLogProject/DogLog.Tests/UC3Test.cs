using Spectre.Console.Testing;
using Xunit;
using DogLog;
using System;
using System.IO;
using System.Globalization;

public class UC3Test
{
    private const string TestLogFile = "test_meals_log.txt";

    [Fact]
    public void Should_Log_Meal()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test meal entry as mock input
        UC3.Handle(console, "Chicken & Rice - 3:00 PM");

        // ✅ Capture console output explicitly
        var output = console.Output;

        // ✅ Print captured output for debugging
        Console.WriteLine("DEBUG: Captured Console Output:");
        Console.WriteLine(output);

        // ✅ Ensure expected test mode message appears
        Assert.Contains("Meal logged: Chicken & Rice - 3:00 PM", output);
    }


    [Fact]
    public void Should_View_Meal_History()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Clear test log before inserting entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create test meal log entries
        File.WriteAllText(TestLogFile, 
            "Meal logged: Chicken & Rice - 2025-04-15 12:30:00\n" +
            "Meal logged: Beef & Vegetables - 2025-04-16 18:45:00\n");

        // ✅ Explicitly call ViewMeals() instead of relying on Handle()
        UC3.ViewMeals(console);

        // ✅ Capture console output explicitly
        var output = console.Output;

        // ✅ Print captured output for debugging
        Console.WriteLine("DEBUG: Captured Console Output:");
        Console.WriteLine(output);

        // ✅ Verify meal history matches expected output
        Assert.Contains("Meal logged: Chicken & Rice - 2025-04-15 12:30:00", output);
        Assert.Contains("Meal logged: Beef & Vegetables - 2025-04-16 18:45:00", output);
    }


    [Fact]
    public void Should_Retrieve_Most_Recent_Meal()
    {
        // ✅ Clear test log before inserting entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Insert meals into test log
        File.WriteAllText(TestLogFile, 
            "Meal logged: Chicken & Rice - 2025-04-15 12:30:00\n" +
            "Meal logged: Beef & Vegetables - 2025-04-16 18:45:00\n");

        string mostRecentMeal = UC3.GetMostRecentMeal();

        // ✅ Ensure expected most recent meal is retrieved
        Assert.Equal("Meal logged: Beef & Vegetables - 2025-04-16 18:45:00", mostRecentMeal);

        // Cleanup
        File.Delete(TestLogFile);
    }
}
