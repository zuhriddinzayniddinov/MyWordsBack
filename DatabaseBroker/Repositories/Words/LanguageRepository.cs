using Entity.Models.Words;
using Microsoft.EntityFrameworkCore;

namespace DatabaseBroker.Repositories.Words;

public class LanguageRepository(DbContext dbContext) : RepositoryBase<Language, long>(dbContext), ILanguageRepository;