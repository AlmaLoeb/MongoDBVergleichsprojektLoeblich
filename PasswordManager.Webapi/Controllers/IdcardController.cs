
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
using PasswordManager.Application.Dto;

//part 3+4 genügend 
namespace PasswordManagerApp.Webapi.Controllers
{
    [ApiController]               // Muss bei jedem Controller stehen
    [Route("/api/[controller]")]  // Muss bei jedem Controller stehen damit new route 
    public class IdcardController : ControllerBase
    {
        private readonly ILogger<IdcardController> _logger;

        private readonly PasswordmanagerContext _db;
        private readonly IMapper _mapper;

        public IdcardController(ILogger<IdcardController> logger, PasswordmanagerContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<IdcardForPostDTO>> GetAllIdcards()
        {
            try
            {
                _logger.LogInformation("GetAllIdcards action invoked");
                var passwords = _db.Idcards
                .AsNoTracking()
            .Select(pa => new
            {
                pa.Idcardnr,
                pa.Surname,
                pa.Firstname,
                pa.Expirationdate,
                pa.Birthdate
            })
            .ToList();
                return Ok(passwords);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching idcards");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching idcards");
            }


        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IdcardForPostDTO> GetIdcard(int id)
        {
            try
            {
                _logger.LogInformation("GetIdcard action invoked");
                var passwords = _db.Idcards
                .AsNoTracking()
                .Where(pa => pa.Idcardnr == id)
            .Select(pa => new
            {
                pa.Idcardnr,
                pa.GroupId,
                pa.Surname,
                pa.Firstname,
                pa.Expirationdate,
                pa.Birthdate
            })
          .FirstOrDefault();
                return Ok(passwords);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching idcards");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching idcards");
            }


        }

        // Reagiert auf POST /api/idcard
        [Authorize]
        [HttpPost]
        public IActionResult AddIdcard(IdcardForPostDTO idcardDTO)
        {
            _logger.LogInformation($"AddIdcard method started with input parameter: {idcardDTO}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var password = _mapper.Map<Idcard>(idcardDTO,
                            opt => opt.AfterMap((dto, entity) =>
                            {
                            }));
            if (!_db.Groups.Any(g => g.Id == password.GroupId))
            {
                return BadRequest("Die angegebene Gruppe existiert nicht");
            }
            _db.Idcards.Add(password);
            try { _db.SaveChanges(); }
            catch (ServiceException e)
            { return BadRequest(e.Message); }
            _logger.LogInformation("AddIdcard method finished");
            return Ok(_mapper.Map<Idcard, IdcardForPostDTO>(password));
        }


        /* [HttpPut("{id:int}")]
         public IActionResult EditIdcard(int id, IdcardDTO idcardDTO)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             try
             {
                 _logger.LogInformation("EditIdcard action invoked");
                 _logger.LogInformation($"Received int: {id}");
                 _logger.LogInformation($"Received idcardDTO: {idcardDTO}");

                 if (id != idcardDTO.Idcardnr)
                 {
                     _logger.LogWarning($"Idcardnr mismatch: {id} != {idcardDTO.Idcardnr}");
                     return BadRequest();
                 }
                 var idcard = _db.Idcards.FirstOrDefault(a => a.Idcardnr == id);
                 if (idcard is null)
                 {
                     _logger.LogWarning($"Idcard not found for idcardnr: {id}");
                     return NotFound();
                 }
                 var passwords = _db.Idcards
                 .AsNoTracking()
                 .Where(pa => pa.Idcardnr == id)
                  .Select(pa => new
                  {
                               pa.GroupId,

                  }).FirstOrDefault(); 
                 _mapper.Map(idcardDTO, idcard, opt => opt.AfterMap((src, dest) =>
                 {
                     if (passwords != null)
                     {

                         dest.GroupId = passwords.GroupId; // Use the GroupId value from the first item
                     }
                 }));

                 _logger.LogInformation("Before SaveChanges: Idcard object: {@Idcard}", idcard);
                 _db.SaveChanges();
                 _logger.LogInformation("After SaveChanges");

                 var updatedIdcard = _db.Idcards.FirstOrDefault(a => a.Idcardnr == id);
                 if (updatedIdcard == null)
                 {
                     _logger.LogWarning($"Updated idcard not found for idcardnr: {id}");
                     return NotFound();
                 }

                 return Ok(_mapper.Map<Idcard, IdcardDTO>(updatedIdcard));
             }
             catch (ServiceException e)
             {
                 return BadRequest(e.Message);
             }
         }*/

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult EditIdcard(int id, IdcardForPutDTO idcardDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("EditIdcard action invoked");
                _logger.LogInformation($"Received int: {id}");
                _logger.LogInformation($"Received idcardDTO: {idcardDTO}");

                if (id != idcardDTO.Idcardnr)
                {
                    _logger.LogWarning($"Idcardnr mismatch: {id} != {idcardDTO.Idcardnr}");
                    return BadRequest();
                }
                var idcard = _db.Idcards.FirstOrDefault(a => a.Idcardnr == id);
                if (idcard is null)
                {
                    _logger.LogWarning($"Idcard not found for idcardnr: {id}");
                    return NotFound();
                }

                // Speichern der originalen GroupId
                var originalGroupId = idcard.GroupId;

                // Das Mapping findet hier statt
                _mapper.Map(idcardDTO, idcard);

                // Weise die originale GroupId wieder zu, nachdem das Mapping erfolgt ist
                idcard.GroupId = originalGroupId;

                _logger.LogInformation("Before SaveChanges: Idcard object: {@Idcard}", idcard);
                _db.SaveChanges();
                _logger.LogInformation("After SaveChanges");

                var updatedIdcard = _db.Idcards.FirstOrDefault(a => a.Idcardnr == id);
                if (updatedIdcard == null)
                {
                    _logger.LogWarning($"Updated idcard not found for idcardnr: {id}");
                    return NotFound();
                }

                return Ok(_mapper.Map<Idcard, IdcardForPutDTO>(updatedIdcard));
            }
            catch (ServiceException e)
            {
                return BadRequest(e.Message);
            }
        }




        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deleteidcard(int id)
        {
            _logger.LogInformation($"Deleteidcard method started with input parameter: {id}");
            var idcard = _db.Idcards.FirstOrDefault(a => a.Idcardnr == id);


            if (idcard is null)
            {
                _logger.LogError($"Deleteidcard method failed: Idcard with idcarnd {id} not found");
                return NotFound();
            }

            _db.Idcards.Remove(idcard);
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
            _logger.LogInformation("Deleteidcard method finished");
            return NoContent();
        }


    }
}



