using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using TwoPairs.MoneyRecorder.Data;
using TwoPairs.MoneyRecorder.Engine;
using TwoPairs.MoneyRecorder.Engine.Data;
using TwoPairs.MoneyRecorder.Exceptions;
using TwoPairs.TestFramework.Attributes;

namespace TwoPairs.MoneyRecorder.Tests.Unit
{
    [TestFixture]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class CurrencyManagerTests
    {
        private CurrencyManager _currencyManager;
        private Repository _repository;

        [BeforeEach]
        public void BeforeEach()
        {
            SetupMocks();
            SetupTestData();
        }

        private void SetupMocks()
        {
            var container = new Container();
            container.RegisterType<Repository, FakeRepository>(new ContainerControlledLifetimeManager());
            _currencyManager = container.Resolve<CurrencyManager>();
            _repository = container.Resolve<FakeRepository>();
        }

        private void SetupTestData()
        {
            _repository.Users.Add(new User { Id = 1, Name = "Finch" });
            _repository.Users.Add(new User { Id = 2, Name = "Shaw" });
            _repository.Users.Add(new User { Id = 3, Name = "Reese" });

            const int cnt = 35;
            for (var i = 1; i <= cnt; i++)
            {
                var currency = new Currency
                {
                    Id = Guid.NewGuid(),
                    Name = string.Format("Currency_{0}", i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                    Symbol = string.Format("SB_{0}", i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                    CreatedOn = DateTimeOffset.Now.AddSeconds(i),
                    CreatedBy = i % 3 + 1,
                    UpdatedOn = DateTimeOffset.Now.AddSeconds(i),
                    UpdatedBy = i % 3 + 1
                };

                _repository.Currencies.Add(currency);
            }
        }

        [Test]
        public void Can_Create_Currency()
        {
            const string name = "NewCurrencyName", symbol = "NSB";
            var currencyData = new CreateCurrencyData() { Name = name, Symbol = symbol, CreatedBy = 1 };

            _currencyManager.CreateCurrency(currencyData);
            var result = _repository.Currencies.SingleOrDefault(u => u.Name == currencyData.Name);

            result.Should().NotBeNull();
            result.Name.Should().BeEquivalentTo(name);
            result.Symbol.Should().BeEquivalentTo(symbol);
        }

        [Test]
        public void Can_Update_Currency()
        {
            const string name = "UpdateCurrencyName", symbol = "USB";
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var currencyData = new UpdateCurrencyData() { Id = currency.Id, Name = name, Symbol = symbol, UpdatedBy = 1 };

            _currencyManager.UpdateCurrency(currencyData);
            var result = _repository.Currencies.SingleOrDefault(u => u.Name == currencyData.Name);

            result.Should().NotBeNull();
            result.Id.Should().Be(currency.Id);
            result.Name.Should().BeEquivalentTo(name);
            result.Symbol.Should().BeEquivalentTo(symbol);
        }

        [Test]
        public void Can_Delete_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            _currencyManager.DeleteCurrency(currency.Id);
            var result = _repository.Currencies.SingleOrDefault(u => u.Id == currency.Id);

            result.Should().BeNull();
        }

        [Test]
        public void Can_Get_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var result = _currencyManager.GetCurrency(currency.Id);

            result.Should().NotBeNull();
            result.Name.Should().BeEquivalentTo(currency.Name);
            result.Symbol.Should().BeEquivalentTo(currency.Symbol);
        }

        [Test]
        public void Can_Get_Currencies()
        {
            var result = _currencyManager.GetCurrencies();

            result.Should().NotBeNull();
            result.Count().Should().Be(35);
        }

        [TestCase("", 35)]
        [TestCase("Currency_0", 9)]
        [Test]
        public void Can_Get_Currency_Count(string keyword, int expectedCount)
        {
            var count = _currencyManager.GetCurrencyCount(keyword);
            count.Should().Be(expectedCount);
        }

        #region Exceptions

        [Test]
        public void Should_Throw_DuplicateCurrencyException_When_Creating_Duplicate_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var exceptionData = new CreateCurrencyData
            {
                Name = currency.Name,
                Symbol = "NSB",
                CreatedBy = currency.CreatedBy
            };

            Action act = () => _currencyManager.CreateCurrency(exceptionData);
            act.ShouldThrow<DuplicateCurrencyException>();
        }

        [Test]
        public void Should_Throw_DuplicateCurrencySymbol_When_Creating_Duplicate_Currency_Symbol()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var exceptionData = new CreateCurrencyData
            {
                Name = "NewCurrencyName",
                Symbol = currency.Symbol,
                CreatedBy = currency.CreatedBy
            };

            Action act = () => _currencyManager.CreateCurrency(exceptionData);
            act.ShouldThrow<DuplicateCurrencySymbolException>();
        }

        [Test]
        public void Should_Throw_CurrencyNotFoundException_When_Updating_Nonexist_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var exceptionData = new UpdateCurrencyData
            {
                Id = Guid.NewGuid(),
                Name = currency.Name,
                Symbol = "NSB",
                UpdatedBy = currency.CreatedBy
            };

            Action act = () => _currencyManager.UpdateCurrency(exceptionData);
            act.ShouldThrow<CurrencyNotFoundException>();
        }

        [Test]
        public void Should_Throw_DuplicateCurrencyException_When_Updating_Duplicate_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var existCurrency = _repository.Currencies.FirstOrDefault(c => c.Id != currency.Id);
            existCurrency.Should().NotBeNull();

            var exceptionData = new UpdateCurrencyData
            {
                Id = currency.Id,
                Name = existCurrency.Name,
                Symbol = "NSB",
                UpdatedBy = currency.CreatedBy
            };

            Action act = () => _currencyManager.UpdateCurrency(exceptionData);
            act.ShouldThrow<DuplicateCurrencyException>();
        }

        [Test]
        public void Should_Throw_DuplicateCurrencySymbol_When_Updating_Duplicate_Currency_Symbol()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            var existCurrency = _repository.Currencies.FirstOrDefault(c => c.Id != currency.Id);
            existCurrency.Should().NotBeNull();

            var exceptionData = new UpdateCurrencyData
            {
                Id = currency.Id,
                Name = "NewCurrencyName",
                Symbol = existCurrency.Symbol,
                UpdatedBy = currency.CreatedBy
            };

            Action act = () => _currencyManager.UpdateCurrency(exceptionData);
            act.ShouldThrow<DuplicateCurrencySymbolException>();
        }

        [Test]
        public void Should_Throw_CurrencyNotFoundException_When_Deleting_Nonexist_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            Action act = () => _currencyManager.DeleteCurrency(Guid.NewGuid());
            act.ShouldThrow<CurrencyNotFoundException>();
        }

        [Test]
        public void Should_Throw_CurrencyNotFoundException_When_Getting_Nonexist_Currency()
        {
            var currency = _repository.Currencies.FirstOrDefault();
            currency.Should().NotBeNull();

            Action act = () => _currencyManager.GetCurrency(Guid.NewGuid());
            act.ShouldThrow<CurrencyNotFoundException>();
        }
        #endregion Exceptions
    }
}