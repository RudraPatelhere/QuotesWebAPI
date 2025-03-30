namespace QuotesWebAPI.Models
{
    public class TagAssignment
    {
        public int Id { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
