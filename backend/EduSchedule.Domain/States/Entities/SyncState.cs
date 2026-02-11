namespace EduSchedule.Domain.States.Entities
{
    public class SyncState
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string NextLink { get; set; }
        public DateTime LastSync { get; set; }

        public SyncState(string entityName, string nextLink)
        {
            EntityName = entityName;
            NextLink = nextLink;
            LastSync = DateTime.Now;
        }

        public void UpdateNextLink(string nextLink)
        {
            NextLink = nextLink;
            LastSync = DateTime.Now;
        }
    }
}
