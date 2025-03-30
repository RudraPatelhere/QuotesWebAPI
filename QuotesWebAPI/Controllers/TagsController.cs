using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuotesWebAPI.Models;

namespace QuotesWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly QuotesDbContext _context;

        public TagsController(QuotesDbContext context)
        {
            _context = context;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await _context.Tags.ToListAsync();
        }

        // POST: api/tags
        [HttpPost]
        public async Task<ActionResult<Tag>> AddTag(Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Name))
                return BadRequest("Tag name is required");

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
        }

        // GET: api/tags/quotes/funny
        [HttpGet("quotes/{tagName}")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotesByTag(string tagName)
        {
            var tag = await _context.Tags
                .Include(t => t.TagAssignments)
                .ThenInclude(ta => ta.Quote)
                .ThenInclude(q => q.TagAssignments)
                .ThenInclude(ta => ta.Tag)
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

            if (tag == null)
                return NotFound("Tag not found");

            var quotes = tag.TagAssignments.Select(ta => ta.Quote).ToList();
            return Ok(quotes);
        }
    }
}
