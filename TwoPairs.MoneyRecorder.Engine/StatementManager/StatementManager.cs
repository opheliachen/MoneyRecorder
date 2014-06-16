using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using TwoPairs.MoneyRecorder.Data;
using TwoPairs.MoneyRecorder.Engine.Data;
using TwoPairs.MoneyRecorder.Exceptions;

namespace TwoPairs.MoneyRecorder
{
    public class StatementManager
    {
        private readonly Func<Repository> _repositoryFactory;

        public StatementManager(Func<Repository> repositoryFactory)
        {
            Contract.Requires(repositoryFactory != null);
            _repositoryFactory = repositoryFactory;
        }

        public void CreateStatement(CreateStatementData statementData)
        {
            Contract.Requires(statementData != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(statementData.Name));
            Contract.Requires(statementData.CurrencyId != Guid.Empty);

            using (var repository = _repositoryFactory())
            {
                var currency = repository.Currencies.SingleOrDefault(c => c.Id == statementData.CurrencyId);
                if (currency == null)
                {
                    throw new CurrencyNotFoundException(statementData.CurrencyId.ToString());
                }

                var statement = new Statement
                {
                    Id = Guid.NewGuid(),
                    Name = statementData.Name,
                    CurrencyId = statementData.CurrencyId,
                    Currency = currency,
                    CreatedBy = statementData.CreatedBy,
                    CreatedOn = DateTimeOffset.Now
                };
                repository.Statements.Attach(statement);

                repository.Statements.Add(statement);
                repository.SaveChanges();
            }
        }

        public void UpdateStatement(UpdateStatementData statementData)
        {
            Contract.Requires(statementData != null);
            Contract.Requires(!Guid.Empty.Equals(statementData.Id));
            Contract.Requires(!string.IsNullOrWhiteSpace(statementData.Name));
            Contract.Requires(statementData.CurrencyId != Guid.Empty);

            using (var repository = _repositoryFactory())
            {
                var statement = repository.Statements.SingleOrDefault(u => u.Id == statementData.Id);

                if (statement == null)
                {
                    throw new StatementNotFoundException(statementData.Name);
                }

                var currency = repository.Currencies.SingleOrDefault(c => c.Id == statementData.CurrencyId);
                if (currency == null)
                {
                    throw new CurrencyNotFoundException(statementData.CurrencyId.ToString());
                }

                statement.Name = statementData.Name;
                statement.CurrencyId = statementData.CurrencyId;
                statement.Currency = currency;
                statement.UpdatedBy = statementData.UpdatedBy;
                statement.UpdatedOn = DateTimeOffset.Now;

                repository.Statements.Attach(statement);

                repository.SetEntityState(statement, EntityState.Modified);
                repository.SaveChanges();
            }
        }

        public void DeleteStatement(Guid id)
        {
            Contract.Requires(!Guid.Empty.Equals(id));

            using (var repository = _repositoryFactory())
            {
                var statement = repository.Statements.SingleOrDefault(u => u.Id == id);

                if (statement == null)
                {
                    throw new StatementNotFoundException(id.ToString());
                }

                repository.Statements.Remove(statement);
                repository.SaveChanges();
            }
        }

        public GetStatementData GetStatement(Guid id)
        {
            Contract.Requires(!Guid.Empty.Equals(id));

            using (var repository = _repositoryFactory())
            {
                var statement = repository.Statements.Include(c => c.Currency).SingleOrDefault(u => u.Id == id);

                if (statement == null)
                {
                    throw new StatementNotFoundException(id.ToString());
                }

                return new GetStatementData
                {
                    Id = statement.Id,
                    Name = statement.Name,
                    Currency = statement.Currency
                };
            }
        }

        public GetStatementData[] GetStatements(string keyword = "", Sort<StatementSortBy> sorting = null, Paging paging = null)
        {
            using (var repository = _repositoryFactory())
            {
                var currencies = repository.Statements.Include(c => c.Currency)
                                                      .Join(repository.Users, c => c.CreatedBy, u => u.Id, (g, c) => new { Statement = g, User = c })
                                                      .Join(repository.Users, g => g.Statement.UpdatedBy, u => u.Id, (g, u) => new GetStatementData
                                                      {
                                                          Id = g.Statement.Id,
                                                          Name = g.Statement.Name,
                                                          Currency = g.Statement.Currency,
                                                          CreatedBy = g.User.Name,
                                                          CreatedOn = g.Statement.CreatedOn,
                                                          UpdatedOn = g.Statement.UpdatedOn,
                                                          UpdatedBy = u.Name
                                                      });

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    currencies = currencies.Where(u => u.Name.Contains(keyword));
                }

                if (sorting != null)
                {
                    currencies = SortGroup(sorting, currencies);
                }

                if (paging != null)
                {
                    currencies = currencies.Skip((paging.CurrentPage - 1) * paging.PageSize).Take(paging.PageSize);
                }

                return currencies.ToArray();
            }
        }

        public int GetStatementCount(string keyword)
        {
            using (var repository = _repositoryFactory())
            {
                var currencies = repository.Statements.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    currencies = currencies.Where(u => u.Name.Contains(keyword));
                }

                return currencies.Count();
            }
        }

        private static IQueryable<GetStatementData> SortGroup(Sort<StatementSortBy> sorting, IQueryable<GetStatementData> currencies)
        {
            switch (sorting.By)
            {
                case StatementSortBy.Name:
                    currencies = currencies.Sort(u => u.Name, sorting.Type).AsQueryable();
                    break;

                case StatementSortBy.CreatedOn:
                    currencies = currencies.Sort(c => c.CreatedOn, sorting.Type).AsQueryable();
                    break;

                case StatementSortBy.CreatedBy:
                    currencies = currencies.Sort(c => c.CreatedBy, sorting.Type).AsQueryable();
                    break;

                case StatementSortBy.UpdatedOn:
                    currencies = currencies.Sort(c => c.UpdatedOn, sorting.Type).AsQueryable();
                    break;

                case StatementSortBy.UpdatedBy:
                    currencies = currencies.Sort(c => c.UpdatedBy, sorting.Type).AsQueryable();
                    break;
            }

            return currencies;
        }
    }
}