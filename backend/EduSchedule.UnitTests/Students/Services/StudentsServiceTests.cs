using Bogus;
using EduSchedule.Domain.Exceptions;
using EduSchedule.Domain.Students.Entities;
using EduSchedule.Domain.Students.Repositories;
using EduSchedule.Domain.Students.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace EduSchedule.UnitTests.Domain.Students.Services
{
    public class StudentsServiceTests
    {
        private readonly IStudentsRepository _studentsRepositoryMock;
        private readonly StudentsService _studentService;
        private readonly Faker _faker;

        public StudentsServiceTests()
        {
            _studentsRepositoryMock = Substitute.For<IStudentsRepository>();
            
            _studentService = new StudentsService(_studentsRepositoryMock);
            
            _faker = new Faker();
        }

        [Fact]
        public async Task GetValidWithEventsAsync_ShouldReturnStudent_WhenFound()
        {
            // Arrange
            var studentId = _faker.Random.Int(1, 1000);
            
            var expectedStudent = new Student(
                _faker.Random.Guid().ToString(), 
                _faker.Person.FullName, 
                _faker.Internet.Email()
            );

            _studentsRepositoryMock
                .GetWithEventsAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(expectedStudent);

            // Act
            var result = await _studentService.GetValidWithEventsAsync(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedStudent);
                
            await _studentsRepositoryMock
                .Received(1)
                .GetWithEventsAsync(studentId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetValidWithEventsAsync_ShouldThrowException_WhenStudentIsNull()
        {
            // Arrange
            var studentId = _faker.Random.Int(1, 1000);

            _studentsRepositoryMock
                .GetWithEventsAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Student?)null);

            // Act
            Func<Task> action = async () => await _studentService.GetValidWithEventsAsync(studentId);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("Student not found")
                .Where(ex => ex.Code == "STUDENT_NOT_FOUND"); 

            await _studentsRepositoryMock
                .Received(1)
                .GetWithEventsAsync(studentId, Arg.Any<CancellationToken>());
        }
    }
}