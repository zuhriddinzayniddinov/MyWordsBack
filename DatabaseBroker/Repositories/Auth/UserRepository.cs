using DatabaseBroker.DataContext;
using Entity.Models;
using Entity.Models.Auth;

namespace DatabaseBroker.Repositories.Auth;

public class UserRepository : RepositoryBase<User, long>, IUserRepository
{
    public UserRepository(ProgramDataContext dbContext) : base(dbContext)
    {
    }
}