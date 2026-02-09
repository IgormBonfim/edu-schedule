namespace EduSchedule.Domain.States.Entities
{
    public class SyncState
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string DeltaToken { get; set; }
        public DateTime LastSync { get; set; }

        public SyncState(string entityName, string deltaToken)
        {
            EntityName = entityName;
            DeltaToken = deltaToken;
            LastSync = DateTime.Now;
        }

        public void UpdateDeltaToken(string deltaToken)
        {
            DeltaToken = deltaToken;
            LastSync = DateTime.Now;
        }
    }
}
