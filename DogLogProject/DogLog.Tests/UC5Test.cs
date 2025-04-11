using Spectre.Console.Testing;
using Xunit;
using DogLog;
using System;
using System.IO;
using System.Globalization;

public class UC5Test
{
    private const string TestLogFile = "test_inventory_log.txt";

    [Fact]
    public void Should_Log_Food()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test input for food logging
        UC5.Handle(console, "Dog Food - 10 lbs");

        // ✅ Capture console output explicitly
        var output = console.Output;

        // ✅ Print captured output for debugging
        Console.WriteLine("DEBUG: Captured Console Output:");
        Console.WriteLine(output);

        // ✅ Ensure expected test mode message appears
        Assert.Contains("Food logged: Dog Food - 10 lbs", output);
    }

    [Fact]
    public void Should_Log_Treats()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test input for treats logging
        UC5.Handle(console, "Peanut Butter Treats - 3 bags");

        // ✅ Capture console output explicitly
        var output = console.Output;

        // ✅ Print captured output for debugging
        Console.WriteLine("DEBUG: Captured Console Output:");
        Console.WriteLine(output);

        // ✅ Ensure expected test mode message appears
        Assert.Contains("Treats logged: Peanut Butter Treats - 3 bags", output);
    }
    
    [Fact]
    public void Should_View_Inventory_History()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Clear test log before inserting entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create test inventory log entries
        File.WriteAllText(TestLogFile, 
            "Food logged on: 2025-04-15 12:30:00 - Dog Food - 10 lbs\n" +
            "Treats logged on: 2025-04-16 15:45:00 - Peanut Butter Treats - 3 bags\n");

        // Act: View inventory history
        UC5.ViewHistory(console);

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Verify inventory history matches expected output
        Assert.Contains("Food logged on: 2025-04-15 12:30:00 - Dog Food - 10 lbs", output);
        Assert.Contains("Treats logged on: 2025-04-16 15:45:00 - Peanut Butter Treats - 3 bags", output);

        // Cleanup
        File.Delete(TestLogFile);
    }
}
