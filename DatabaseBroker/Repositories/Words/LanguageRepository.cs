using DatabaseBroker.DataContext;
using Entity.Models.Words;

namespace DatabaseBroker.Repositories.Words;

public class LanguageRepository(ProgramDataContext dbContext) : RepositoryBase<Language, long>(dbContext), ILanguageRepository;