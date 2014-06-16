using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using TwoPairs.MoneyRecorder.Data;
using TwoPairs.MoneyRecorder.Engine.Data;
using TwoPairs.MoneyRecorder.Exceptions;

namespace TwoPairs.MoneyRecorder
{
    public class CurrencyManager
    {
        private readonly Func<Repository> _repositoryFactory;

        public CurrencyManager(Func<Repository> repositoryFactory)
        {
            Contract.Requires(repositoryFactory != null);
            _repositoryFactory = repositoryFactory;
        }

        public void CreateCurrency(CreateCurrencyData currencyData)
        {
            Contract.Requires(currencyData != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(currencyData.Name));
            Contract.Requires(!string.IsNullOrWhiteSpace(currencyData.Symbol));

            if (CurrencyNameExist(currencyData.Name))
            {
                throw new DuplicateCurrencyException(currencyData.Name);
            }

            if (CurrencySymbolExist(currencyData.Symbol))
            {
                throw new DuplicateCurrencySymbolException(currencyData.Symbol);
            }

            using (var repository = _repositoryFactory())
            {
                var currency = new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = currencyData.Name,
                    Symbol = currencyData.Symbol,
                    CreatedBy = currencyData.CreatedBy,
                    CreatedOn = DateTimeOffset.Now
                };
                repository.Currencies.Attach(currency);

                repository.Currencies.Add(currency);
                repository.SaveChanges();
            }
        }

        public void UpdateCurrency(UpdateCurrencyData currencyData)
        {
            Contract.Requires(currencyData != null);
            Contract.Requires(!Guid.Empty.Equals(currencyData.Id));
            Contract.Requires(!string.IsNullOrWhiteSpace(currencyData.Name));
            Contract.Requires(!string.IsNullOrWhiteSpace(currencyData.Symbol));

            using (var repository = _repositoryFactory())
            {
                var currency = repository.Currencies.SingleOrDefault(u => u.Id == currencyData.Id);

                if (currency == null)
                {
                    throw new CurrencyNotFoundException(currencyData.Name);
                }

                if (CurrencyNameExist(currencyData.Name))
                {
                    throw new DuplicateCurrencyException(currencyData.Name);
                }

                if (CurrencySymbolExist(currencyData.Symbol))
                {
                    throw new DuplicateCurrencySymbolException(currencyData.Symbol);
                }

                currency.Name = currencyData.Name;
                currency.Symbol = currencyData.Symbol;
                currency.UpdatedBy = currencyData.UpdatedBy;
                currency.UpdatedOn = DateTimeOffset.Now;

                repository.Currencies.Attach(currency);

                repository.SetEntityState(currency, EntityState.Modified);
                repository.SaveChanges();
            }
        }

        public void DeleteCurrency(Guid id)
        {
            Contract.Requires(!Guid.Empty.Equals(id));

            using (var repository = _repositoryFactory())
            {
                var currency = repository.Currencies.SingleOrDefault(u => u.Id == id);

                if (currency == null)
                {
                    throw new CurrencyNotFoundException(id.ToString());
                }

                repository.Currencies.Remove(currency);
                repository.SaveChanges();
            }
        }

        public GetCurrencyData GetCurrency(Guid id)
        {
            Contract.Requires(!Guid.Empty.Equals(id));

            using (var repository = _repositoryFactory())
            {
                var currency = repository.Currencies.SingleOrDefault(u => u.Id == id);

                if (currency == null)
                {
                    throw new CurrencyNotFoundException(id.ToString());
                }

                return new GetCurrencyData
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    Symbol = currency.Symbol
                };
            }
        }

        public GetCurrencyData[] GetCurrencies(string keyword = "", Sort<CurrencySortBy> sorting = null, Paging paging = null)
        {
            using (var repository = _repositoryFactory())
            {
                var currencies = repository.Currencies.Join(repository.Users, c => c.CreatedBy, u => u.Id, (g, c) => new { Currency = g, User = c })
                                                      .Join(repository.Users, g => g.Currency.UpdatedBy, u => u.Id, (g, u) => new GetCurrencyData
                                                      {
                                                          Id = g.Currency.Id,
                                                          Name = g.Currency.Name,
                                                          Symbol = g.Currency.Symbol,
                                                          CreatedBy = g.User.Name,
                                                          CreatedOn = g.Currency.CreatedOn,
                                                          UpdatedOn = g.Currency.UpdatedOn,
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

        public int GetCurrencyCount(string keyword)
        {
            using (var repository = _repositoryFactory())
            {
                var currencies = repository.Currencies.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    currencies = currencies.Where(u => u.Name.Contains(keyword));
                }

                return currencies.Count();
            }
        }

        private bool CurrencyNameExist(string name)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.Currencies.Any(u => String.Equals(u.Name, name.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private bool CurrencySymbolExist(string symbol)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.Currencies.Any(u => String.Equals(u.Symbol, symbol.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private static IQueryable<GetCurrencyData> SortGroup(Sort<CurrencySortBy> sorting, IQueryable<GetCurrencyData> currencies)
        {
            switch (sorting.By)
            {
                case CurrencySortBy.Name:
                    currencies = currencies.Sort(u => u.Name, sorting.Type).AsQueryable();
                    break;

                case CurrencySortBy.Symbol:
                    currencies = currencies.Sort(u => u.Symbol, sorting.Type).AsQueryable();
                    break;

                case CurrencySortBy.CreatedOn:
                    currencies = currencies.Sort(c => c.CreatedOn, sorting.Type).AsQueryable();
                    break;

                case CurrencySortBy.CreatedBy:
                    currencies = currencies.Sort(c => c.CreatedBy, sorting.Type).AsQueryable();
                    break;

                case CurrencySortBy.UpdatedOn:
                    currencies = currencies.Sort(c => c.UpdatedOn, sorting.Type).AsQueryable();
                    break;

                case CurrencySortBy.UpdatedBy:
                    currencies = currencies.Sort(c => c.UpdatedBy, sorting.Type).AsQueryable();
                    break;
            }

            return currencies;
        }
    }
}