namespace EduSchedule.Domain.Students.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        protected Event() { }

        public Event(string externalId, string subject, DateTime start, DateTime end, int studentId)
        {
            ExternalId = externalId;
            Subject = subject;
            StartTime = start;
            EndTime = end;
            StudentId = studentId;
        }
    }
}
