namespace QuotesWebAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<TagAssignment> TagAssignments { get; set; } = new List<TagAssignment>();
    }
}
