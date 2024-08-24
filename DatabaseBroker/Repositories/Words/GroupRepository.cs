using Entity.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace DatabaseBroker.Repositories.Words;

public class GroupRepository(DbContext dbContext) : RepositoryBase<Group, long>(dbContext), IGroupRepository;