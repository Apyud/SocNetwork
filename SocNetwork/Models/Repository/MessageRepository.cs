using SocNetwork.Models.Db;
//using SocNetwork.Models.DbContext;

namespace SocNetwork.Models.Repository
{
    public class MessageRepository : Repository<Message>
    {
      public MessageRepository(ApplicationDbContext db) : base(db)

        {

        }
       
    }
}
