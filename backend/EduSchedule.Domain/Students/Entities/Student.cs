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
        public string? EventsDeltaToken { get; protected set; }
        public bool IsActive { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

        protected Student() { }

        public Student(string externalId, string displayName, string email)
        {
            ExternalId = externalId;
            DisplayName = displayName;
            Email = email;
            IsActive = true;
            CreatedAt = DateTime.Now;
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

        public void Update(string displayName, string email)
        {
            DisplayName = displayName;
            Email = email;
            UpdatedAt = DateTime.Now;
            Validate();
        }

        public void UpdateEventsDeltaToken(string eventsDeltaToken)
        {
            EventsDeltaToken = eventsDeltaToken;
        }

        public void Inactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.Now;
        }
    }
}
