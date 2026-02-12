using Bogus;
using EduSchedule.Domain.Students.Entities;
using FluentAssertions;
using Xunit;

namespace EduSchedule.UnitTests.Domain.Students.Entities
{
    public class EventTests
    {
        private readonly Faker _faker = new Faker();

        [Fact]
        public void Constructor_ShouldInitializeEvent_WithCorrectValues()
        {
            // Arrange
            var externalId = _faker.Random.Guid().ToString();
            var subject = _faker.Lorem.Sentence(3);
            var startTime = _faker.Date.Soon();
            var endTime = startTime.AddHours(1);
            var studentId = _faker.Random.Int(1, 1000);

            // Act
            var evt = new Event(externalId, subject, startTime, endTime, studentId);

            // Assert
            evt.ExternalId.Should().Be(externalId);
            evt.Subject.Should().Be(subject);
            evt.StartTime.Should().Be(startTime);
            evt.EndTime.Should().Be(endTime);
            evt.StudentId.Should().Be(studentId);
            evt.IsActive.Should().BeTrue();
            evt.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            evt.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldModifyProperties_AndSetTimestamp()
        {
            // Arrange
            var evt = new Event(
                "ext-1", 
                "Math Class", 
                DateTime.Now, 
                DateTime.Now.AddHours(1), 
                10
            );

            var newSubject = "Advanced Math";
            var newStart = DateTime.Now.AddDays(1);
            var newEnd = newStart.AddHours(2);
            var creationTime = evt.CreatedAt;

            await Task.Delay(20);

            // Act
            evt.Update(newSubject, newStart, newEnd);

            // Assert
            evt.Subject.Should().Be(newSubject);
            evt.StartTime.Should().Be(newStart);
            evt.EndTime.Should().Be(newEnd);
            evt.UpdatedAt.Should().NotBeNull();
            evt.UpdatedAt.Should().BeAfter(creationTime);
            evt.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            evt.ExternalId.Should().Be("ext-1");
            evt.StudentId.Should().Be(10);
        }

        [Fact]
        public async Task Inactivate_ShouldSetIsActiveFalse_AndSetTimestamp()
        {
            // Arrange
            var evt = new Event(
                "ext-1", 
                "History", 
                DateTime.Now, 
                DateTime.Now.AddHours(1), 
                10
            );
            
            await Task.Delay(20);

            // Act
            evt.Inactivate();

            // Assert
            evt.IsActive.Should().BeFalse();
            evt.UpdatedAt.Should().NotBeNull();
            evt.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void NavigationProperties_ShouldBeSettable()
        {
            // Arrange
            var evt = new Event("1", "A", DateTime.Now, DateTime.Now, 1);
            var student = new Student("123", "Student Name", "email@test.com");

            // Act
            evt.Student = student;

            // Assert
            evt.Student.Should().NotBeNull();
            evt.Student.DisplayName.Should().Be("Student Name");
        }
    }
}