using System;

namespace PasswordManager.Webapi.Controllers
{
    public class BankcardDtoMongo
    {
        public string? Id { get; set; }
        public string? Iban { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
