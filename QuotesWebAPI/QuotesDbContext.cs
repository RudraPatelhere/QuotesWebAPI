using Microsoft.EntityFrameworkCore;
using QuotesWebAPI.Models;

namespace QuotesWebAPI
{
    public class QuotesDbContext : DbContext
    {
        public QuotesDbContext(DbContextOptions<QuotesDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagAssignment> TagAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TagAssignment>()
                .HasOne(t => t.Quote)
                .WithMany(q => q.TagAssignments)
                .HasForeignKey(t => t.QuoteId);

            modelBuilder.Entity<TagAssignment>()
                .HasOne(t => t.Tag)
                .WithMany(q => q.TagAssignments)
                .HasForeignKey(t => t.TagId);
        }
    }
}
