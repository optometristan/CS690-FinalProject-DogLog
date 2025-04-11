using Spectre.Console.Testing;
using Xunit;
using DogLog;
using System;
using System.IO;
using System.Globalization;

public class UC1Test
{
    private const string TestLogFile = "test_flea_treatment_log.txt";

    [Fact]
    public void Should_Track_Flea_Medication_Correctly()
    {
        // Arrange: Create a test console instance
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // Act: Run UC1 logic and inject a predefined menu choice
        UC1.Handle(console, "Log Flea Treatment");

        // Assert: Verify expected output
        Assert.Contains("Treatment logged:", console.Output);
    }

    [Fact]
    public void Should_View_Treatment_History()
    {
        // Arrange: Create a test console instance
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false; 

        // ✅ Auto-clear test log before writing new entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create a test log file with **strict UTC date formatting**
        File.WriteAllText(TestLogFile, 
            "Treatment administered on: 2025-04-01\n" +
            "Treatment administered on: 2025-04-11\n");

        // Act: View history
        UC1.Handle(console, "View History");

        // Assert: Verify history output matches expected format
        Assert.Contains("Treatment administered on: 2025-04-01", console.Output);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Retrieve_Most_Recent_Treatment_Date()
    {
        // ✅ Auto-clear test log before inserting expected test dates
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Ensure recent treatment date retrieval works with proper UTC format
        File.WriteAllText(TestLogFile, 
            "Treatment administered on: 2025-04-01\n" +
            "Treatment administered on: 2025-04-11\n");

        string mostRecentDate = UC1.GetMostRecentTreatmentDate();

        // ✅ Ensure expected and retrieved dates match precisely
        Assert.Equal("2025-04-11", mostRecentDate);

        // Cleanup
        File.Delete(TestLogFile);
    }
}

