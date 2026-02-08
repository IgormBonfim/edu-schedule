using EduSchedule.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace EduSchedule.Domain.Students.Entities
{
    public class Student
    {
        public int Id { get; protected set; }
        public string ExternalId { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Email { get; protected set; }

        protected Student() { }

        public Student(string externalId, string displayName, string email)
        {
            ExternalId = externalId;
            DisplayName = displayName;
            Email = email;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ExternalId))
                throw new DomainException("The ExternalId is required.", "EXTERNAL_ID_REQUIRED");

            if (ExternalId.Length > 100)
                throw new DomainException("The ExternalId cannot exceed 100 characters.", "EXTERNAL_ID_TOO_LONG");

            if (string.IsNullOrWhiteSpace(DisplayName))
                throw new DomainException("The DisplayName is required.", "DISPLAY_NAME_REQUIRED");

            if (DisplayName.Length > 250)
                throw new DomainException("The DisplayName cannot exceed 250 characters.", "DISPLAY_NAME_TOO_LONG");

            if (string.IsNullOrWhiteSpace(Email))
                throw new DomainException("The Email is required.", "EMAIL_REQUIRED");

            if (Email.Length > 150)
                throw new DomainException("The Email cannot exceed 150 characters.", "EMAIL_TOO_LONG");

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new DomainException("The Email provided is not in a valid format.", "EMAIL_INVALID_FORMAT");
        }
    }
}
