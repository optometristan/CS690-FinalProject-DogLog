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
    public void Should_Schedule_Vet_Appointment_With_Time_And_Type()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Provide test appointment details as mock input
        UC2.Handle(console, "2025-04-20|10:30 AM|Vaccination");

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Ensure expected test mode messages appear
        Assert.Contains("Test mode: Scheduling appointment -> 2025-04-20 at 10:30 AM (Vaccination)", output);
        Assert.Contains("Vet appointment scheduled: 2025-04-20 at 10:30 AM (Vaccination)", output);
    }

    [Fact]
    public void Should_View_Upcoming_Appointments()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        // ✅ Clear test log before writing new entries
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Manually create a test log file with strict UTC date formatting
        File.WriteAllText(TestLogFile, 
            "Vet appointment on: 2025-04-15 at 3:00 PM (General Checkup)\n" +
            "Vet appointment on: 2025-05-01 at 11:00 AM (Dental Cleaning)\n");

        // Act: View appointment history
        UC2.ViewHistory(console);

        // ✅ Capture console output
        var output = console.Output;

        // ✅ Verify appointment history matches expected output
        Assert.Contains("Vet appointment on: 2025-04-15 at 3:00 PM (General Checkup)", output);
        Assert.Contains("Vet appointment on: 2025-05-01 at 11:00 AM (Dental Cleaning)", output);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Retrieve_Next_Upcoming_Appointment()
    {
        // ✅ Clear test log before inserting expected test dates
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Ensure upcoming appointment retrieval works with full format
        File.WriteAllText(TestLogFile, 
            "Vet appointment on: 2025-04-15 at 3:00 PM (General Checkup)\n" +
            "Vet appointment on: 2025-05-01 at 11:00 AM (Dental Cleaning)\n");

        string upcomingAppointment = UC2.GetUpcomingAppointment();

        // ✅ Ensure expected and retrieved dates match precisely
        Assert.Equal("2025-04-15", upcomingAppointment);

        // Cleanup
        File.Delete(TestLogFile);
    }

    [Fact]
    public void Should_Alert_If_Appointment_Is_Soon()
    {
        // ✅ Clear test log before inserting an appointment close to today's date
        if (File.Exists(TestLogFile))
        {
            File.Delete(TestLogFile);
        }

        // ✅ Insert a vet appointment **within the next 7 days**
        File.WriteAllText(TestLogFile, 
            $"Vet appointment on: {DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd")} at 2:00 PM (Wellness Check)\n");

        string upcomingAppointment = UC2.GetUpcomingAppointment();
        bool isAppointmentSoon = UC2.CheckAppointmentDate(upcomingAppointment);

        // ✅ Ensure the appointment alert triggers properly
        Assert.True(isAppointmentSoon);

        // Cleanup
        File.Delete(TestLogFile);
    }
}
