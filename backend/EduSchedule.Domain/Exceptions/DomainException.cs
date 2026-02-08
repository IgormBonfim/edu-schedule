namespace EduSchedule.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Code { get; set; }
        public DomainException(string message, string code) : base(message) 
        {
            Code = code;
        }
    }
}
