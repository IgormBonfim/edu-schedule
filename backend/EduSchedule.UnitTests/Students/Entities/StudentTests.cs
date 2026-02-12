using Bogus;
using EduSchedule.Domain.Exceptions;
using EduSchedule.Domain.Students.Entities;
using FluentAssertions;
using Xunit;

namespace EduSchedule.UnitTests.Domain.Students.Entities
{
    public class StudentTests
    {
        private readonly Faker _faker = new Faker();

        [Fact]
        public void Constructor_ShouldCreateValidActiveStudent()
        {
            // Arrange
            var externalId = _faker.Random.Guid().ToString();
            var name = _faker.Person.FullName;
            var email = _faker.Internet.Email();

            // Act
            var student = new Student(externalId, name, email);

            // Assert
            student.ExternalId.Should().Be(externalId);
            student.DisplayName.Should().Be(name);
            student.Email.Should().Be(email);
            student.IsActive.Should().BeTrue();
            student.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            student.Events.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "John", "john@email.com", "EXTERNAL_ID_REQUIRED")]
        [InlineData("123", "", "john@email.com", "DISPLAY_NAME_REQUIRED")]
        [InlineData("123", "John", "", "EMAIL_REQUIRED")]
        public void Validate_ShouldThrow_WhenFieldsAreEmpty(string extId, string name, string email, string errorCode)
        {
            // Arrange
            var student = new Student(extId, name, email);

            // Act
            Action action = () => student.Validate();

            // Assert
            action.Should()
                .Throw<DomainException>()
                .Where(ex => ex.Code == errorCode); 
        }

        [Fact]
        public void Validate_ShouldThrow_WhenFieldsExceedMaxLength()
        {
            // Arrange
            var hugeId = _faker.Random.String2(101);
            var hugeName = _faker.Random.String2(251);
            var hugeEmail = _faker.Random.String2(151);

            var student1 = new Student(hugeId, "Valid", "valid@email.com");
            var student2 = new Student("Valid", hugeName, "valid@email.com");
            var student3 = new Student("Valid", "Valid", hugeEmail);

            // Act & Assert
            Action action1 = () => student1.Validate();
            action1.Should().Throw<DomainException>().WithMessage("*100 characters*");

            Action action2 = () => student2.Validate();
            action2.Should().Throw<DomainException>().WithMessage("*250 characters*");

            Action action3 = () => student3.Validate();
            action3.Should().Throw<DomainException>().WithMessage("*150 characters*");
        }

        [Theory]
        [InlineData("usuario.com")]
        [InlineData("usuario@com")]
        [InlineData("@dominio.com")]
        [InlineData("usuario@.com")]
        public void Validate_ShouldThrow_WhenEmailFormatIsInvalid(string invalidEmail)
        {
            // Arrange
            var student = new Student("123", "Nome", invalidEmail);

            // Act
            Action action = () => student.Validate();

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("*not in a valid format*");
        }

        [Fact]
        public async Task Update_ShouldUpdateValues_AndTimestamp()
        {
            // Arrange
            var student = new Student("123", "Nome Antigo", "old@email.com");
            var dataCriacao = student.CreatedAt;
            
            var novoNome = _faker.Person.FullName;
            var novoEmail = _faker.Internet.Email();

            await Task.Delay(10);

            // Act
            student.Update(novoNome, novoEmail);

            // Assert
            student.DisplayName.Should().Be(novoNome);
            student.Email.Should().Be(novoEmail);
            student.UpdatedAt.Should().NotBeNull();
            student.UpdatedAt.Should().BeAfter(dataCriacao);
        }

        [Fact]
        public void Update_ShouldThrow_WhenNewDataIsInvalid()
        {
            // Arrange
            var student = new Student("123", "Nome Válido", "email@valido.com");

            // Act - Tentando atualizar com e-mail inválido
            Action action = () => student.Update("Novo Nome", "email-invalido");

            // Assert - Deve chamar o Validate() internamente e estourar o erro
            action.Should().Throw<DomainException>().WithMessage("*not in a valid format*");
        }

        [Fact]
        public async Task Inactivate_ShouldSetIsActiveFalse()
        {
            // Arrange
            var student = new Student("123", "Nome", "email@teste.com");
            await Task.Delay(10);

            // Act
            student.Inactivate();

            // Assert
            student.IsActive.Should().BeFalse();
            student.UpdatedAt.Should().NotBeNull();
            student.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdateEventsDeltaToken_ShouldUpdateProperty()
        {
            // Arrange
            var student = new Student("123", "Nome", "email@teste.com");
            var token = _faker.Random.AlphaNumeric(20);

            // Act
            student.UpdateEventsDeltaToken(token);

            // Assert
            student.EventsDeltaToken.Should().Be(token);
        }
    }
}