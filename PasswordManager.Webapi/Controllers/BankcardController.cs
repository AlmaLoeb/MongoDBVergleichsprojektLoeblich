using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PasswordmanagerApp.Application.Dto;
using PasswordManagerApp.Applcation.Infastruture;
using PasswordmanagerApp.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PasswordManagerApp.Webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BankcardController : ControllerBase
    {
        private readonly ILogger<BankcardController> _logger;
        private readonly PasswordmanagerContext _db;
        private readonly IMapper _mapper;

        public BankcardController(ILogger<BankcardController> logger, PasswordmanagerContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<BankcardDto>> GetAllBankcards()
        {
            var bankcards = _db.Bankcards.AsNoTracking().Select(b => _mapper.Map<BankcardDto>(b)).ToList();
            return Ok(bankcards);
        }

        [Authorize]
        [HttpGet("{iban:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<BankcardDto> GetBankcard(int iban)
        {
            var bankcard = _db.Bankcards.AsNoTracking().FirstOrDefault(b => b.Iban == iban);
            if (bankcard == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BankcardDto>(bankcard));
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddBankcard(BankcardDto bankcardDto)
        {
            var bankcard = _mapper.Map<Bankcard>(bankcardDto);
            _db.Bankcards.Add(bankcard);
            _db.SaveChanges();

            return Ok(_mapper.Map<BankcardDto>(bankcard));
        }

        [Authorize]
        [HttpPut("{iban:int}")]
        public IActionResult EditBankcard(int iban, BankcardDto bankcardDto)
        {
            var bankcard = _db.Bankcards.FirstOrDefault(b => b.Iban == iban);
            if (bankcard == null)
            {
                return NotFound();
            }

            _mapper.Map(bankcardDto, bankcard);
            _db.SaveChanges();

            return Ok(_mapper.Map<BankcardDto>(bankcard));
        }

        [Authorize]
        [HttpDelete("{iban:int}")]
        public IActionResult DeleteBankcard(int iban)
        {
            var bankcard = _db.Bankcards.FirstOrDefault(b => b.Iban == iban);
            if (bankcard == null)
            {
                return NotFound();
            }

            _db.Bankcards.Remove(bankcard);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
