using DatabaseBroker.DataContext;
using Entity.Models.Words;

namespace DatabaseBroker.Repositories.Words;

public class WordRepository(ProgramDataContext dbContext) : RepositoryBase<Word, long>(dbContext), IWordRepository;