using Spectre.Console.Testing;
using Xunit;
using DogLog;
using System;
using System.IO;
using System.Globalization;

public class UC4Test
{
    private const string TestLogFile = "test_exercise_log.txt";

    [Fact]
    public void Should_Log_Exercise()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test exercise entry as mock input
        UC4.Handle(console, "Jogging - 30 minutes");

        // ✅ Capture console output explicitly
        var output = console.Output;

        // ✅ Print captured output for debugging
        Console.WriteLine("DEBUG: Captured Console Output:");
        Console.WriteLine(output);

        // ✅ Ensure expected test mode message appears
        Assert.Contains("Exercise logged: Jogging - 30 minutes", output);
    }


    [Fact]
    public void Should_View_Exercise_History()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Clear test log before inserting entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create test exercise log entries
        File.WriteAllText(TestLogFile, 
            "Exercise logged: Jogging - 2025-04-15 07:30:00\n" +
            "Exercise logged: Hiking - 2025-04-16 10:15:00\n");

        // Act: View exercise history
        UC4.ViewExercises(console);

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Verify exercise history matches expected output
        Assert.Contains("Exercise logged: Jogging - 2025-04-15 07:30:00", output);
        Assert.Contains("Exercise logged: Hiking - 2025-04-16 10:15:00", output);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Retrieve_Latest_Exercise()
    {
        // ✅ Clear test log before inserting entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Insert exercises into test log
        File.WriteAllText(TestLogFile, 
            "Exercise logged: Jogging - 2025-04-15 07:30:00\n" +
            "Exercise logged: Hiking - 2025-04-16 10:15:00\n");

        string latestExercise = UC4.GetLatestExercise();

        // ✅ Ensure expected latest exercise is retrieved
        Assert.Equal("Exercise logged: Hiking - 2025-04-16 10:15:00", latestExercise);

        // Cleanup
        File.Delete(TestLogFile);
    }
}
