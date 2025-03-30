using System.ComponentModel.DataAnnotations;

namespace QuotesWebAPI.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public string? Text { get; set; }

        public string? Author { get; set; }

        public int Likes { get; set; } = 0;

        public ICollection<TagAssignment> TagAssignments { get; set; } = new List<TagAssignment>();
    }
}
