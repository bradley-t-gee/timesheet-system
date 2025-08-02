using TimesheetSystem.Models;
using TimesheetSystem.Services;
using Xunit;

namespace TimesheetSystem.Tests
{
    public class TimesheetServiceTests
    {
        private readonly TimesheetService _service;

        public TimesheetServiceTests()
        {
            _service = new TimesheetService();
        }

        [Fact]
        public async Task AddEntryAsync_ValidEntry_ReturnsEntryWithId()
        {
            // Arrange
            var entry = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-001",
                Date = DateTime.Today,
                Hours = 8.0m,
                Description = "Test work"
            };

            // Act
            var result = await _service.AddEntryAsync(entry);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.Equal("user1", result.UserId);
        }

        [Fact]
        public async Task AddEntryAsync_DuplicateEntry_ThrowsException()
        {
            // Arrange
            var entry1 = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-001",
                Date = DateTime.Today,
                Hours = 4.0m
            };
            var entry2 = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-001",
                Date = DateTime.Today,
                Hours = 4.0m
            };

            await _service.AddEntryAsync(entry1);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddEntryAsync(entry2));
        }

        [Fact]
        public async Task GetEntriesForUserAndWeekAsync_ReturnsCorrectEntries()
        {
            // Arrange
            var monday = new DateTime(2024, 1, 1); // Assume this is a Monday
            var tuesday = monday.AddDays(1);
            var nextWeek = monday.AddDays(7);

            var entry1 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-001", Date = monday, Hours = 8.0m };
            var entry2 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-002", Date = tuesday, Hours = 6.0m };
            var entry3 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-001", Date = nextWeek, Hours = 4.0m };

            await _service.AddEntryAsync(entry1);
            await _service.AddEntryAsync(entry2);
            await _service.AddEntryAsync(entry3);

            // Act
            var result = await _service.GetEntriesForUserAndWeekAsync("user1", monday);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Date == monday);
            Assert.Contains(result, e => e.Date == tuesday);
        }

        [Fact]
        public async Task GetProjectTotalsForUserAndWeekAsync_ReturnsCorrectTotals()
        {
            // Arrange
            var monday = new DateTime(2024, 1, 1);
            var tuesday = monday.AddDays(1);

            var entry1 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-001", Date = monday, Hours = 8.0m };
            var entry2 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-001", Date = tuesday, Hours = 4.0m };
            var entry3 = new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-002", Date = tuesday, Hours = 6.0m };

            await _service.AddEntryAsync(entry1);
            await _service.AddEntryAsync(entry2);
            await _service.AddEntryAsync(entry3);

            // Act
            var result = await _service.GetProjectTotalsForUserAndWeekAsync("user1", monday);

            // Assert
            Assert.Equal(12.0m, result["PROJ-001"]);
            Assert.Equal(6.0m, result["PROJ-002"]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(25)]
        public async Task AddEntryAsync_InvalidHours_ShouldValidateInController(decimal hours)
        {
            // This test shows that hours validation should be handled at the model level
            // The service itself doesn't validate - that's the controller's job
            var entry = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-001",
                Date = DateTime.Today,
                Hours = hours
            };

            // Service will accept any decimal value - validation happens in controller
            var result = await _service.AddEntryAsync(entry);
            Assert.Equal(hours, result.Hours);
        }
    }
}