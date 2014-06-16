using System.Data.Entity;
using TwoPairs.MoneyRecorder.Data;
using TwoPairs.MoneyRecorder.Engine.Common;

namespace TwoPairs.MoneyRecorder.Engine.Data
{
    public class FakeRepository : Repository
    {
        private readonly FakeDbSet<Currency> _currencies;
        private readonly FakeDbSet<Statement> _statements;
        private readonly FakeDbSet<User> _users;

        public override IDbSet<Currency> Currencies { get { return _currencies; } }

        public override IDbSet<Statement> Statements { get { return _statements; } }

        public override IDbSet<User> Users { get { return _users; } }

        public FakeRepository()
        {
            _currencies = new FakeDbSet<Currency>();
            _statements = new FakeDbSet<Statement>();
            _users = new FakeDbSet<User>();
        }

        public override int SaveChanges()
        {
            return 0;
        }

        public override void SetEntityState<T>(T entity, EntityState state)
        {
        }
    }
}