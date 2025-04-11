using Spectre.Console.Testing;
using Xunit;
using DogLog;

public class ConsoleUITest
{
    [Theory]
    [InlineData("Track Flea Medication Administration (UC1)")]
    [InlineData("Schedule Vet Appointment Reminder (UC2)")]
    [InlineData("Log and Track Meals (UC3)")]
    [InlineData("Track Exercise Activities (UC4)")]
    [InlineData("Manage Pet Supply Inventory (UC5)")]
    [InlineData("Exit")]
    public void Should_Display_Menu_Options(string menuChoice)
    {
        // Arrange: Create a test console instance
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false; // 🚀 Mark as non-interactive for testing

        // Act: Run the UI using the test console and mock menu choice
        ConsoleUI.Run(console, menuChoice);

        // Assert: Verify expected output
        Assert.Contains($"Selected: {menuChoice}", console.Output);
    }
}
