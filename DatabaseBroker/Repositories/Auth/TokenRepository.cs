﻿using DatabaseBroker.DataContext;
using Entity.Models;
using Entity.Models.Auth;

namespace DatabaseBroker.Repositories.Auth;

public class TokenRepository(ProgramDataContext dbContext)
    : RepositoryBase<TokenModel, long>(dbContext), ITokenRepository;