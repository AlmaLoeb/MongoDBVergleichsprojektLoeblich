using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Mongo;
using System.Threading.Tasks;
using static PasswordManager.Mongo.PasswordManagerMongoContext;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using PasswordmanagerApp.Application.Dto;
using PasswordmanagerApp.Application.Model;

namespace PasswordManager.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MongoController : ControllerBase
    {
        private readonly PasswordManagerMongoContext _context;

        public MongoController(PasswordManagerMongoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDtoMongo>>> GetGroups()
        {
            var groups = await _context.Groups.Find(_ => true).ToListAsync();
            var groupDtos = groups.Select(g => new GroupDtoMongo
            {
                Id = g.Id.ToString(),
                Name = g.Name,
                Bankcards = g.Bankcards
            }).ToList();

            return groupDtos;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<GroupDtoMongo>> GetGroup(string id)
        {
            var group = await _context.Groups.Find(g => g.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }

            var groupDto = new GroupDtoMongo 
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Bankcards = group.Bankcards
            };

            return groupDto;
        }


        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<GroupDocument>> CreateGroup(GroupDocument group)
        {
            await _context.Groups.InsertOneAsync(group);
            return CreatedAtAction(nameof(GetGroup), new { id = group.Id.ToString() }, group);
        }

        // GET: api/groups/{groupId}/bankcards
        [HttpGet("{groupId:length(24)}/bankcards")]
        public async Task<ActionResult<IEnumerable<BankcardDtoMongo>>> GetBankcardsByGroup(string groupId)
        {
            var group = await _context.Groups.Find(g => g.Id == new ObjectId(groupId)).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            // Map your bankcard documents to the DTO
            var bankcardDTOs = group.Bankcards.Select(bankcard => new BankcardDtoMongo
            {
                Id = bankcard.Id.ToString(),
                Iban = bankcard.Iban,
                Firstname = bankcard.Firstname,
                Surname = bankcard.Surname,
                ExpirationDate = bankcard.ExpirationDate
            });

            return bankcardDTOs.ToList();
        }


        // PUT: api/groups/{id}
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

        // DELETE: api/groups/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var group = await _context.Groups.Find(g => g.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            await _context.Groups.DeleteOneAsync(g => g.Id == new ObjectId(id));
            return NoContent();
        }
        [HttpPut("{groupId:length(24)}/bankcards/{bankcardId:length(24)}")]
        public async Task<IActionResult> UpdateBankcard(string groupId, string bankcardId, [FromBody] BankcardDtoMongo bankcardDto)
        {
            // You need to convert the string IDs back to ObjectId
            if (!ObjectId.TryParse(groupId, out var groupObjectId))
            {
                return BadRequest("Invalid Group ID");
            }

            if (!ObjectId.TryParse(bankcardId, out var bankcardObjectId))
            {
                return BadRequest("Invalid Bankcard ID");
            }

            // Construct a filter to find the group and bankcard to update
            var filter = Builders<GroupDocument>.Filter.And(
                Builders<GroupDocument>.Filter.Eq(g => g.Id, groupObjectId),
                Builders<GroupDocument>.Filter.ElemMatch(g => g.Bankcards, b => b.Id == bankcardObjectId)
            );

            // Construct an update definition for the fields you want to update
            var update = Builders<GroupDocument>.Update
                .Set("Bankcards.$.Iban", bankcardDto.Iban)
                .Set("Bankcards.$.Firstname", bankcardDto.Firstname)
                .Set("Bankcards.$.Surname", bankcardDto.Surname)
                .Set("Bankcards.$.ExpirationDate", bankcardDto.ExpirationDate);

            // Use the positional operator ($) to update the matched bankcard within the array
            var result = await _context.Groups.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound("Group or Bankcard not found.");
            }

            return NoContent();
        }

        [HttpPost("{groupId:length(24)}/bankcards")]
        public async Task<ActionResult<BankcardDtoMongo>> CreateBankcard(string groupId, BankcardDtoMongo bankcardDto)
        {
            if (!ObjectId.TryParse(groupId, out var groupObjectId))
            {
                return BadRequest("Invalid Group ID");
            }

            var group = _context.GetGroupById(groupObjectId);
            if (group == null)
            {
                return NotFound("Group not found.");
            }

            var bankcardDocument = new BankcardDocument(
                Iban: bankcardDto.Iban?? "default",
                Firstname: bankcardDto.Firstname?? "default",
                Surname: bankcardDto.Surname ?? "default", 
                ExpirationDate: bankcardDto.ExpirationDate
            );

            await Task.Run(() => _context.AddBankcardToGroup(groupObjectId, bankcardDocument));

            var bankcardDtoResult = new BankcardDtoMongo
            {
                Id = bankcardDocument.Id.ToString(),
                Iban = bankcardDocument.Iban,
                Firstname = bankcardDocument.Firstname,
                Surname = bankcardDocument.Surname,
                ExpirationDate = bankcardDocument.ExpirationDate
            };

            return CreatedAtAction(nameof(GetBankcardsByGroup), new { groupId }, bankcardDtoResult);
        }




        // DELETE: api/mongo/{groupId}/bankcards/{bankcardId}
        [HttpDelete("{groupId:length(24)}/bankcards/{bankcardId:length(24)}")]
        public async Task<IActionResult> DeleteBankcard(string groupId, string bankcardId)
        {
            var update = Builders<GroupDocument>.Update.PullFilter(g => g.Bankcards, b => b.Id == new ObjectId(bankcardId));
            var result = await _context.Groups.UpdateOneAsync(
                g => g.Id == new ObjectId(groupId),
                update
            );

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
