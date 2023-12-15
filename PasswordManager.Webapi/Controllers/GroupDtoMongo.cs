using static PasswordManager.Mongo.PasswordManagerMongoContext;
using System.Collections.Generic;

namespace PasswordManager.Webapi.Controllers
{
    public class GroupDtoMongo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<BankcardDocument>? Bankcards { get; set; }
    }
}
