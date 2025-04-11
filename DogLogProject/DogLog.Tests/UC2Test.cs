using Spectre.Console.Testing;
using Xunit;
using DogLog;
using System;
using System.IO;
using System.Globalization;

public class UC2Test
{
    private const string TestLogFile = "test_vet_appointments.txt";

    [Fact]
    public void Should_Schedule_Vet_Appointment()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test appointment date as mock input
        UC2.Handle(console, "2025-04-20");

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Ensure expected test mode messages appear
        Assert.Contains($"Test mode: Using appointment date 2025-04-20", output);
        Assert.Contains($"Appointment scheduled: 2025-04-20", output);
    }

    [Fact]
    public void Should_View_Upcoming_Appointments()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Auto-clear test log before writing new entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create a test log file with **strict UTC date formatting**
        File.WriteAllText(TestLogFile, 
            "Vet appointment on: 2025-04-15\n" +
            "Vet appointment on: 2025-05-01\n");

        // Act: View upcoming appointments
        UC2.Handle(console, "View Appointments");

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Verify appointment history matches expected output
        Assert.Contains("Vet appointment on: 2025-04-15", output);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Retrieve_Next_Upcoming_Appointment()
    {
        // ✅ Auto-clear test log before inserting expected test dates
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Ensure upcoming appointment retrieval works with proper format
        File.WriteAllText(TestLogFile, 
            "Vet appointment on: 2025-04-15\n" +
            "Vet appointment on: 2025-05-01\n");

        string upcomingAppointment = UC2.GetUpcomingAppointment();

        // ✅ Ensure expected and retrieved dates match precisely
        Assert.Equal("2025-04-15", upcomingAppointment);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Alert_If_Appointment_Is_Soon()
    {
        // ✅ Auto-clear test log before inserting an appointment close to today's date
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Insert a vet appointment **within the next 7 days**
        File.WriteAllText(TestLogFile, 
            $"Vet appointment on: {DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd")}\n");

        string upcomingAppointment = UC2.GetUpcomingAppointment();
        bool isAppointmentSoon = UC2.CheckAppointmentDate(upcomingAppointment);

        // ✅ Ensure the appointment alert triggers properly
        Assert.True(isAppointmentSoon);

        // Cleanup
        File.Delete(TestLogFile);
    }
}
