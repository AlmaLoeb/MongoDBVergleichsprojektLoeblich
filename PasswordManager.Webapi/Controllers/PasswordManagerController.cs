
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerApp.Applcation.Infastruture;
using PasswordmanagerApp.Application.Dto;
using PasswordmanagerApp.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Internal;
using PasswordManager.Application.Services;

//part 3+4 genügend 
namespace PasswordManagerApp.Webapi.Controllers
{
    [ApiController]               // Muss bei jedem Controller stehen
    [Route("/api/[controller]")]  // Muss bei jedem Controller stehen damit new route 
    public class PasswordController : ControllerBase
    {
        private readonly ILogger<PasswordController> _logger;

        private readonly PasswordmanagerContext _db;
        private readonly IMapper _mapper;

        public PasswordController(ILogger<PasswordController> logger, PasswordmanagerContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }
        // Reagiert auf GET /api/password
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<PasswordForPostDTO>> GetAllPassword()
        {
            try
            {
                _logger.LogInformation("GetAllPassword action invoked");
                var passwords = _db.Passwords
                .AsNoTracking()
            .Select(pa => new
            {
                pa.Guid,
                pa.Id,
                pa.WebsiteUrl,
                pa.Accountname,
                pa.Passworde,
                PasswordPoliciesGuid = pa.PasswordPolicies.Guid,
                length = pa.PasswordPolicies.Length,
                safeness = pa.PasswordPolicies.Safeness.ToString(),
            })
            .ToList();
                return Ok(passwords);
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching passwords");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching passwords");
            }


        }

        [Authorize]
        [HttpGet("{guid:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<PasswordForPostDTO> GetPassword(Guid guid)
        {
            try
            {
                _logger.LogInformation("GetPassword action invoked");
                var passwords = _db.Passwords
                .AsNoTracking()
                .Where(pa => pa.Guid == guid)
            .Select(pa => new
            {
                pa.Guid,
                pa.GroupId,
                pa.WebsiteUrl,
                pa.Accountname,
                pa.Passworde,
                PasswordPoliciesGuid = pa.PasswordPolicies.Guid,
                length = pa.PasswordPolicies.Length,
                safeness = pa.PasswordPolicies.Safeness.ToString(),
            })
          .FirstOrDefault();
                return Ok(passwords);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching passwords");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching passwords");
            }


        }

        // Reagiert auf POST /api/password
        [Authorize]
         [HttpPost]
         public IActionResult AddPassword(PasswordForPostDTO passwordDto)
         {
             _logger.LogInformation($"AddPassword method started with input parameter: {passwordDto}");
             var passwordPolicy = _db.PasswordPolicies.First(a => a.Guid == passwordDto.PasswordPoliciesGuid);

             var password = _mapper.Map<Password>(passwordDto,
                             opt => opt.AfterMap((dto, entity) =>
                             {
                                 entity.PasswordPolicies = passwordPolicy;
                                 entity.Guid = Guid.NewGuid(); // Add this line to generate a new Guid for the Password entity
                             }));
             if (!_db.Groups.Any(g => g.Id == password.GroupId))
             {
                 return BadRequest("Die angegebene Gruppe existiert nicht");
             }
             _db.Passwords.Add(password);
             try { _db.SaveChanges(); }


             catch (ServiceException e)
             { return BadRequest(e.Message); }
             _logger.LogInformation("AddPassword method finished");
             return Ok(_mapper.Map<Password, PasswordForPostDTO>(password));
         }
      /*  [HttpPost]
        public IActionResult AddPassword(PasswordDto passwordDto)
        {
            _logger.LogInformation($"AddPassword method started with input parameter: {passwordDto}");

            Strongpwd passwordStrength = PasswordHelper.CalculateSafeness(passwordDto.Passworde);
            var passwordPolicy = new PasswordPolicies // Create a new PasswordPolicies object
            {
                Guid = Guid.NewGuid(),
                Length = passwordDto.Passworde.Length,
                Safeness = safeness.ToString()
            };
            var password = new Password(
                passwordDto.WebsiteUrl,
                passwordDto.Accountname,
                passwordDto.Passworde,
                passwordPolicy
            )
            {
                Guid = Guid.NewGuid(),
                GroupId = passwordDto.GroupId
            };

            if (!_db.Groups.Any(g => g.Id == password.GroupId))
            {
                // Gruppe existiert nicht, geben Sie einen Fehler zurück oder fügen Sie eine neue Gruppe hinzu
                return BadRequest("Die angegebene Gruppe existiert nicht");
            }
            if (password.PasswordPolicies == null)
            {
                return BadRequest("Das passwordPolicies-Feld ist erforderlich.");
            }
            _db.Passwords.Add(password);
            try { _db.SaveChanges(); }

            catch (ServiceException e)
            { return BadRequest(e.Message); }
            _logger.LogInformation("AddPassword method finished");
            return Ok(_mapper.Map<Password, PasswordDto>(password));
        }*/


        [Authorize]
        [HttpPut("{guid:Guid}")]
        public IActionResult Editpassword(Guid guid, PasswordForPutDTO passwordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("EditPassword action invoked");
                _logger.LogInformation($"Received guid: {guid}");
                _logger.LogInformation($"Received passwordDto: {passwordDto}");

                if (guid != passwordDto.Guid)
                {
                    _logger.LogWarning($"Guid mismatch: {guid} != {passwordDto.Guid}");
                    return BadRequest();
                }
                var password = _db.Passwords.FirstOrDefault(a => a.Guid == guid);
                if (password is null)
                {
                    _logger.LogWarning($"Password not found for guid: {guid}");
                    return NotFound();
                }
                _mapper.Map(passwordDto, password,
                opt => opt.AfterMap((dto, entity) =>
                {
                    entity.PasswordPolicies = _db.PasswordPolicies.First(a => a.Guid == passwordDto.PasswordPoliciesGuid);
                }));

                _logger.LogInformation("Before SaveChanges: Password object: {@Password}", password);
                _db.SaveChanges();
                _logger.LogInformation("After SaveChanges");

                var updatedPassword = _db.Passwords.FirstOrDefault(a => a.Guid == guid);
                if (updatedPassword == null)
                {
                    _logger.LogWarning($"Updated password not found for guid: {guid}");
                    return NotFound();
                }

                // Get the updated password policy from database
                var passwordPolicy = _db.PasswordPolicies.First(a => a.Guid == updatedPassword.PasswordPolicies.Guid);

                return Ok(new
                {
                    Password = _mapper.Map<Password, PasswordForPutDTO>(updatedPassword),
                    Length = passwordPolicy.Length,
                    Safeness = passwordPolicy.Safeness.ToString()
                });


            }
            catch (ServiceException e)
            { return BadRequest(e.Message); }
            //catch (DbUpdateException ex)
            //{
            //    _logger.LogError($"Error occured while wanting to update password:  {ex.Message}");
            //    return BadRequest();
            //}
        }

        [Authorize]
        [HttpDelete("{guid:Guid}")]
        public IActionResult Deletepassword(Guid guid)
        {
            _logger.LogInformation($"Deletepassword method started with input parameter: {guid}");
            var password = _db.Passwords.FirstOrDefault(a => a.Guid == guid);

            if (password is null)
            {
                _logger.LogError($"Deletepassword method failed: Password with guid {guid} not found");
                 return NotFound(); 
            }


            _db.Passwords.Remove(password);
            try { _db.SaveChanges(); }

            catch (ServiceException e) when (e.NotFound == true)
            { return NotFound(); }
            catch (ServiceException e) when (e.Forbidden == true)
            { return StatusCode(403); }
            catch (ServiceException e)
            { return BadRequest(e.Message); }
            //catch (DbUpdateException ex) { 
            //    _logger.LogError($"Deletepassword method failed with error: {ex.Message}"); 
            //    return BadRequest(); }
            _logger.LogInformation("Deletepassword method finished");
            return NoContent();
        }


    }
}



