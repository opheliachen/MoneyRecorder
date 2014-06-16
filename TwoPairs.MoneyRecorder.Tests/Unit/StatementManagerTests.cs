using System;
using System.Collections.ObjectModel;
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
    public class StatementManagerTests
    {
        private StatementManager _statementManager;
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
            _statementManager = container.Resolve<StatementManager>();
            _repository = container.Resolve<FakeRepository>();
        }

        private void SetupTestData()
        {
            _repository.Users.Add(new User { Id = 1, Name = "Finch" });
            _repository.Users.Add(new User { Id = 2, Name = "Shaw" });
            _repository.Users.Add(new User { Id = 3, Name = "Reese" });

            _repository.Currencies.Add(new Currency { Id = Guid.NewGuid(), Name = "AUD", Symbol = "AUD", Statements = new Collection<Statement>() });
            _repository.Currencies.Add(new Currency { Id = Guid.NewGuid(), Name = "TWD", Symbol = "TWD", Statements = new Collection<Statement>() });
            _repository.Currencies.Add(new Currency { Id = Guid.NewGuid(), Name = "USD", Symbol = "USD", Statements = new Collection<Statement>() });

            const int cnt = 35;
            Currency currency;
            for (var i = 1; i <= cnt; i++)
            {
                currency = _repository.Currencies.ElementAt(i%3);
                var statement = new Statement
                {
                    Id = Guid.NewGuid(),
                    Name = string.Format("Statement_{0}", i.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                    CurrencyId = currency.Id,
                    Currency = currency,
                    CreatedOn = DateTimeOffset.Now.AddSeconds(i),
                    CreatedBy = i % 3 + 1,
                    UpdatedOn = DateTimeOffset.Now.AddSeconds(i),
                    UpdatedBy = i % 3 + 1
                };

                currency.Statements.Add(statement);
                _repository.Statements.Add(statement);
            }
        }

        [Test]
        public void Can_Create_Statement()
        {
            const string name = "NewStatementName";
            var currencyId = _repository.Currencies.FirstOrDefault().Id;
            var statementData = new CreateStatementData() { Name = name, CurrencyId = currencyId, CreatedBy = 1 };

            _statementManager.CreateStatement(statementData);
            var result = _repository.Statements.SingleOrDefault(u => u.Name == statementData.Name);

            result.Should().NotBeNull();
            result.Name.Should().BeEquivalentTo(name);
            result.CurrencyId.Should().Be(currencyId);
        }

        [Test]
        public void Can_Update_Statement()
        {
            const string name = "UpdateStatementName";
            var currencyId = _repository.Currencies.FirstOrDefault().Id;

            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            var statementData = new UpdateStatementData() { Id = statement.Id, Name = name, CurrencyId = currencyId, UpdatedBy = 1 };

            _statementManager.UpdateStatement(statementData);
            var result = _repository.Statements.SingleOrDefault(u => u.Name == statementData.Name);

            result.Should().NotBeNull();
            result.Id.Should().Be(statement.Id);
            result.Name.Should().BeEquivalentTo(name);
            result.CurrencyId.Should().Be(currencyId);
        }

        [Test]
        public void Can_Delete_Statement()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            _statementManager.DeleteStatement(statement.Id);
            var result = _repository.Statements.SingleOrDefault(u => u.Id == statement.Id);

            result.Should().BeNull();
        }

        [Test]
        public void Can_Get_Statement()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            var result = _statementManager.GetStatement(statement.Id);

            result.Should().NotBeNull();
            result.Name.Should().BeEquivalentTo(statement.Name);
            result.Currency.Id.Should().Be(statement.CurrencyId);
        }

        [Test]
        public void Can_Get_Statements()
        {
            var result = _statementManager.GetStatements();

            result.Should().NotBeNull();
            result.Count().Should().Be(35);
        }

        [TestCase("", 35)]
        [TestCase("Statement_0", 9)]
        [Test]
        public void Can_Get_Statement_Count(string keyword, int expectedCount)
        {
            var count = _statementManager.GetStatementCount(keyword);
            count.Should().Be(expectedCount);
        }

        #region Exceptions
        [Test]
        public void Should_Throw_CurrencyNotFoundException_When_Creating_Statement_With_Nonexist_Currency()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            var exceptionData = new CreateStatementData
            {
                Name = "NewStatement",
                CurrencyId = Guid.NewGuid(),
                CreatedBy = statement.CreatedBy
            };

            Action act = () => _statementManager.CreateStatement(exceptionData);
            act.ShouldThrow<CurrencyNotFoundException>();
        }

        [Test]
        public void Should_Throw_StatementNotFoundException_When_Updating_Nonexist_Statement()
        {
            var currencyId = _repository.Currencies.FirstOrDefault().Id;
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            var exceptionData = new UpdateStatementData
            {
                Id = Guid.NewGuid(),
                Name = statement.Name,
                CurrencyId = currencyId,
                UpdatedBy = statement.CreatedBy
            };

            Action act = () => _statementManager.UpdateStatement(exceptionData);
            act.ShouldThrow<StatementNotFoundException>();
        }

        [Test]
        public void Should_Throw_CurrencyNotFoundException_When_Updating_Statement_With_Nonexist_Currency()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            var exceptionData = new UpdateStatementData
            {
                Id = statement.Id,
                Name = "UpdateStatement",
                CurrencyId = Guid.NewGuid(),
                UpdatedBy = statement.UpdatedBy
            };

            Action act = () => _statementManager.UpdateStatement(exceptionData);
            act.ShouldThrow<CurrencyNotFoundException>();
        }

        [Test]
        public void Should_Throw_StatementNotFoundException_When_Deleting_Nonexist_Statement()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            Action act = () => _statementManager.DeleteStatement(Guid.NewGuid());
            act.ShouldThrow<StatementNotFoundException>();
        }

        [Test]
        public void Should_Throw_StatementNotFoundException_When_Getting_Nonexist_Statement()
        {
            var statement = _repository.Statements.FirstOrDefault();
            statement.Should().NotBeNull();

            Action act = () => _statementManager.GetStatement(Guid.NewGuid());
            act.ShouldThrow<StatementNotFoundException>();
        }
        #endregion Exceptions
    }
}