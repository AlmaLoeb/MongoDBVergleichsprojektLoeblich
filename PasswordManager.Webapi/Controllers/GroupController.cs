using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using PasswordManager.Mongo;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PasswordManager.Mongo.PasswordManagerMongoContext;

namespace PasswordManager.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly PasswordManagerMongoContext _context;

        public GroupController(PasswordManagerMongoContext context)
        {
            _context = context;
        }

        // GET: api/group
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDocument>>> GetGroups()
        {
            var groups = await _context.Groups.Find(_ => true).ToListAsync();
            return Ok(groups);
        }

        // GET: api/group/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<GroupDocument>> GetGroup(string id)
        {
            var group = await _context.Groups.Find(g => g.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            return group;
        }

        // POST: api/group
        [HttpPost]
        public async Task<ActionResult<GroupDocument>> CreateGroup(GroupDocument group)
        {
            await _context.Groups.InsertOneAsync(group);
            return CreatedAtAction(nameof(GetGroup), new { id = group.Id.ToString() }, group);
        }

        // PUT: api/group/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateGroup(string id, GroupDocument updatedGroup)
        {
            var group = await _context.Groups.Find(g => g.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            await _context.Groups.ReplaceOneAsync(g => g.Id == new ObjectId(id), updatedGroup);
            return NoContent();
        }

        // DELETE: api/group/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var deleteResult = await _context.Groups.DeleteOneAsync(g => g.Id == new ObjectId(id));
            if (deleteResult.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
