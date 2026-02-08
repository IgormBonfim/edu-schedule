namespace EduSchedule.Domain.States.Entities
{
    public class SyncState
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string DeltaLink { get; set; }
        public DateTime LastSync { get; set; }
    }
}
