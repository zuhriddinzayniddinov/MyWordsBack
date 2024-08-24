using DatabaseBroker.DataContext;
using Entity.Models;

namespace DatabaseBroker.Repositories.Auth;

public class PermissionRepository(ProgramDataContext dbContext)
    : RepositoryBase<Permission, long>(dbContext), IPermissionRepository;
