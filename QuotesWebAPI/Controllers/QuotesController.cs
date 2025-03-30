using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuotesWebAPI.Models;

namespace QuotesWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly QuotesDbContext _context;

        public QuotesController(QuotesDbContext context)
        {
            _context = context;
        }

        // GET: api/quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotes()
        {
            return await _context.Quotes
                .Include(q => q.TagAssignments)
                .ThenInclude(ta => ta.Tag)
                .ToListAsync();
        }

        // GET: api/quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.TagAssignments)
                .ThenInclude(ta => ta.Tag)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            return quote;
        }

        // POST: api/quotes
        [HttpPost]
        public async Task<ActionResult<Quote>> PostQuote(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quote);
        }

        // PUT: api/quotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuote(int id, Quote updatedQuote)
        {
            if (id != updatedQuote.Id)
                return BadRequest();

            _context.Entry(updatedQuote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Quotes.Any(q => q.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/quotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
                return NotFound();

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/quotes/5/like
        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikeQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
                return NotFound();

            quote.Likes++;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/quotes/top?count=10
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetTopLikedQuotes([FromQuery] int count = 10)
        {
            return await _context.Quotes
                .OrderByDescending(q => q.Likes)
                .Take(count)
                .ToListAsync();
        }

        // ✅ NEW: GET all tags
        [HttpGet("/api/tags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await _context.Tags.ToListAsync();
        }

        // ✅ NEW: Assign a tag to a quote
        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AssignTagToQuote(int id, [FromBody] string tagName)
        {
            var quote = await _context.Quotes
                .Include(q => q.TagAssignments)
                .ThenInclude(ta => ta.Tag)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            if (quote.TagAssignments.Any(ta => ta.TagId == tag.Id))
                return BadRequest("Tag already assigned.");

            _context.TagAssignments.Add(new TagAssignment
            {
                QuoteId = quote.Id,
                TagId = tag.Id
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ✅ NEW: Get quotes by tag name
        [HttpGet("tag/{tagName}")]
        public async Task<ActionResult<IEnumerable<Quote>>> GetQuotesByTag(string tagName)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
                return NotFound("Tag not found.");

            var quotes = await _context.Quotes
                .Include(q => q.TagAssignments)
                .ThenInclude(ta => ta.Tag)
                .Where(q => q.TagAssignments.Any(ta => ta.TagId == tag.Id))
                .ToListAsync();

            return quotes;
        }
    }
}
