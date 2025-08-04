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

        private TimesheetEntry CreateStandardEntry() => new TimesheetEntry
        {
            UserId = "user1",
            ProjectId = "PROJ-001",
            Date = DateTime.Today,
            Hours = 8.0m,
            Description = "Test work"
        };

        [Fact]
        public async Task AddEntryAsync_ValidEntry_ReturnsEntryWithId()
        {
            // Arrange
            var entry = CreateStandardEntry();

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
            var entry1 = CreateStandardEntry();
            var entry2 = CreateStandardEntry();

            await _service.AddEntryAsync(entry1);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddEntryAsync(entry2));
        }

        [Fact]
        public async Task GetEntriesForUserAndWeekAsync_ReturnsCorrectEntries()
        {
            // Arrange
            var monday = new DateTime(2024, 1, 1);
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
        public async Task GetProjectTotalsAsync_ReturnsCorrectTotals()
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

            var entries = await _service.GetEntriesForUserAndWeekAsync("user1", monday);

            // Act
            var result = await _service.GetProjectTotalsAsync(entries);

            // Assert
            Assert.Equal(12.0m, result["PROJ-001"]);
            Assert.Equal(6.0m, result["PROJ-002"]);
        }

        [Fact]
        public async Task GetEntriesForUserAndWeekAsync_EmptyUserId_ReturnsEmpty()
        {
            // Arrange
            await _service.AddEntryAsync(new TimesheetEntry { UserId = "user1", ProjectId = "PROJ-001", Date = DateTime.Today, Hours = 8.0m });

            // Act
            var result = await _service.GetEntriesForUserAndWeekAsync("", DateTime.Today);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteEntryAsync_RemovesEntry()
        {

            var entry = CreateStandardEntry();
            var added = await _service.AddEntryAsync(entry);

            // Act
            var result = await _service.DeleteEntryAsync(added.Id);

            // Assert
            Assert.True(result);
            var entries = await _service.GetEntriesForUserAndWeekAsync("user1", DateTime.Today);
            Assert.DoesNotContain(entries, e => e.Id == added.Id);
        }

        [Fact]
        public async Task DeleteEntryAsync_InvalidId_ReturnsFalse()
        {
            var result = await _service.DeleteEntryAsync(999);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateEntryAsync_UpdatesValues()
        {
            var entry = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-001",
                Date = DateTime.Today,
                Hours = 8
            };

            var added = await _service.AddEntryAsync(entry);

            var updatedEntry = new TimesheetEntry
            {
                UserId = "user1",
                ProjectId = "PROJ-002",
                Date = entry.Date.AddDays(1),
                Hours = 6,
                Description = "Updated"
            };

            var result = await _service.UpdateEntryAsync(added.Id, updatedEntry);

            Assert.NotNull(result);
            Assert.Equal("PROJ-002", result!.ProjectId);
            Assert.Equal(6, result.Hours);
            Assert.Equal("Updated", result.Description);
        }

        [Fact]
        public async Task UpdateEntryAsync_InvalidId_ReturnsNull()
        {
            var entry = CreateStandardEntry();

            var result = await _service.UpdateEntryAsync(999, entry);

            Assert.Null(result);
        }
    }
}