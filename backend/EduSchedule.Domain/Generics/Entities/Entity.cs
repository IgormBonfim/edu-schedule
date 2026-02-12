namespace EduSchedule.Domain.Generics.Entities
{
    public class Entity
    {
        public bool IsActive { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected Entity() {}
    }
}