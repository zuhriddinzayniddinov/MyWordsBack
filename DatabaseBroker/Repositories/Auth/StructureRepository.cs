using DatabaseBroker.DataContext;
using Entity.Models;

namespace DatabaseBroker.Repositories.Auth;

public class StructureRepository(ProgramDataContext dbContext)
    : RepositoryBase<Structure, long>(dbContext), IStructureRepository;